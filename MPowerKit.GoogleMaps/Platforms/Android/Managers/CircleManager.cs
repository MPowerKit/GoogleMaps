using System.Collections.Specialized;

using Android.Content;
using Android.Gms.Maps.Model;

using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Platform;

using GMap = Android.Gms.Maps.GoogleMap;
using NCircle = Android.Gms.Maps.Model.Circle;
using VCircle = MPowerKit.GoogleMaps.Circle;

namespace MPowerKit.GoogleMaps;

public class CircleManager : IMapFeatureManager<GoogleMap, GMap, GoogleMapHandler>
{
    protected GoogleMap? VirtualView { get; set; }
    protected GMap? NativeView { get; set; }
    protected GoogleMapHandler? Handler { get; set; }

    protected List<VCircle> Circles { get; set; } = [];

    public virtual void Connect(GoogleMap virtualView, GMap platformView, GoogleMapHandler handler)
    {
        VirtualView = virtualView;
        NativeView = platformView;
        Handler = handler;

        ResetCircles();

        virtualView.PropertyChanged += VirtualView_PropertyChanged;
        virtualView.PropertyChanging += VirtualView_PropertyChanging;

        if (virtualView.Circles is INotifyCollectionChanged circles)
        {
            circles.CollectionChanged += Circles_CollectionChanged;
        }

        platformView.CircleClick += NativeMap_CircleClick;
    }

    public virtual void Disconnect(GoogleMap virtualView, GMap platformView, GoogleMapHandler handler)
    {
        platformView.CircleClick -= NativeMap_CircleClick;

        virtualView.PropertyChanged -= VirtualView_PropertyChanged;
        virtualView.PropertyChanging -= VirtualView_PropertyChanging;

        if (virtualView.Circles is INotifyCollectionChanged circles)
        {
            circles.CollectionChanged -= Circles_CollectionChanged;
        }

        ClearCircles();

        VirtualView = null;
        NativeView = null;
        Handler = null;
    }

    protected virtual void NativeMap_CircleClick(object? sender, GMap.CircleClickEventArgs e)
    {
        var circle = Circles.Single(c => (NativeObjectAttachedProperty.GetNativeObject(c) as NCircle)!.Id == e.Circle.Id);

        VirtualView!.SendCircleClick(circle);
    }

    protected virtual void VirtualView_PropertyChanging(object sender, PropertyChangingEventArgs e)
    {
        if (e.PropertyName == GoogleMap.CirclesProperty.PropertyName)
        {
            if (VirtualView!.Circles is INotifyCollectionChanged circles)
            {
                circles.CollectionChanged -= Circles_CollectionChanged;
            }

            ClearCircles();
        }
    }

