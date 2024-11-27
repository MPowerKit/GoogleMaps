using System.Collections.Specialized;

using Android.Content;
using Android.Gms.Maps.Model;

using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Platform;

using GMap = Android.Gms.Maps.GoogleMap;
using NPolygon = Android.Gms.Maps.Model.Polygon;
using VPolygon = Microsoft.Maui.Controls.Shapes.Polygon;

namespace MPowerKit.GoogleMaps;

public class PolygonManager : IMapFeatureManager<GoogleMap, GMap, GoogleMapHandler>
{
    protected GoogleMap? VirtualView { get; set; }
    protected GMap? NativeView { get; set; }
    protected GoogleMapHandler? Handler { get; set; }

    protected List<VPolygon> Polygons { get; set; } = [];

    public virtual void Connect(GoogleMap virtualView, GMap platformView, GoogleMapHandler handler)
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

        platformView.PolygonClick += NativeMap_PolygonClick;
    }

    public virtual void Disconnect(GoogleMap virtualView, GMap platformView, GoogleMapHandler handler)
    {
        platformView.PolygonClick -= NativeMap_PolygonClick;

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

    protected virtual void NativeMap_PolygonClick(object? sender, GMap.PolygonClickEventArgs e)
    {
        var polygon = Polygons.Single(p => (NativeObjectAttachedProperty.GetNativeObject(p) as NPolygon)!.Id == e.Polygon.Id);

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
        RemovePolygonsFromNativeMap([.. Polygons]);
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
            native.Clickable = polygon.IsEnabled;
        }
        else if (e.PropertyName == VPolygon.PointsProperty.PropertyName)
        {
            native.Points = polygon.Points.Select(p => p.ToLatLng()).ToList();
        }
        else if (e.PropertyName == Shape.StrokeDashArrayProperty.PropertyName)
        {
            if (polygon.StrokeDashPattern.Length != 0)
                native.StrokePattern = polygon.StrokeDashPattern.Select(v => Handler!.Context.ToPixels(v)).ToArray().ToPatternItems();
            else native.StrokePattern = null;
        }
        else if (e.PropertyName == Shape.IsVisibleProperty.PropertyName)
        {
            native.Visible = polygon.IsVisible;
        }
        else if (e.PropertyName == Shape.StrokeLineJoinProperty.PropertyName)
        {
            native.StrokeJointType = (int)polygon.StrokeLineJoin;
        }
        else if (e.PropertyName == Shape.ZIndexProperty.PropertyName)
        {
            native.ZIndex = polygon.ZIndex;
        }
        else if (e.PropertyName == Shape.StrokeThicknessProperty.PropertyName)
        {
            native.StrokeWidth = Handler!.Context.ToPixels(polygon.StrokeThickness);
        }
        else if (e.PropertyName == Shape.StrokeProperty.PropertyName)
        {
            native.StrokeColor = (polygon.Stroke as SolidColorBrush)?.Color.ToInt() ?? Android.Graphics.Color.Black.ToArgb();
        }
        else if (e.PropertyName == Shape.FillProperty.PropertyName)
        {
            native.FillColor = (polygon.Fill as SolidColorBrush)?.Color.ToInt() ?? Android.Graphics.Color.Black.ToArgb();
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
        var context = Platform.AppContext;

        foreach (var polygon in polygons)
        {
            NativeObjectAttachedProperty.SetNativeObject(polygon, NativeView!.AddPolygon(polygon.ToNative(context)));
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
            native?.Remove();

            Polygons.Remove(polygon);
        }
    }
}

public static class PolygonExtensions
{
    public static PolygonOptions ToNative(this VPolygon polygon, Context context)
    {
        var options = new PolygonOptions();

        foreach (var point in polygon.Points)
        {
            options.Add(point.ToLatLng());
        }

        options.Clickable(polygon.IsEnabled);

        options.InvokeFillColor((polygon.Stroke as SolidColorBrush)?.Color.ToInt() ?? Android.Graphics.Color.Black.ToArgb());
        options.InvokeStrokeColor((polygon.Fill as SolidColorBrush)?.Color.ToInt() ?? Android.Graphics.Color.Black.ToArgb());
        options.InvokeStrokeWidth(context.ToPixels(polygon.StrokeThickness));
        options.InvokeZIndex(polygon.ZIndex);
        options.Visible(polygon.IsVisible);

        if (polygon.StrokeDashPattern.Length != 0)
            options.InvokeStrokePattern(polygon.StrokeDashPattern.Select(v => context.ToPixels(v)).ToArray().ToPatternItems());

        options.InvokeStrokeJointType((int)polygon.StrokeLineJoin);

        return options;
    }
}