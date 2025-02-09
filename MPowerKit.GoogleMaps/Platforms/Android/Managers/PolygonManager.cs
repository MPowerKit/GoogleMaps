using Android.Content;
using Android.Gms.Maps.Model;
using Android.Runtime;

using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Platform;

using GMap = Android.Gms.Maps.GoogleMap;
using NPolygon = Android.Gms.Maps.Model.Polygon;
using VPolygon = Microsoft.Maui.Controls.Shapes.Polygon;

namespace MPowerKit.GoogleMaps;

public class PolygonManager : ItemsMapFeatureManager<VPolygon, NPolygon, GMap>
{
    protected override IEnumerable<VPolygon> VirtualViewItems => VirtualView!.Polygons;
    protected override string VirtualViewItemsPropertyName => GoogleMap.PolygonsProperty.PropertyName;

    protected override void SubscribeToEvents(GoogleMap virtualView, GMap platformView, GoogleMapHandler handler)
    {
        base.SubscribeToEvents(virtualView, platformView, handler);

        platformView.PolygonClick += PlatformView_PolygonClick;
    }

    protected override void UnsubscribeFromEvents(GoogleMap virtualView, GMap platformView, GoogleMapHandler handler)
    {
        platformView.PolygonClick -= PlatformView_PolygonClick;

        base.UnsubscribeFromEvents(virtualView, platformView, handler);
    }

    protected override void RemoveItemFromPlatformView(NPolygon? nItem)
    {
        nItem?.Remove();
    }

    protected override NPolygon AddItemToPlatformView(VPolygon vItem)
    {
        return PlatformView!.AddPolygon(vItem.ToNative(Handler!.Context));
    }

    protected override void ItemPropertyChanged(VPolygon vItem, NPolygon nItem, string? propertyName)
    {
        base.ItemPropertyChanged(vItem, nItem, propertyName);

        if (propertyName == Shape.IsEnabledProperty.PropertyName)
        {
            OnIsEnabledChanged(vItem, nItem);
        }
        else if (propertyName == VPolygon.PointsProperty.PropertyName)
        {
            OnPointsChanged(vItem, nItem);
        }
        else if (propertyName == Shape.StrokeLineJoinProperty.PropertyName)
        {
            OnStrokeLineJoinChanged(vItem, nItem);
        }
        else if (propertyName == Shape.StrokeDashArrayProperty.PropertyName)
        {
            OnStrokeDashArrayChanged(vItem, nItem);
        }
        else if (propertyName == Shape.IsVisibleProperty.PropertyName)
        {
            OnIsVisibleChanged(vItem, nItem);
        }
        else if (propertyName == Shape.ZIndexProperty.PropertyName)
        {
            OnZIndexChanged(vItem, nItem);
        }
        else if (propertyName == Shape.StrokeThicknessProperty.PropertyName)
        {
            OnStrokeThicknessChanged(vItem, nItem);
        }
        else if (propertyName == Shape.StrokeProperty.PropertyName)
        {
            OnStrokeChanged(vItem, nItem);
        }
        else if (propertyName == Shape.FillProperty.PropertyName)
        {
            OnFillChanged(vItem, nItem);
        }
        else if (propertyName == PolygonAttached.HolesProperty.PropertyName)
        {
            OnHolesChanged(vItem, nItem);
        }
    }

    protected virtual void OnIsEnabledChanged(VPolygon vPolygon, NPolygon nPolygon)
    {
        nPolygon.Clickable = vPolygon.IsEnabled;
    }

    protected virtual void OnPointsChanged(VPolygon vPolygon, NPolygon nPolygon)
    {
        nPolygon.Points = vPolygon.Points.Select(p => p.ToLatLng()).ToList();
    }

    protected virtual void OnStrokeLineJoinChanged(VPolygon vPolygon, NPolygon nPolygon)
    {
        nPolygon.StrokeJointType = (int)vPolygon.StrokeLineJoin;
    }

    protected virtual void OnStrokeDashArrayChanged(VPolygon vPolygon, NPolygon nPolygon)
    {
        nPolygon.StrokePattern = vPolygon.StrokeDashPattern.Length > 0
            ? vPolygon.StrokeDashPattern.Select(v => Handler!.Context.ToPixels(v)).ToArray().ToPatternItems()
            : null;
    }

    protected virtual void OnIsVisibleChanged(VPolygon vPolygon, NPolygon nPolygon)
    {
        nPolygon.Visible = vPolygon.IsVisible;
    }

    protected virtual void OnZIndexChanged(VPolygon vPolygon, NPolygon nPolygon)
    {
        nPolygon.ZIndex = vPolygon.ZIndex;
    }

    protected virtual void OnStrokeThicknessChanged(VPolygon vPolygon, NPolygon nPolygon)
    {
        nPolygon.StrokeWidth = Handler!.Context.ToPixels(vPolygon.StrokeThickness);
    }

    protected virtual void OnStrokeChanged(VPolygon vPolygon, NPolygon nPolygon)
    {
        nPolygon.StrokeColor = (vPolygon.Stroke as SolidColorBrush)?.Color.ToInt()
            ?? Android.Graphics.Color.Blue.ToArgb();
    }

    protected virtual void OnFillChanged(VPolygon vPolygon, NPolygon nPolygon)
    {
        nPolygon.FillColor = (vPolygon.Fill as SolidColorBrush)?.Color.ToInt()
            ?? Colors.Blue.WithAlpha(0.4f).ToInt();
    }

    protected virtual void OnHolesChanged(VPolygon vPolygon, NPolygon nPolygon)
    {
        var holes = PolygonAttached.GetHoles(vPolygon);
        nPolygon.SetHoles(holes?.Select(h => (h?.Select(p => p.ToLatLng()).ToJavaList() ?? []) as IList<LatLng>).ToJavaList() ?? []);
    }

    protected virtual void PlatformView_PolygonClick(object? sender, GMap.PolygonClickEventArgs e)
    {
        var polygon = Items.SingleOrDefault(p => (NativeObjectAttachedProperty.GetNativeObject(p) as NPolygon)!.Id == e.Polygon.Id);
        if (polygon is null) return;

        VirtualView!.SendPolygonClick(polygon);
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

        options.InvokeFillColor((polygon.Fill as SolidColorBrush)?.Color.ToInt() ?? Colors.Blue.WithAlpha(0.4f).ToInt());
        options.InvokeStrokeColor((polygon.Stroke as SolidColorBrush)?.Color.ToInt() ?? Android.Graphics.Color.Blue.ToArgb());
        options.InvokeStrokeWidth(context.ToPixels(polygon.StrokeThickness));
        options.InvokeZIndex(polygon.ZIndex);
        options.Visible(polygon.IsVisible);

        var holes = PolygonAttached.GetHoles(polygon);
        if (holes is not null)
        {
            foreach (var hole in holes)
            {
                if (hole is null) continue;
                options.Holes.Add(hole.Select(p => p.ToLatLng()).ToJavaList());
            }
        }

        if (polygon.StrokeDashPattern.Length != 0)
            options.InvokeStrokePattern(polygon.StrokeDashPattern.Select(v => context.ToPixels(v)).ToArray().ToPatternItems());

        options.InvokeStrokeJointType((int)polygon.StrokeLineJoin);

        return options;
    }

    public static JavaList<T> ToJavaList<T>(this IEnumerable<T> enumerable)
    {
        var array = new JavaList<T>();
        foreach (var item in enumerable ?? [])
        {
            array.Add(item);
        }

        return array;
    }
}