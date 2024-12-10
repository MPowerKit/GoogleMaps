using System.Collections.Specialized;

using Google.Maps;

using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Platform;

using UIKit;

using NCircle = Google.Maps.Circle;
using VCircle = MPowerKit.GoogleMaps.Circle;

namespace MPowerKit.GoogleMaps;

public class CircleManager : IMapFeatureManager<GoogleMap, MapView, GoogleMapHandler>
{
    protected GoogleMap? VirtualView { get; set; }
    protected MapView? NativeView { get; set; }
    protected GoogleMapHandler? Handler { get; set; }

    protected List<VCircle> Circles { get; set; } = [];

    public virtual void Connect(GoogleMap virtualView, MapView platformView, GoogleMapHandler handler)
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

        platformView.OverlayTapped += NativeMap_OverlayTapped;
    }

    public virtual void Disconnect(GoogleMap virtualView, MapView platformView, GoogleMapHandler handler)
    {
        platformView.OverlayTapped -= NativeMap_OverlayTapped;

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

    protected virtual void NativeMap_OverlayTapped(object? sender, GMSOverlayEventEventArgs e)
    {
        var circle = Circles.SingleOrDefault(c => NativeObjectAttachedProperty.GetNativeObject(c) == e.Overlay);

        if (circle is null) return;

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
        RemoveCirclesFromNativeMap([..Circles]);
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
            native.Tappable = circle.IsEnabled;
        }
        else if (e.PropertyName == VCircle.RadiusProperty.PropertyName)
        {
            native.Radius = circle.Radius;
        }
        else if (e.PropertyName == VCircle.CenterProperty.PropertyName)
        {
            native.Position = circle.Center.ToCoord();
        }
        else if (e.PropertyName == Shape.IsVisibleProperty.PropertyName)
        {
            native.Map = circle.IsVisible ? NativeView! : null;
        }
        else if (e.PropertyName == Shape.ZIndexProperty.PropertyName)
        {
            native.ZIndex = circle.ZIndex;
        }
        else if (e.PropertyName == Shape.StrokeThicknessProperty.PropertyName)
        {
            native.StrokeWidth = (float)circle.StrokeThickness;
        }
        else if (e.PropertyName == Shape.StrokeProperty.PropertyName)
        {
            native.StrokeColor = (circle.Stroke as SolidColorBrush)?.Color.ToPlatform() ?? UIColor.Black;
        }
        else if (e.PropertyName == Shape.FillProperty.PropertyName)
        {
            native.FillColor = (circle.Fill as SolidColorBrush)?.Color.ToPlatform() ?? UIColor.Black;
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
        foreach (var circle in circles)
        {
            NativeObjectAttachedProperty.SetNativeObject(circle, circle.ToNative(NativeView!));
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

            if (native is not null)
            {
                native.Map = null;
            }

            Circles.Remove(circle);
        }
    }
}

public static class CircleExtensions
{
    public static NCircle ToNative(this VCircle circle, MapView map)
    {
        var native = NCircle.FromPosition(circle.Center.ToCoord(), circle.Radius);
        native.Tappable = circle.IsEnabled;
        native.StrokeWidth = (float)circle.StrokeThickness;
        native.StrokeColor = (circle.Stroke as SolidColorBrush)?.Color.ToPlatform() ?? UIColor.Black;
        native.FillColor = (circle.Fill as SolidColorBrush)?.Color.ToPlatform() ?? UIColor.Black;
        native.ZIndex = circle.ZIndex;

        if (circle.IsVisible)
        {
            native.Map = map;
        }

        return native;
    }
}