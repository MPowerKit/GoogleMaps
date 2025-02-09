using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

using Microsoft.Maui.Controls.Shapes;

using MPowerKit.GoogleMaps.Data;

using IGeometry = MPowerKit.GoogleMaps.Data.IGeometry;

namespace MPowerKit.GoogleMaps;

public partial class GoogleMap
{
    protected CancellationTokenSource? GeoJsonCts;
    protected GeoJsonParser? GeoJsonParser;

    public event Action? GeoJsonParsed;

    protected List<Pin>? GeoJsonPins { get; set; }
    protected List<Polyline>? GeoJsonPolylines { get; set; }
    protected List<Polygon>? GeoJsonPolygons { get; set; }

    protected virtual void InitGeoJson()
    {
        this.PropertyChanging += GoogleMap_GeoJson_PropertyChanging;
        this.PropertyChanged += GoogleMap_GeoJson_PropertyChanged;
    }

    private void GoogleMap_GeoJson_PropertyChanging(object sender, PropertyChangingEventArgs e)
    {
        if (e.PropertyName == GeoJsonProperty.PropertyName)
        {
            OnGeoJsonChanging();
        }
    }

    private void GoogleMap_GeoJson_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == GeoJsonProperty.PropertyName)
        {
            OnGeoJsonChanged();
        }
    }

    protected virtual void OnGeoJsonChanging()
    {
        try
        {
            GeoJsonCts?.Cancel();
            GeoJsonCts?.Dispose();
            GeoJsonCts = null;
        }
        catch { }

        RemoveGeoJsonPins();
        RemoveGeoJsonPolylines();
        RemoveGeoJsonPolygons();

        GeoJsonParser = null;
    }

    protected virtual async Task OnGeoJsonChanged()
    {
        GeoJsonCts = new();

        var token = GeoJsonCts.Token;

        try
        {
            try
            {
                ParseGeoJsonString(GeoJson, token);
                GeoJsonParser = null;
                return;
            }
            catch (Exception ex)
            {

            }

            ImageSource? source = GeoJson;

            if (source is not null)
            {
                if (source is UriImageSource uriSource)
                {
                    await ParseGeoJsonFromUri(uriSource.Uri, token);
                }
                else if (source is FileImageSource fileSource)
                {
                    await ParseGeoJsonFromFile(fileSource.File, token);
                }
            }
        }
        finally
        {
            OnGeoJsonParsed();

            GeoJsonParser = null;
        }
    }

    protected virtual void OnGeoJsonParsed()
    {
        GeoJsonParsed?.Invoke();

        if (GeoJsonParsedCommand?.CanExecute(null) is true)
            GeoJsonParsedCommand.Execute(null);
    }

    protected virtual async Task ParseGeoJsonFromUri(Uri uri, CancellationToken token = default)
    {
        try
        {
            using HttpClient httpClient = new();

            var response = await httpClient.GetAsync(uri, token);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(token);
            ParseGeoJsonString(json, token);
        }
        catch (OperationCanceledException)
        {

        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex}");
        }
    }

    protected virtual async Task ParseGeoJsonFromFile(string file, CancellationToken token = default)
    {
        if (!File.Exists(file))
        {
            if (!await FileSystem.AppPackageFileExistsAsync(file)) return;
            try
            {
                using var fs = await FileSystem.OpenAppPackageFileAsync(file);
                var sr = new StreamReader(fs);
                ParseGeoJsonString(await sr.ReadToEndAsync(token), token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex}");
            }

            return;
        }

        try
        {
            using var fs = File.OpenRead(file);
            var sr = new StreamReader(fs);
            ParseGeoJsonString(await sr.ReadToEndAsync(token), token);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex}");
        }
    }

    protected virtual void ParseGeoJsonString(string json, CancellationToken token = default)
    {
        if (token.IsCancellationRequested) return;

        GeoJsonParser = new(Encoding.UTF8.GetBytes(json));

        GeoJsonParser.ParseGeoJson();

        RenderGeoJson(token);
    }

    protected virtual void RenderGeoJson(CancellationToken token = default)
    {
        if (GeoJsonParser is null) return;

        AddGeoJsonFeatures(token);
    }

    protected virtual void AddGeoJsonFeatures(CancellationToken token = default)
    {
        if (GeoJsonParser?.Features?.Count is null or 0) return;

        List<VisualElement> mapObjects = [];
        foreach (var feature in GeoJsonParser.Features)
        {
            if (token.IsCancellationRequested) return;

            var objects = GetMapObjectFromGeoJsonFeature(feature, feature.Geometry);
            mapObjects.AddRange(objects);
        }

        if (token.IsCancellationRequested) return;

        AddGeoJsonPins(mapObjects.OfType<Pin>());

        if (token.IsCancellationRequested) return;

        AddGeoJsonPolylines(mapObjects.OfType<Polyline>());

        if (token.IsCancellationRequested) return;

        AddGeoJsonPolygons(mapObjects.OfType<Polygon>());
    }

    protected virtual IEnumerable<VisualElement> GetMapObjectFromGeoJsonFeature(GeoJsonFeature feature, IGeometry geometry)
    {
        List<VisualElement> elements = [];
        switch (geometry)
        {
            case PointGeometry pointGeometry:
                {
                    Pin pin = new()
                    {
                        Position = pointGeometry.Position,
                        BindingContext = feature
                    };

                    ApplyGeoJsonPinStyle(pin, feature);

                    elements.Add(pin);
                }
                break;
            case LineString lineGeometry:
                {
                    Polyline polyline = new()
                    {
                        Points = new(lineGeometry.Points.ToArray()),
                        StrokeThickness = 1d,
                        BindingContext = feature
                    };

                    ApplyGeoJsonPolylineStyle(polyline, feature);

                    elements.Add(polyline);
                }
                break;
            case GeoJsonPolygon polygonGeometry:
                {
                    Polygon polygon = new()
                    {
                        Points = new(polygonGeometry.OuterBoundaryCoordinates.ToArray()),
                        StrokeThickness = 1d,
                        BindingContext = feature
                    };

                    if (polygonGeometry.InnerBoundaryCoordinates?.Count() > 0)
                    {
                        PolygonAttached.SetHoles(polygon, polygonGeometry.InnerBoundaryCoordinates);
                    }

                    ApplyGeoJsonPolygonStyle(polygon, feature);

                    elements.Add(polygon);
                }
                break;
            case MultiGeometry multiGeometry:
                {
                    foreach (var innerGeometry in multiGeometry.Geometries)
                    {
                        elements.AddRange(GetMapObjectFromGeoJsonFeature(feature, innerGeometry));
                    }
                }
                break;
            default:
                break;
        }

        return elements;
    }

    protected virtual void ApplyGeoJsonPinStyle(Pin pin, GeoJsonFeature feature)
    {
        var style = feature.PointStyle;

        if (style.Icon is string icon)
        {
            pin.Icon = icon;
        }

        if (style.IconColor is string color)
        {
            pin.DefaultIconColor = Color.FromArgb(color);
        }

        if (style.Rotation is double rotation)
        {
            pin.Rotation = rotation;
        }

        SetGeoJsonPinInfoWindow(pin, style);
    }

    protected virtual void SetGeoJsonPinInfoWindow(Pin pin, GeoJsonPointStyle style)
    {
        var text = style.Title;
        if (!string.IsNullOrWhiteSpace(style.Snippet))
        {
            text = $"<b>{text}</b><br>{style.Snippet}";
        }

        if (string.IsNullOrWhiteSpace(text)) return;

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
    }

    protected virtual void ApplyGeoJsonPolylineStyle(Polyline polyline, GeoJsonFeature feature)
    {
        var style = feature.LineStringStyle;

        if (style.Stroke is string stroke)
        {
            var color = Color.FromArgb(stroke);
            if (style.StrokeOpacity is double opacity)
            {
                color = color.WithAlpha((float)opacity);
            }
            polyline.Stroke = color;
        }

        if (style.StrokeWidth is double width)
        {
            polyline.StrokeThickness = width;
        }
    }

    protected virtual void ApplyGeoJsonPolygonStyle(Polygon polyline, GeoJsonFeature feature)
    {
        var style = feature.PolygonStyle;

        if (style.Stroke is string stroke)
        {
            var color = Color.FromArgb(stroke);
            if (style.StrokeOpacity is double opacity)
            {
                color = color.WithAlpha((float)opacity);
            }
            polyline.Stroke = color;
        }

        if (style.StrokeWidth is double width)
        {
            polyline.StrokeThickness = width;
        }

        if (style.Fill is string fill)
        {
            var color = Color.FromArgb(fill);
            if (style.FillOpacity is double opacity)
            {
                color = color.WithAlpha((float)opacity);
            }
            polyline.Fill = color;
        }
    }

    protected virtual void AddGeoJsonPins(IEnumerable<Pin> pins)
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

        GeoJsonPins = observablePins.ToList();
    }

    protected virtual void AddGeoJsonPolylines(IEnumerable<Polyline> polylines)
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

        GeoJsonPolylines = observablePolylines.ToList();
    }

    protected virtual void AddGeoJsonPolygons(IEnumerable<Polygon> polygons)
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

        GeoJsonPolygons = observablePolygons.ToList();
    }

    protected virtual void RemoveGeoJsonPins()
    {
        if (GeoJsonPins?.Count is null or 0
            || Pins is not ICollection<Pin> changableCollection
            || changableCollection.Count == 0) return;

        foreach (var pin in GeoJsonPins)
        {
            changableCollection.Remove(pin);
        }
    }

    protected virtual void RemoveGeoJsonPolylines()
    {
        if (GeoJsonPolylines?.Count is null or 0
            || Polylines is not ICollection<Polyline> changableCollection
            || changableCollection.Count == 0) return;

        foreach (var polyline in GeoJsonPolylines)
        {
            changableCollection.Remove(polyline);
        }
    }

    protected virtual void RemoveGeoJsonPolygons()
    {
        if (GeoJsonPolygons?.Count is null or 0
            || Polygons is not ICollection<Polygon> changableCollection
            || changableCollection.Count == 0) return;

        foreach (var polygon in GeoJsonPolygons)
        {
            changableCollection.Remove(polygon);
        }
    }

    #region GeoJson
    public string GeoJson
    {
        get => (string)GetValue(GeoJsonProperty);
        set => SetValue(GeoJsonProperty, value);
    }

    public static readonly BindableProperty GeoJsonProperty =
        BindableProperty.Create(
            nameof(GeoJson),
            typeof(string),
            typeof(GoogleMap));
    #endregion

    #region GeoJsonParsedCommand
    public ICommand GeoJsonParsedCommand
    {
        get => (ICommand)GetValue(GeoJsonParsedCommandProperty);
        set => SetValue(GeoJsonParsedCommandProperty, value);
    }

    public static readonly BindableProperty GeoJsonParsedCommandProperty =
        BindableProperty.Create(
            nameof(GeoJsonParsedCommand),
            typeof(ICommand),
            typeof(GoogleMap));
    #endregion
}