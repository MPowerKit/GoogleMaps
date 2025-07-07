using Google.Maps;

using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Platform;

using UIKit;

using NPolygon = Google.Maps.Polygon;
using VPolygon = Microsoft.Maui.Controls.Shapes.Polygon;

namespace MPowerKit.GoogleMaps;

public class PolygonManager : ItemsMapFeatureManager<VPolygon, NPolygon, MapView>
{
    protected override IEnumerable<VPolygon> VirtualViewItems => VirtualView!.Polygons;
    protected override string VirtualViewItemsPropertyName => GoogleMap.PolygonsProperty.PropertyName;

    protected override void SubscribeToEvents(GoogleMap virtualView, MapView platformView, GoogleMapHandler handler)
    {
        base.SubscribeToEvents(virtualView, platformView, handler);

        platformView.OverlayTapped += PlatformView_OverlayTapped;
    }

    protected override void UnsubscribeFromEvents(GoogleMap virtualView, MapView platformView, GoogleMapHandler handler)
    {
        platformView.OverlayTapped -= PlatformView_OverlayTapped;

        base.UnsubscribeFromEvents(virtualView, platformView, handler);
    }

    protected override void RemoveItemFromPlatformView(NPolygon? nItem)
    {
        if (nItem is not null)
        {
            nItem.Map = null;
        }
    }

    protected override NPolygon AddItemToPlatformView(VPolygon vItem)
    {
        return vItem.ToNative(PlatformView!);
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
        nPolygon.Tappable = vPolygon.IsEnabled;
    }

    protected virtual void OnPointsChanged(VPolygon vPolygon, NPolygon nPolygon)
    {
        nPolygon.Path = vPolygon.Points.ToPath();
    }

    protected virtual void OnIsVisibleChanged(VPolygon vPolygon, NPolygon nPolygon)
    {
        nPolygon.Map = vPolygon.IsVisible ? PlatformView! : null;
    }

    protected virtual void OnZIndexChanged(VPolygon vPolygon, NPolygon nPolygon)
    {
        nPolygon.ZIndex = vPolygon.ZIndex;
    }

    protected virtual void OnStrokeThicknessChanged(VPolygon vPolygon, NPolygon nPolygon)
    {
        nPolygon.StrokeWidth = (float)vPolygon.StrokeThickness;
    }

    protected virtual void OnStrokeChanged(VPolygon vPolygon, NPolygon nPolygon)
    {
        nPolygon.StrokeColor = (vPolygon.Stroke as SolidColorBrush)?.Color.ToPlatform() ?? UIColor.Blue;
    }

    protected virtual void OnFillChanged(VPolygon vPolygon, NPolygon nPolygon)
    {
        nPolygon.FillColor = (vPolygon.Fill as SolidColorBrush)?.Color.ToPlatform() ?? Colors.Blue.WithAlpha(0.4f).ToPlatform();
    }

    protected virtual void OnHolesChanged(VPolygon vPolygon, NPolygon nPolygon)
    {
        var holes = PolygonAttached.GetHoles(vPolygon);
        nPolygon.Holes = holes?.Select(h => h?.ToPath() ?? new()).ToArray();
    }

    protected virtual void PlatformView_OverlayTapped(object? sender, GMSOverlayEventEventArgs e)
    {
        var polygon = Items.SingleOrDefault(p => NativeObjectAttachedProperty.GetNativeObject(p) == e.Overlay);
        if (polygon is null) return;

        VirtualView!.SendPolygonClick(polygon);
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
        native.StrokeColor = (polygon.Stroke as SolidColorBrush)?.Color.ToPlatform() ?? UIColor.Blue;
        native.FillColor = (polygon.Fill as SolidColorBrush)?.Color.ToPlatform() ?? Colors.Blue.WithAlpha(0.4f).ToPlatform();
        var holes = PolygonAttached.GetHoles(polygon);
        native.Holes = holes?.Select(h => h?.ToPath() ?? new()).ToArray();

        if (polygon.IsVisible)
        {
            native.Map = map;
        }

        return native;
    }
}