    protected virtual void VirtualView_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == GoogleMap.CirclesProperty.PropertyName)
        {
            InitCircles();

            if (VirtualView!.Circles is INotifyCollectionChanged circles)
            {
                circles.CollectionChanged += Circles_CollectionChanged;
            }
        }
    }

    protected virtual void Circles_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                AddCircles(e);
                break;
            case NotifyCollectionChangedAction.Remove:
                RemoveCircles(e);
                break;
            case NotifyCollectionChangedAction.Replace:
                ReplaceCircles(e);
                break;
            case NotifyCollectionChangedAction.Move:
                break;
            case NotifyCollectionChangedAction.Reset:
            default:
                ResetCircles();
                break;
        }
    }

    protected virtual void ResetCircles()
    {
        ClearCircles();

        InitCircles();
    }

    protected virtual void ClearCircles()
    {
        RemoveCirclesFromNativeMap([.. Circles]);
    }

    protected virtual void InitCircles()
    {
        if (VirtualView!.Circles?.Count() is null or 0) return;

        var circles = VirtualView!.Circles.ToList();

        Circles = new(circles.Count);

        AddCirclesToNativeMap(circles);
    }

    protected virtual void Circle_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        var circle = (sender as VCircle)!;

        if (NativeObjectAttachedProperty.GetNativeObject(circle) is not NCircle native) return;

        if (e.PropertyName == Shape.IsEnabledProperty.PropertyName)
        {
            native.Clickable = circle.IsEnabled;
        }
        else if (e.PropertyName == VCircle.RadiusProperty.PropertyName)
        {
            native.Radius = circle.Radius;
        }
        else if (e.PropertyName == VCircle.CenterProperty.PropertyName)
        {
            native.Center = circle.Center.ToLatLng();
        }
        else if (e.PropertyName == Shape.StrokeDashArrayProperty.PropertyName)
        {
            if (circle.StrokeDashPattern.Length != 0)
                native.StrokePattern = circle.StrokeDashPattern.Select(v => Handler!.Context.ToPixels(v)).ToArray().ToPatternItems();
            else native.StrokePattern = null;
        }
        else if (e.PropertyName == Shape.IsVisibleProperty.PropertyName)
        {
            native.Visible = circle.IsVisible;
        }
        else if (e.PropertyName == Shape.ZIndexProperty.PropertyName)
        {
            native.ZIndex = circle.ZIndex;
        }
        else if (e.PropertyName == Shape.StrokeThicknessProperty.PropertyName)
        {
            native.StrokeWidth = Handler!.Context.ToPixels(circle.StrokeThickness);
        }
        else if (e.PropertyName == Shape.StrokeProperty.PropertyName)
        {
            native.StrokeColor = (circle.Stroke as SolidColorBrush)?.Color.ToInt() ?? Android.Graphics.Color.Black.ToArgb();
        }
        else if (e.PropertyName == Shape.FillProperty.PropertyName)
        {
            native.FillColor = (circle.Fill as SolidColorBrush)?.Color.ToInt() ?? Android.Graphics.Color.Black.ToArgb();
        }
    }

    protected virtual void AddCircles(NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems?.Count is null or 0 || NativeView is null) return;

        AddCirclesToNativeMap(e.NewItems.Cast<VCircle>());
    }

    protected virtual void RemoveCircles(NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems?.Count is null or 0 || NativeView is null) return;

        RemoveCirclesFromNativeMap(e.OldItems.Cast<VCircle>());
    }

    protected virtual void ReplaceCircles(NotifyCollectionChangedEventArgs e)
    {
        RemoveCircles(e);
        AddCircles(e);
    }

    protected virtual void AddCirclesToNativeMap(IEnumerable<VCircle> circles)
    {
        var context = Platform.AppContext;

        foreach (var circle in circles)
        {
            NativeObjectAttachedProperty.SetNativeObject(circle, NativeView!.AddCircle(circle.ToNative(context)));
            circle.PropertyChanged += Circle_PropertyChanged;
            Circles.Add(circle);
        }
    }

    protected virtual void RemoveCirclesFromNativeMap(IEnumerable<VCircle> circles)
    {
        foreach (var circle in circles)
        {
            circle.PropertyChanged -= Circle_PropertyChanged;

            var native = NativeObjectAttachedProperty.GetNativeObject(circle) as NCircle;
            native?.Remove();

            Circles.Remove(circle);
        }
    }
}

public static class CircleExtensions
{
    public static CircleOptions ToNative(this VCircle circle, Context context)
    {
        var options = new CircleOptions();

        options.InvokeCenter(circle.Center.ToLatLng());
        options.InvokeRadius(circle.Radius);

        options.Clickable(circle.IsEnabled);

        options.InvokeFillColor((circle.Stroke as SolidColorBrush)?.Color.ToInt() ?? Android.Graphics.Color.Black.ToArgb());
        options.InvokeStrokeColor((circle.Fill as SolidColorBrush)?.Color.ToInt() ?? Android.Graphics.Color.Black.ToArgb());
        options.InvokeStrokeWidth(context.ToPixels(circle.StrokeThickness));
        options.InvokeZIndex(circle.ZIndex);
        options.Visible(circle.IsVisible);

        if (circle.StrokeDashPattern.Length != 0)
            options.InvokeStrokePattern(circle.StrokeDashPattern.Select(v => context.ToPixels(v)).ToArray().ToPatternItems());

        return options;
    }
}