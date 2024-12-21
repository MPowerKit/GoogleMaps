using Google.Maps;

using NGroundOverlay = Google.Maps.GroundOverlay;
using VGroundOverlay = MPowerKit.GoogleMaps.GroundOverlay;

namespace MPowerKit.GoogleMaps;

public class GroundOverlayManager : ItemsMapFeatureManager<VGroundOverlay, NGroundOverlay, GoogleMap, MapView, GoogleMapHandler>
{
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

    protected override string GetVirtualViewItemsPropertyName()
    {
        return GoogleMap.GroundOverlaysProperty.PropertyName;
    }

    protected override IEnumerable<VGroundOverlay> GetVirtualViewItems()
    {
        return VirtualView!.GroundOverlays;
    }

    protected override void RemoveItemFromPlatformView(NGroundOverlay? nItem)
    {
        if (nItem is not null)
        {
            nItem.Map = null;
        }
    }

    protected override NGroundOverlay AddItemToPlatformView(VGroundOverlay vItem)
    {
        var ngo = vItem.ToNative(PlatformView!);
        OnImageChanged(vItem, ngo);
        return ngo;
    }

    protected override void ItemPropertyChanged(VGroundOverlay vItem, NGroundOverlay nItem, string? propertyName)
    {
        base.ItemPropertyChanged(vItem, nItem, propertyName);

        if (propertyName == VisualElement.IsEnabledProperty.PropertyName)
        {
            OnIsEnabledChanged(vItem, nItem);
        }
        else if (propertyName == VisualElement.IsVisibleProperty.PropertyName)
        {
            OnIsVisibleChanged(vItem, nItem);
        }
        else if (propertyName == VisualElement.ZIndexProperty.PropertyName)
        {
            OnZIndexChanged(vItem, nItem);
        }
        else if (propertyName == VisualElement.OpacityProperty.PropertyName)
        {
            OnOpacityChanged(vItem, nItem);
        }
        else if (propertyName == VGroundOverlay.BearingProperty.PropertyName)
        {
            OnBearingChanged(vItem, nItem);
        }
        else if (propertyName == VisualElement.AnchorXProperty.PropertyName
            || propertyName == VisualElement.AnchorYProperty.PropertyName)
        {
            OnAnchorChanged(vItem, nItem);
        }
        else if (propertyName == VGroundOverlay.PositionProperty.PropertyName)
        {
            OnPositionChanged(vItem, nItem);
        }
        else if (propertyName == VGroundOverlay.OverlayBoundsProperty.PropertyName)
        {
            OnOverlayBoundsChanged(vItem, nItem);
        }
        else if (propertyName == VGroundOverlay.ImageProperty.PropertyName)
        {
            OnImageChanged(vItem, nItem);
        }
    }

    protected virtual void OnIsEnabledChanged(VGroundOverlay vgo, NGroundOverlay ngo)
    {
        ngo.Tappable = vgo.IsEnabled;
    }

    protected virtual void OnIsVisibleChanged(VGroundOverlay vgo, NGroundOverlay ngo)
    {
        ngo.Map = vgo.IsVisible ? PlatformView! : null;
    }

    protected virtual void OnZIndexChanged(VGroundOverlay vgo, NGroundOverlay ngo)
    {
        ngo.ZIndex = vgo.ZIndex;
    }

    protected virtual void OnOpacityChanged(VGroundOverlay vgo, NGroundOverlay ngo)
    {
        ngo.Opacity = (float)vgo.Opacity;
    }

    protected virtual void OnBearingChanged(VGroundOverlay vgo, NGroundOverlay ngo)
    {
        ngo.Bearing = vgo.Bearing;
    }

    protected virtual void OnAnchorChanged(VGroundOverlay vgo, NGroundOverlay ngo)
    {
        ngo.Anchor = new(vgo.AnchorX, vgo.AnchorY);
        vgo.Position = ngo.Position.ToCrossPlatformPoint();
    }

    protected virtual void OnPositionChanged(VGroundOverlay vgo, NGroundOverlay ngo)
    {
        ngo.Position = vgo.Position.ToCoord();
        vgo.OverlayBounds = ngo.Bounds!.ToCrossPlatform();
    }

    protected virtual void OnOverlayBoundsChanged(VGroundOverlay vgo, NGroundOverlay ngo)
    {
        ngo.Bounds = vgo.OverlayBounds?.ToNative();
        vgo.Position = ngo.Position.ToCrossPlatformPoint();
    }

    protected virtual async Task OnImageChanged(VGroundOverlay vgo, NGroundOverlay ngo)
    {
        try
        {
            await Task.Yield();
            ngo.Icon = (await vgo.Image.GetPlatformImageAsync(Handler!.MauiContext!))?.Value;
        }
        catch (Exception)
        {
            ngo.Icon = null;
        }

        UpdateBoundsAndPosition(vgo, ngo);
    }

    protected virtual void UpdateBoundsAndPosition(VGroundOverlay vgo, NGroundOverlay ngo)
    {
        vgo.OverlayBounds = ngo.Bounds?.ToCrossPlatform();
        vgo.Position = ngo.Position.ToCrossPlatformPoint();
    }

    protected virtual void PlatformView_OverlayTapped(object? sender, GMSOverlayEventEventArgs e)
    {
        var groundOverlay = Items.SingleOrDefault(go => NativeObjectAttachedProperty.GetNativeObject(go) == e.Overlay);

        if (groundOverlay is null) return;

        VirtualView!.SendGroundOverlayClick(groundOverlay);
    }
}

public static class GroundOverlayExtensions
{
    public static NGroundOverlay ToNative(this VGroundOverlay groundOverlay, MapView map)
    {
        var native = new NGroundOverlay
        {
            Tappable = groundOverlay.IsEnabled,
            ZIndex = groundOverlay.ZIndex,
            Bearing = groundOverlay.Bearing,
            Anchor = new(groundOverlay.AnchorX, groundOverlay.AnchorY),
            Opacity = (float)groundOverlay.Opacity
        };

        groundOverlay.SetupPostionForOverlay(native);

        if (groundOverlay.IsVisible)
        {
            native.Map = map;
        }

        return native;
    }

    public static void SetupPostionForOverlay(this VGroundOverlay groundOverlay, NGroundOverlay overlay)
    {
        if (groundOverlay.OverlayBounds is LatLngBounds bounds)
        {
            overlay.Bounds = bounds.ToNative();
        }
        else if (groundOverlay.Position is Point position)
        {
            overlay.Position = position.ToCoord();
        }
    }
}