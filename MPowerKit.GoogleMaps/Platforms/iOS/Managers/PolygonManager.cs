using System.Collections.Specialized;

using Google.Maps;

using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Platform;

using UIKit;

using NPolygon = Google.Maps.Polygon;
using VPolygon = Microsoft.Maui.Controls.Shapes.Polygon;

namespace MPowerKit.GoogleMaps;

public class PolygonManager : IMapFeatureManager<GoogleMap, MapView, GoogleMapHandler>
{
    protected GoogleMap? VirtualView { get; set; }
    protected MapView? NativeView { get; set; }
    protected GoogleMapHandler? Handler { get; set; }

    protected List<VPolygon> Polygons { get; set; } = [];

    public virtual void Connect(GoogleMap virtualView, MapView platformView, GoogleMapHandler handler)
    {
        VirtualView = virtualView;
        NativeView = platformView;
        Handler = handler;

        ResetPolygons();

        virtualView.PropertyChanged += VirtualView_PropertyChanged;
        virtualView.PropertyChanging += VirtualView_PropertyChanging;

        if (virtualView.Polygons is INotifyCollectionChanged polygons)
        {
            polygons.CollectionChanged += Polygons_CollectionChanged;
        }

        platformView.OverlayTapped += NativeMap_OverlayTapped;
    }

    public virtual void Disconnect(GoogleMap virtualView, MapView platformView, GoogleMapHandler handler)
    {
        platformView.OverlayTapped -= NativeMap_OverlayTapped;

        virtualView.PropertyChanged -= VirtualView_PropertyChanged;
        virtualView.PropertyChanging -= VirtualView_PropertyChanging;

        if (virtualView.Polygons is INotifyCollectionChanged polygons)
        {
            polygons.CollectionChanged -= Polygons_CollectionChanged;
        }

        ClearPolygons();

        VirtualView = null;
        NativeView = null;
        Handler = null;
    }

    protected virtual void NativeMap_OverlayTapped(object? sender, GMSOverlayEventEventArgs e)
    {
        var polygon = Polygons.SingleOrDefault(p => NativeObjectAttachedProperty.GetNativeObject(p) == e.Overlay);

        if (polygon is null) return;

        VirtualView!.SendPolygonClick(polygon);
    }

    protected virtual void VirtualView_PropertyChanging(object sender, PropertyChangingEventArgs e)
    {
        if (e.PropertyName == GoogleMap.PolygonsProperty.PropertyName)
        {
            if (VirtualView!.Polygons is INotifyCollectionChanged polygons)
            {
                polygons.CollectionChanged -= Polygons_CollectionChanged;
            }

            ClearPolygons();
        }
    }

    protected virtual void VirtualView_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == GoogleMap.PolygonsProperty.PropertyName)
        {
            InitPolygons();

            if (VirtualView!.Polygons is INotifyCollectionChanged polygons)
            {
                polygons.CollectionChanged += Polygons_CollectionChanged;
            }
        }
    }

    protected virtual void Polygons_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                AddPolygons(e);
                break;
            case NotifyCollectionChangedAction.Remove:
                RemovePolygons(e);
                break;
            case NotifyCollectionChangedAction.Replace:
                ReplacePolygons(e);
                break;
            case NotifyCollectionChangedAction.Move:
                break;
            case NotifyCollectionChangedAction.Reset:
            default:
                ResetPolygons();
                break;
        }
    }

    protected virtual void ResetPolygons()
    {
        ClearPolygons();

        InitPolygons();
    }

    protected virtual void ClearPolygons()
    {
        RemovePolygonsFromNativeMap([..Polygons]);
    }

    protected virtual void InitPolygons()
    {
        if (VirtualView!.Polygons?.Count() is null or 0) return;

        var polygons = VirtualView!.Polygons.ToList();

        Polygons = new(polygons.Count);

        AddPolygonsToNativeMap(polygons);
    }

    protected virtual void Polygon_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        var polygon = (sender as VPolygon)!;

        if (NativeObjectAttachedProperty.GetNativeObject(polygon) is not NPolygon native) return;

        if (e.PropertyName == Shape.IsEnabledProperty.PropertyName)
        {
            native.Tappable = polygon.IsEnabled;
        }
        else if (e.PropertyName == VPolygon.PointsProperty.PropertyName)
        {
            native.Path = polygon.Points.ToPath();
        }
        else if (e.PropertyName == Shape.ZIndexProperty.PropertyName)
        {
            native.ZIndex = polygon.ZIndex;
        }
        else if (e.PropertyName == Shape.StrokeThicknessProperty.PropertyName)
        {
            native.StrokeWidth = (float)polygon.StrokeThickness;
        }
        else if (e.PropertyName == Shape.StrokeProperty.PropertyName)
        {
            native.StrokeColor = (polygon.Stroke as SolidColorBrush)?.Color.ToPlatform() ?? UIColor.Black;
        }
        else if (e.PropertyName == Shape.FillProperty.PropertyName)
        {
            native.FillColor = (polygon.Fill as SolidColorBrush)?.Color.ToPlatform() ?? UIColor.Black;
        }
        else if (e.PropertyName == Shape.IsVisibleProperty.PropertyName)
        {
            native.Map = polygon.IsVisible ? NativeView! : null;
        }
    }

    protected virtual void AddPolygons(NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems?.Count is null or 0 || NativeView is null) return;

        AddPolygonsToNativeMap(e.NewItems.Cast<VPolygon>());
    }

    protected virtual void RemovePolygons(NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems?.Count is null or 0 || NativeView is null) return;

        RemovePolygonsFromNativeMap(e.OldItems.Cast<VPolygon>());
    }

    protected virtual void ReplacePolygons(NotifyCollectionChangedEventArgs e)
    {
        RemovePolygons(e);
        AddPolygons(e);
    }

    protected virtual void AddPolygonsToNativeMap(IEnumerable<VPolygon> polygons)
    {
        foreach (var polygon in polygons)
        {
            NativeObjectAttachedProperty.SetNativeObject(polygon, polygon.ToNative(NativeView!));
            polygon.PropertyChanged += Polygon_PropertyChanged;
            Polygons.Add(polygon);
        }
    }

    protected virtual void RemovePolygonsFromNativeMap(IEnumerable<VPolygon> polygons)
    {
        foreach (var polygon in polygons)
        {
            polygon.PropertyChanged -= Polygon_PropertyChanged;

            var native = NativeObjectAttachedProperty.GetNativeObject(polygon) as NPolygon;

            if (native is not null)
            {
                native.Map = null;
            }

            Polygons.Remove(polygon);
        }
    }
}

public static class PolygonExtensions
{
    public static NPolygon ToNative(this VPolygon polygon, MapView map)
    {
        var path = polygon.Points.ToPath();
        var native = NPolygon.FromPath(path);
        native.ZIndex = polygon.ZIndex;
        native.StrokeWidth = (float)polygon.StrokeThickness;
        native.Tappable = polygon.IsEnabled;
        native.StrokeColor = (polygon.Stroke as SolidColorBrush)?.Color.ToPlatform() ?? UIColor.Black;
        native.FillColor = (polygon.Fill as SolidColorBrush)?.Color.ToPlatform() ?? UIColor.Black;
        
        if (polygon.IsVisible)
        {
            native.Map = map;
        }

        return native;
    }
}