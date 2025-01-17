using Google.Maps;

using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Platform;

using UIKit;

using NCircle = Google.Maps.Circle;
using VCircle = MPowerKit.GoogleMaps.Circle;

namespace MPowerKit.GoogleMaps;

public class CircleManager : ItemsMapFeatureManager<VCircle, NCircle, MapView>
{
    protected override IEnumerable<VCircle> VirtualViewItems => VirtualView!.Circles;
    protected override string VirtualViewItemsPropertyName => GoogleMap.CirclesProperty.PropertyName;

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

    protected override void RemoveItemFromPlatformView(NCircle? nItem)
    {
        if (nItem is not null)
        {
            nItem.Map = null;
        }
    }

    protected override NCircle AddItemToPlatformView(VCircle vItem)
    {
        return vItem.ToNative(PlatformView!);
    }

    protected override void ItemPropertyChanged(VCircle vItem, NCircle nItem, string? propertyName)
    {
        base.ItemPropertyChanged(vItem, nItem, propertyName);

        if (propertyName == Shape.IsEnabledProperty.PropertyName)
        {
            OnIsEnabledChanged(vItem, nItem);
        }
        else if (propertyName == VCircle.RadiusProperty.PropertyName)
        {
            OnRadiusChanged(vItem, nItem);
        }
        else if (propertyName == VCircle.CenterProperty.PropertyName)
        {
            OnCenterChanged(vItem, nItem);
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
    }

    protected virtual void OnIsEnabledChanged(VCircle vCircle, NCircle nCircle)
    {
        nCircle.Tappable = vCircle.IsEnabled;
    }

    protected virtual void OnRadiusChanged(VCircle vCircle, NCircle nCircle)
    {
        nCircle.Radius = vCircle.Radius;
    }

    protected virtual void OnCenterChanged(VCircle vCircle, NCircle nCircle)
    {
        nCircle.Position = vCircle.Center.ToCoord();
    }

    protected virtual void OnIsVisibleChanged(VCircle vCircle, NCircle nCircle)
    {
        nCircle.Map = vCircle.IsVisible ? PlatformView! : null;
    }

    protected virtual void OnZIndexChanged(VCircle vCircle, NCircle nCircle)
    {
        nCircle.ZIndex = vCircle.ZIndex;
    }

    protected virtual void OnStrokeThicknessChanged(VCircle vCircle, NCircle nCircle)
    {
        nCircle.StrokeWidth = (float)vCircle.StrokeThickness;
    }

    protected virtual void OnStrokeChanged(VCircle vCircle, NCircle nCircle)
    {
        nCircle.StrokeColor = (vCircle.Stroke as SolidColorBrush)?.Color.ToPlatform()
            ?? UIColor.Black;
    }

    protected virtual void OnFillChanged(VCircle vCircle, NCircle nCircle)
    {
        nCircle.FillColor = (vCircle.Fill as SolidColorBrush)?.Color.ToPlatform()
            ?? UIColor.Clear;
    }

    protected virtual void PlatformView_OverlayTapped(object? sender, GMSOverlayEventEventArgs e)
    {
        var circle = Items.SingleOrDefault(c => NativeObjectAttachedProperty.GetNativeObject(c) == e.Overlay);
        if (circle is null) return;

        VirtualView!.SendCircleClick(circle);
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
        native.FillColor = (circle.Fill as SolidColorBrush)?.Color.ToPlatform() ?? UIColor.Clear;
        native.ZIndex = circle.ZIndex;

        if (circle.IsVisible)
        {
            native.Map = map;
        }

        return native;
    }
}