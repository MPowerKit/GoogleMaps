using System.Collections.ObjectModel;
using System.IO.Compression;
using System.Text;
using System.Windows.Input;
using System.Xml;

using Microsoft.Maui.Controls.Shapes;

using MPowerKit.GoogleMaps.Data;

using IGeometry = MPowerKit.GoogleMaps.Data.IGeometry;

namespace MPowerKit.GoogleMaps;

public partial class GoogleMap
{
    protected CancellationTokenSource? KmlCts;
    protected KmlParser? KmlParser;

    public event Action? KmlParsed;

    protected List<GroundOverlay>? KmlGroundOverlays { get; set; }
    protected List<Pin>? KmlPins { get; set; }
    protected List<Polyline>? KmlPolylines { get; set; }
    protected List<Polygon>? KmlPolygons { get; set; }

    protected virtual void InitKml()
    {
        this.PropertyChanging += GoogleMap_Kml_PropertyChanging;
        this.PropertyChanged += GoogleMap_Kml_PropertyChanged;
    }

    private void GoogleMap_Kml_PropertyChanging(object sender, PropertyChangingEventArgs e)
    {
        if (e.PropertyName == KmlProperty.PropertyName)
        {
            OnKmlChanging();
        }
    }

    private void GoogleMap_Kml_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == KmlProperty.PropertyName)
        {
            OnKmlChanged();
        }
    }

    protected virtual void OnKmlChanging()
    {
        try
        {
            KmlCts?.Cancel();
            KmlCts?.Dispose();
            KmlCts = null;
        }
        catch { }

        RemoveKmlPins();
        RemoveKmlPolylines();
        RemoveKmlPolygons();
        RemoveKmlGroundOverlays();

        KmlParser = null;
    }

    protected virtual async Task OnKmlChanged()
    {
        KmlCts = new();

        ImageSource? source = null;

        if (!Kml.StartsWith("<?xml", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                source = Kml;
            }
            catch (Exception ex)
            {
                return;
            }
        }

        var token = KmlCts.Token;

        if (source is not null)
        {
            if (source is UriImageSource uriSource)
            {
                await ParseKmlFromUri(uriSource.Uri, token);
            }
            else if (source is FileImageSource fileSource)
            {
                await ParseKmlFromFile(fileSource.File, token);
            }
        }
        else
        {
            await ParseKmlFromString(Kml, token);
        }

        OnKmlParsed();

        KmlParser = null;
    }

    protected virtual void OnKmlParsed()
    {
        KmlParsed?.Invoke();

        if (KmlParsedCommand?.CanExecute(null) is true)
            KmlParsedCommand.Execute(null);
    }

    protected virtual async Task ParseKmlFromString(string kml, CancellationToken token = default)
    {
        try
        {
            var byteArray = Encoding.UTF8.GetBytes(kml);
            using MemoryStream stream = new(byteArray);
            await ParseKmlFromStream(stream, token);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    protected virtual async Task ParseKmlFromUri(Uri uri, CancellationToken token = default)
    {
        try
        {
            using HttpClient httpClient = new();

            var response = await httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, token);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync(token);
            await ParseKmlFromStream(stream, token);
        }
        catch (OperationCanceledException)
        {

        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    protected virtual async Task ParseKmlFromFile(string file, CancellationToken token = default)
    {
        if (!File.Exists(file))
        {
            if (!await FileSystem.AppPackageFileExistsAsync(file)) return;
            try
            {
                using var fs = await FileSystem.OpenAppPackageFileAsync(file);
                await ParseKmlFromStream(fs, token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return;
        }

        try
        {
            using var fs = File.OpenRead(file);
            await ParseKmlFromStream(fs, token);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    protected virtual async Task ParseKmlFromStream(Stream stream, CancellationToken token = default)
    {
        try
        {
            using MemoryStream ms = new();
            await stream.CopyToAsync(ms, token);
            ms.Position = 0;

            var parser = await ParseKmz(ms, token);
            if (parser is null)
            {
                ms.Position = 0;
                parser = ParseKmlStream(ms);
            }

            if (token.IsCancellationRequested) return;

            KmlParser = parser;

            RenderKml(token);
        }
        catch (OperationCanceledException)
        {

        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    protected virtual async Task<KmlParser?> ParseKmz(Stream stream, CancellationToken token = default)
    {
        KmlParser? parser = null;
        try
        {
            using ZipArchive zip = new(stream);

            Dictionary<string, byte[]> images = [];
            foreach (var entry in zip.Entries)
            {
                using var entryStream = entry.Open();
                if (parser is null && entry.Name.EndsWith(".kml", StringComparison.OrdinalIgnoreCase))
                {
                    parser = ParseKmlStream(entryStream);
                }
                else
                {
                    using MemoryStream ms = new();
                    await entryStream.CopyToAsync(ms, token);
                    images[entry.FullName] = ms.ToArray();
                }
            }

            if (parser is not null) parser.Images = images;

            return parser;
        }
        catch (Exception)
        {
            return null;
        }
    }

    protected virtual KmlParser? ParseKmlStream(Stream stream)
    {
        using XmlTextReader reader = new(stream);

        KmlParser? parser = new(reader);

        parser.ParseKml();

        return parser;
    }

    protected virtual void RenderKml(CancellationToken token = default)
    {
        if (KmlParser is null) return;

        AddKmlPlacemarks(token);

        AddKmlGroundOverlays(token);
    }

    protected virtual void AddKmlPlacemarks(CancellationToken token = default)
    {
        if (KmlParser?.Placemarks?.Count is null or 0) return;

        List<VisualElement> mapObjects = [];
        foreach (var placemark in KmlParser.Placemarks)
        {
            if (token.IsCancellationRequested) return;

            var objects = GetMapObjectFromKmlPlacemark(placemark, placemark.Geometry);
            mapObjects.AddRange(objects);
        }

        if (token.IsCancellationRequested) return;

        AddKmlPins(mapObjects.OfType<Pin>());

        if (token.IsCancellationRequested) return;

        AddKmlPolylines(mapObjects.OfType<Polyline>());

        if (token.IsCancellationRequested) return;

        AddKmlPolygons(mapObjects.OfType<Polygon>());
    }

    protected virtual IEnumerable<VisualElement> GetMapObjectFromKmlPlacemark(KmlPlacemark placemark, IGeometry geometry)
    {
        List<VisualElement> elements = [];
        switch (geometry)
        {
            case PointGeometry pointGeometry:
                {
                    Pin pin = new()
                    {
                        Position = pointGeometry.Position,
                        ZIndex = placemark.ZIndex,
                        IsVisible = placemark.IsVisible,
                        BindingContext = placemark
                    };

                    if (placemark.InlineStyle is not null || placemark.Style is not null)
                    {
                        ApplyKmlPinStyle(pin, placemark);
                    }
                    else SetKmlPinText(pin, placemark, null);

                    SetKmlPinInfoWindow(pin);

                    elements.Add(pin);
                }
                break;
            case LineString lineGeometry:
                {
                    Polyline polyline = new()
                    {
                        Points = new([.. lineGeometry.Points]),
                        StrokeThickness = 1d,
                        ZIndex = placemark.ZIndex,
                        IsVisible = placemark.IsVisible,
                        BindingContext = placemark
                    };

                    if (placemark.InlineStyle is not null || placemark.Style is not null)
                    {
                        ApplyKmlPolylineStyle(polyline, placemark);
                    }

                    elements.Add(polyline);
                }
                break;
            case KmlPolygon polygonGeometry:
                {
                    Polygon polygon = new()
                    {
                        Points = new([.. polygonGeometry.OuterBoundaryCoordinates]),
                        ZIndex = placemark.ZIndex,
                        IsVisible = placemark.IsVisible,
                        BindingContext = placemark
                    };

                    if (polygonGeometry.InnerBoundaryCoordinates?.Count() > 0)
                    {
                        PolygonAttached.SetHoles(polygon, polygonGeometry.InnerBoundaryCoordinates);
                    }

                    ApplyKmlPolygonStyle(polygon, placemark);

                    elements.Add(polygon);
                }
                break;
            case MultiGeometry multiGeometry:
                {
                    foreach (var innerGeometry in multiGeometry.Geometries)
                    {
                        elements.AddRange(GetMapObjectFromKmlPlacemark(placemark, innerGeometry));
                    }
                }
                break;
            default:
                break;
        }

        return elements;
    }

    protected virtual void ApplyKmlPinStyle(Pin pin, KmlPlacemark placemark)
    {
        if (placemark.InlineStyle is null && placemark.Style is null) return;

        var inlineMarkerStyle = placemark.InlineStyle?.GetMarkerStyle();
        var markerStyle = placemark.Style?.GetMarkerStyle();

        var anchor = inlineMarkerStyle?.Anchor ?? markerStyle?.Anchor;
        if (anchor is Point)
        {
            pin.AnchorX = anchor.Value.X;
            pin.AnchorY = anchor.Value.Y;
        }

        var rotation = inlineMarkerStyle?.Rotation ?? markerStyle?.Rotation;
        if (rotation is double)
        {
            pin.Rotation = rotation.Value;
        }

        var scale = inlineMarkerStyle?.Scale ?? markerStyle?.Scale;
        if (scale is double)
        {
            pin.Scale = scale.Value;
        }

        var iconColor = inlineMarkerStyle?.IconColor ?? markerStyle?.IconColor;
        if (iconColor is string color)
        {
            pin.DefaultIconColor = Color.FromArgb(color);
        }

        var infoWindowText = inlineMarkerStyle?.InfoWindowText ?? markerStyle?.InfoWindowText;
        SetKmlPinText(pin, placemark, infoWindowText);

        var icon = inlineMarkerStyle?.Icon ?? markerStyle?.Icon;
        if (icon is not null)
        {
            if (KmlParser?.Images?.TryGetValue(icon, out var imageBytes) is true)
            {
                pin.Icon = ImageSource.FromStream(() => new MemoryStream([.. imageBytes]));
            }
            else pin.Icon = icon;
        }
    }

    protected virtual void SetKmlPinText(Pin pin, KmlPlacemark placemark, string? infoWindowText)
    {
        if (!string.IsNullOrWhiteSpace(infoWindowText))
        {
            pin.Title = KmlUtil.SubstituteProperties(infoWindowText, placemark);
        }
        else if (!string.IsNullOrWhiteSpace(placemark.Name) && !string.IsNullOrWhiteSpace(placemark.Description))
        {
            pin.Title = placemark.Name;
            pin.Snippet = placemark.Description;
        }
        else if (!string.IsNullOrWhiteSpace(placemark.Name))
        {
            pin.Title = placemark.Name;
        }
        else if (!string.IsNullOrWhiteSpace(placemark.Description))
        {
            pin.Title = placemark.Description;
        }
        else return;
    }

    protected virtual void SetKmlPinInfoWindow(Pin pin)
    {
        var text = pin.Title;
        if (!string.IsNullOrWhiteSpace(pin.Snippet))
        {
            text = $"<b>{text}</b><br>{text}";
        }

        if (string.IsNullOrWhiteSpace(text)) return;

#if ANDROID
        pin.InfoWindow = new ContentView()
        {
            Padding = 10,
            BackgroundColor = Colors.White,
            Content = new Label()
            {
                TextColor = Colors.Black,
                LineBreakMode = LineBreakMode.WordWrap,
                TextType = TextType.Html,
                Text = text
            }
        };
#endif
    }

    protected virtual void ApplyKmlPolylineStyle(Polyline polyline, KmlPlacemark placemark)
    {
        if (placemark.InlineStyle is null && placemark.Style is null) return;

        var inlineMarkerStyle = placemark.InlineStyle?.GetPolylineStyle();
        var markerStyle = placemark.Style?.GetPolylineStyle();

        var stroke = inlineMarkerStyle?.Stroke ?? markerStyle?.Stroke;
        if (stroke is string color)
        {
            polyline.Stroke = Color.FromArgb(color);
        }

        var strokeWidth = inlineMarkerStyle?.StrokeWidth ?? markerStyle?.StrokeWidth;
        if (strokeWidth is double width)
        {
            polyline.StrokeThickness = width;
        }
    }

    protected virtual void ApplyKmlPolygonStyle(Polygon polygon, KmlPlacemark placemark)
    {
        if (placemark.InlineStyle is null && placemark.Style is null) return;

        var inlineMarkerStyle = placemark.InlineStyle?.GetPolygonStyle();
        var markerStyle = placemark.Style?.GetPolygonStyle();

        var fill = inlineMarkerStyle?.Fill ?? markerStyle?.Fill;
        if (fill is string fillColor)
        {
            polygon.Stroke = Color.FromArgb(fillColor);
            polygon.Fill = Color.FromArgb(fillColor);
        }

        var stroke = inlineMarkerStyle?.Stroke ?? markerStyle?.Stroke;
        if (stroke is string color)
        {
            polygon.Stroke = Color.FromArgb(color);
        }

        var strokeWidth = inlineMarkerStyle?.StrokeWidth ?? markerStyle?.StrokeWidth;
        if (strokeWidth is double width)
        {
            polygon.StrokeThickness = width;
        }
    }

    protected virtual void AddKmlPins(IEnumerable<Pin> pins)
    {
        if (!pins.Any()) return;

        ObservableCollection<Pin> observablePins = new(pins);

        if (Pins is null)
        {
            Pins = observablePins;
        }
        else if (Pins is ICollection<Pin> list)
        {
            foreach (var pin in observablePins)
            {
                list.Add(pin);
            }
        }

        KmlPins = [.. observablePins];
    }

    protected virtual void AddKmlPolylines(IEnumerable<Polyline> polylines)
    {
        if (!polylines.Any()) return;

        ObservableCollection<Polyline> observablePolylines = new(polylines);

        if (Polylines is null)
        {
            Polylines = observablePolylines;
        }
        else if (Polylines is ICollection<Polyline> list)
        {
            foreach (var polyline in observablePolylines)
            {
                list.Add(polyline);
            }
        }

        KmlPolylines = [.. observablePolylines];
    }

    protected virtual void AddKmlPolygons(IEnumerable<Polygon> polygons)
    {
        if (!polygons.Any()) return;

        ObservableCollection<Polygon> observablePolygons = new(polygons);

        if (Polygons is null)
        {
            Polygons = observablePolygons;
        }
        else if (Polygons is ICollection<Polygon> list)
        {
            foreach (var polygon in observablePolygons)
            {
                list.Add(polygon);
            }
        }

        KmlPolygons = [.. observablePolygons];
    }

    protected virtual void AddKmlGroundOverlays(CancellationToken token = default)
    {
        if (KmlParser?.GroundOverlays?.Count is null or 0) return;

        ObservableCollection<GroundOverlay> overlays = [];
        foreach (var kmlOverlay in KmlParser.GroundOverlays)
        {
            if (token.IsCancellationRequested) return;

            var overlay = kmlOverlay.GroundOverlay;

            GroundOverlay mapped = new()
            {
                Image = overlay.Image,
                OverlayBounds = overlay.Bounds,
                Rotation = overlay.Rotation,
                ZIndex = overlay.ZIndex,
                IsVisible = overlay.IsVisible,
                BindingContext = kmlOverlay
            };

            overlays.Add(mapped);
        }

        if (token.IsCancellationRequested) return;

        if (GroundOverlays is null)
        {
            GroundOverlays = overlays;
        }
        else if (GroundOverlays is ICollection<GroundOverlay> list)
        {
            foreach (var overlay in overlays)
            {
                list.Add(overlay);
            }
        }

        KmlGroundOverlays = [.. overlays];
    }

    protected virtual void RemoveKmlGroundOverlays()
    {
        if (KmlGroundOverlays?.Count is null or 0
            || GroundOverlays is not ICollection<GroundOverlay> changableCollection
            || changableCollection.Count == 0) return;

        foreach (var overlay in KmlGroundOverlays.ToList())
        {
            changableCollection.Remove(overlay);
        }
    }

    protected virtual void RemoveKmlPins()
    {
        if (KmlPins?.Count is null or 0
            || Pins is not ICollection<Pin> changableCollection
            || changableCollection.Count == 0) return;

        foreach (var pin in KmlPins.ToList())
        {
            changableCollection.Remove(pin);
        }
    }

    protected virtual void RemoveKmlPolylines()
    {
        if (KmlPolylines?.Count is null or 0
            || Polylines is not ICollection<Polyline> changableCollection
            || changableCollection.Count == 0) return;

        foreach (var polyline in KmlPolylines.ToList())
        {
            changableCollection.Remove(polyline);
        }
    }

    protected virtual void RemoveKmlPolygons()
    {
        if (KmlPolygons?.Count is null or 0
            || Polygons is not ICollection<Polygon> changableCollection
            || changableCollection.Count == 0) return;

        foreach (var polygon in KmlPolygons.ToList())
        {
            changableCollection.Remove(polygon);
        }
    }

    #region Kml
    public string Kml
    {
        get => (string)GetValue(KmlProperty);
        set => SetValue(KmlProperty, value);
    }

    public static readonly BindableProperty KmlProperty =
        BindableProperty.Create(
            nameof(Kml),
            typeof(string),
            typeof(GoogleMap));
    #endregion

    #region KmlParsedCommand
    public ICommand KmlParsedCommand
    {
        get => (ICommand)GetValue(KmlParsedCommandProperty);
        set => SetValue(KmlParsedCommandProperty, value);
    }

    public static readonly BindableProperty KmlParsedCommandProperty =
        BindableProperty.Create(
            nameof(KmlParsedCommand),
            typeof(ICommand),
            typeof(GoogleMap));
    #endregion
}