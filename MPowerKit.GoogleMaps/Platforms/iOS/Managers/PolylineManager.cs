using System.Collections.Specialized;

using Foundation;

using Google.Maps;

using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Platform;

using UIKit;

using NPolyline = Google.Maps.Polyline;
using VPolyline = Microsoft.Maui.Controls.Shapes.Polyline;

namespace MPowerKit.GoogleMaps;

public class PolylineManager : IMapFeatureManager<GoogleMap, MapView, GoogleMapHandler>
{
    protected GoogleMap? VirtualView { get; set; }
    protected MapView? NativeView { get; set; }
    protected GoogleMapHandler? Handler { get; set; }

    protected List<VPolyline> Polylines { get; set; } = [];

    public virtual void Connect(GoogleMap virtualView, MapView platformView, GoogleMapHandler handler)
    {
        VirtualView = virtualView;
        NativeView = platformView;
        Handler = handler;

        ResetPolylines();

        virtualView.PropertyChanged += VirtualView_PropertyChanged;
        virtualView.PropertyChanging += VirtualView_PropertyChanging;

        if (virtualView.Polylines is INotifyCollectionChanged polyLines)
        {
            polyLines.CollectionChanged += PolyLines_CollectionChanged;
        }

        platformView.OverlayTapped += NativeMap_OverlayTapped;
    }

    public virtual void Disconnect(GoogleMap virtualView, MapView platformView, GoogleMapHandler handler)
    {
        platformView.OverlayTapped -= NativeMap_OverlayTapped;

        virtualView.PropertyChanged -= VirtualView_PropertyChanged;
        virtualView.PropertyChanging -= VirtualView_PropertyChanging;

        if (virtualView.Polylines is INotifyCollectionChanged polyLines)
        {
            polyLines.CollectionChanged -= PolyLines_CollectionChanged;
        }

        ClearPolylines();

        VirtualView = null;
        NativeView = null;
        Handler = null;
    }

    private void NativeMap_OverlayTapped(object? sender, GMSOverlayEventEventArgs e)
    {
        var polyline = Polylines.SingleOrDefault(p => NativeObjectAttachedProperty.GetNativeObject(p) == e.Overlay);

        if (polyline is null) return;

        VirtualView!.SendPolylineClick(polyline);
    }

    protected virtual void VirtualView_PropertyChanging(object sender, PropertyChangingEventArgs e)
    {
        if (e.PropertyName == GoogleMap.PolylinesProperty.PropertyName)
        {
            if (VirtualView!.Polylines is INotifyCollectionChanged polyLines)
            {
                polyLines.CollectionChanged -= PolyLines_CollectionChanged;
            }

            ClearPolylines();
        }
    }

