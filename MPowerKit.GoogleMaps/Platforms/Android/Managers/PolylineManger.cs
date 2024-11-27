using System.Collections.Specialized;

using Android.Content;
using Android.Gms.Maps.Model;

using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Platform;

using GMap = Android.Gms.Maps.GoogleMap;
using NPolyline = Android.Gms.Maps.Model.Polyline;
using VPolyline = Microsoft.Maui.Controls.Shapes.Polyline;

namespace MPowerKit.GoogleMaps;

public class PolylineManger : IMapFeatureManager<GoogleMap, GMap, GoogleMapHandler>
{
    protected GoogleMap? VirtualView { get; set; }
    protected GMap? NativeView { get; set; }
    protected GoogleMapHandler? Handler { get; set; }

    protected List<VPolyline> Polylines { get; set; } = [];

    public virtual void Connect(GoogleMap virtualView, GMap platformView, GoogleMapHandler handler)
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

        platformView.PolylineClick += NativeMap_PolylineClick;
    }

    public virtual void Disconnect(GoogleMap virtualView, GMap platformView, GoogleMapHandler handler)
    {
        platformView.PolylineClick -= NativeMap_PolylineClick;

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

    protected virtual void NativeMap_PolylineClick(object? sender, GMap.PolylineClickEventArgs e)
    {
        var polyline = Polylines.Single(p => (NativeObjectAttachedProperty.GetNativeObject(p) as NPolyline)!.Id == e.Polyline.Id);

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
        RemovePolylinesFromNativeMap([.. Polylines]);
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
            native.Clickable = polyline.IsEnabled;
        }
        else if (e.PropertyName == VPolyline.PointsProperty.PropertyName)
        {
            native.Points = polyline.Points.Select(p => p.ToLatLng()).ToList();
        }
        else if (e.PropertyName == Shape.StrokeDashArrayProperty.PropertyName)
        {
            if (polyline.StrokeDashPattern.Length != 0)
                native.Pattern = polyline.StrokeDashPattern.Select(v => Handler!.Context.ToPixels(v)).ToArray().ToPatternItems();
            else native.Pattern = null;
        }
        else if (e.PropertyName == Shape.IsVisibleProperty.PropertyName)
        {
            native.Visible = polyline.IsVisible;
        }
        else if (e.PropertyName == Shape.StrokeLineCapProperty.PropertyName)
        {
            var cap = polyline.StrokeLineCap.ToCap();

            native.StartCap = cap;
            native.EndCap = cap;
        }
        else if (e.PropertyName == Shape.StrokeLineJoinProperty.PropertyName)
        {
            native.JointType = (int)polyline.StrokeLineJoin;
        }
        else if (e.PropertyName == Shape.ZIndexProperty.PropertyName)
        {
            native.ZIndex = polyline.ZIndex;
        }
        else if (e.PropertyName == Shape.StrokeThicknessProperty.PropertyName)
        {
            native.Width = Handler!.Context.ToPixels(polyline.StrokeThickness);
        }
        else if (e.PropertyName == Shape.StrokeProperty.PropertyName)
        {
            native.Spans = [polyline.Stroke.ToSpan()];
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
        var context = Platform.AppContext;

        foreach (var polyline in polylines)
        {
            NativeObjectAttachedProperty.SetNativeObject(polyline, NativeView!.AddPolyline(polyline.ToNative(context)));
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
            native?.Remove();

            Polylines.Remove(polyline);
        }
    }
}

public static class PolylineExtensions
{
    public static PolylineOptions ToNative(this VPolyline polyline, Context context)
    {
        var options = new PolylineOptions();

        foreach (var point in polyline.Points)
        {
            options.Add(point.ToLatLng());
        }

        options.Clickable(polyline.IsEnabled);

        options.AddSpan(polyline.Stroke.ToSpan());
        options.InvokeWidth(context.ToPixels(polyline.StrokeThickness));
        options.InvokeZIndex(polyline.ZIndex);
        var cap = polyline.StrokeLineCap.ToCap();
        options.InvokeStartCap(cap);
        options.InvokeEndCap(cap);
        options.InvokeJointType((int)polyline.StrokeLineJoin);

        if (polyline.StrokeDashPattern.Length != 0)
            options.InvokePattern(polyline.StrokeDashPattern.Select(v => context.ToPixels(v)).ToArray().ToPatternItems());

        options.Visible(polyline.IsVisible);

        return options;
    }

    public static StyleSpan ToSpan(this Brush? brush)
    {
        return brush switch
        {
            SolidColorBrush solidBrush => new(solidBrush.Color.ToInt()),
            LinearGradientBrush gradientBrush => gradientBrush.GradientStops.Count switch
            {
                0 => new(Android.Graphics.Color.Black.ToArgb()),
                1 => new(gradientBrush.GradientStops[0].Color.ToInt()),
                _ => new(
                    StrokeStyle.GradientBuilder(
                        gradientBrush.GradientStops[0].Color.ToInt(),
                        gradientBrush.GradientStops[1].Color.ToInt())
                    .Build())
            },
            _ => new(Android.Graphics.Color.Black.ToArgb())
        };
    }

    public static Cap ToCap(this PenLineCap cap)
    {
        return cap switch
        {
            PenLineCap.Flat => new ButtCap(),
            PenLineCap.Round => new RoundCap(),
            PenLineCap.Square => new SquareCap(),
            _ => throw new ArgumentOutOfRangeException(nameof(cap), cap, null)
        };
    }
}