    protected virtual void VirtualView_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == GoogleMap.PolylinesProperty.PropertyName)
        {
            InitPolylines();

            if (VirtualView!.Polylines is INotifyCollectionChanged polyLines)
            {
                polyLines.CollectionChanged += PolyLines_CollectionChanged;
            }
        }
    }

    protected virtual void PolyLines_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                AddPolylines(e);
                break;
            case NotifyCollectionChangedAction.Remove:
                RemovePolylines(e);
                break;
            case NotifyCollectionChangedAction.Replace:
                ReplacePolylines(e);
                break;
            case NotifyCollectionChangedAction.Move:
                break;
            case NotifyCollectionChangedAction.Reset:
            default:
                ResetPolylines();
                break;
        }
    }

    protected virtual void ResetPolylines()
    {
        ClearPolylines();

        InitPolylines();
    }

    protected virtual void ClearPolylines()
    {
        RemovePolylinesFromNativeMap([..Polylines]);
    }

    protected virtual void InitPolylines()
    {
        if (VirtualView!.Polylines?.Count() is null or 0) return;

        var polylines = VirtualView!.Polylines.ToList();

        Polylines = new(polylines.Count);

        AddPolylinesToNativeMap(polylines);
    }

    protected virtual void Polyline_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        var polyline = (sender as VPolyline)!;

        if (NativeObjectAttachedProperty.GetNativeObject(polyline) is not NPolyline native) return;

        if (e.PropertyName == Shape.IsEnabledProperty.PropertyName)
        {
            native.Tappable = polyline.IsEnabled;
        }
        else if (e.PropertyName == VPolyline.PointsProperty.PropertyName)
        {
            native.Path = polyline.Points.ToPath();
        }
        else if (e.PropertyName == Shape.ZIndexProperty.PropertyName)
        {
            native.ZIndex = polyline.ZIndex;
        }
        else if (e.PropertyName == Shape.StrokeThicknessProperty.PropertyName)
        {
            native.StrokeWidth = (float)polyline.StrokeThickness;
        }
        else if(e.PropertyName == Shape.IsVisibleProperty.PropertyName)
        {
            native.Map = polyline.IsVisible ? NativeView! : null;
        }

        if (e.PropertyName == Shape.StrokeDashArrayProperty.PropertyName
            || e.PropertyName == Shape.StrokeProperty.PropertyName
            || e.PropertyName == VPolyline.PointsProperty.PropertyName)
        {
            native.Spans = polyline.ToSpans();
        }
    }

    protected virtual void AddPolylines(NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems?.Count is null or 0 || NativeView is null) return;

        AddPolylinesToNativeMap(e.NewItems.Cast<VPolyline>());
    }

    protected virtual void RemovePolylines(NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems?.Count is null or 0 || NativeView is null) return;

        RemovePolylinesFromNativeMap(e.OldItems.Cast<VPolyline>());
    }

    protected virtual void ReplacePolylines(NotifyCollectionChangedEventArgs e)
    {
        RemovePolylines(e);
        AddPolylines(e);
    }

    protected virtual void AddPolylinesToNativeMap(IEnumerable<VPolyline> polylines)
    {
        foreach (var polyline in polylines)
        {
            NativeObjectAttachedProperty.SetNativeObject(polyline, polyline.ToNative(NativeView!));
            polyline.PropertyChanged += Polyline_PropertyChanged;
            Polylines.Add(polyline);
        }
    }

    protected virtual void RemovePolylinesFromNativeMap(IEnumerable<VPolyline> polylines)
    {
        foreach (var polyline in polylines)
        {
            polyline.PropertyChanged -= Polyline_PropertyChanged;

            var native = NativeObjectAttachedProperty.GetNativeObject(polyline) as NPolyline;
            if (native is not null)
            {
                native.Map = null;
            }

            Polylines.Remove(polyline);
        }
    }
}

public static class PolylineExtensions
{
    public static NPolyline ToNative(this VPolyline polyline, MapView map)
    {
        var path = polyline.Points.ToPath();
        var native = NPolyline.FromPath(path);
        native.ZIndex = polyline.ZIndex;
        native.StrokeWidth = (float)polyline.StrokeThickness;
        native.Tappable = polyline.IsEnabled;
        native.Spans = polyline.ToSpans();

        if (polyline.IsVisible)
        {
            native.Map = map;
        }

        return native;
    }

    public static StyleSpan[] ToSpans(this VPolyline polyline)
    {
        var path = polyline.Points.ToPath();

        StyleSpan[] spans;
        if (polyline.StrokeDashPattern.Length != 0)
        {
            var colorStyle = polyline.Stroke.ToStrokeStyle();
            var pattern = polyline.StrokeDashPattern;
            List<StrokeStyle> styles = [];
            for (int i = 0; i < pattern.Length; i++)
            {
                var value = pattern[i];

                styles.Add(i % 2 == 0
                    ? colorStyle
                    : StrokeStyle.GetSolidColor(UIColor.Clear));
            }

            spans = GeometryUtils.StyleSpans(path, styles.ToArray(), pattern.Select(v => new NSNumber(v)).ToArray(), LengthKind.Rhumb);
        }
        else
        {
            spans = [polyline.Stroke.ToSpan()];
        }

        return spans;
    }

    public static StrokeStyle ToStrokeStyle(this Brush? brush)
    {
        return brush switch
        {
            SolidColorBrush solidBrush => StrokeStyle.GetSolidColor(solidBrush.Color.ToPlatform()),
            LinearGradientBrush gradientBrush => gradientBrush.GradientStops.Count switch
            {
                0 => StrokeStyle.GetSolidColor(UIColor.Black),
                1 => StrokeStyle.GetSolidColor(gradientBrush.GradientStops[0].Color.ToPlatform()),
                _ => StrokeStyle.GetGradient(gradientBrush.GradientStops[0].Color.ToPlatform(),
                    gradientBrush.GradientStops[1].Color.ToPlatform())
            },
            _ => StrokeStyle.GetSolidColor(UIColor.Black)
        };
    }

    public static StyleSpan ToSpan(this Brush? brush)
    {
        return StyleSpan.FromStyle(brush.ToStrokeStyle());
    }
}