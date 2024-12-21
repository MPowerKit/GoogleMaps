using Android.Gms.Maps.Model;

using GMap = Android.Gms.Maps.GoogleMap;
using NGroundOverlay = Android.Gms.Maps.Model.GroundOverlay;
using VGroundOverlay = MPowerKit.GoogleMaps.GroundOverlay;

namespace MPowerKit.GoogleMaps;

public class GroundOverlayManager : ItemsMapFeatureManager<VGroundOverlay, NGroundOverlay, GoogleMap, GMap, GoogleMapHandler>
{
    protected override void SubscribeToEvents(GoogleMap virtualView, GMap platformView, GoogleMapHandler handler)
    {
        base.SubscribeToEvents(virtualView, platformView, handler);

        platformView.GroundOverlayClick += PlatformView_GroundOverlayClick;
    }

    protected override void UnsubscribeFromEvents(GoogleMap virtualView, GMap platformView, GoogleMapHandler handler)
    {
        platformView.GroundOverlayClick -= PlatformView_GroundOverlayClick;

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
        nItem?.Remove();
    }

    protected override NGroundOverlay AddItemToPlatformView(VGroundOverlay vItem)
    {
        var overlay = PlatformView!.AddGroundOverlay(vItem.ToNative(Handler!.MauiContext!));
        OnImageChanged(vItem, overlay);
        return overlay;
    }

    private bool _overlayPositionBanchUpdate;
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
        else if (propertyName == VGroundOverlay.WidthRequestProperty.PropertyName
            || propertyName == VGroundOverlay.HeightRequestProperty.PropertyName)
        {
            OnSizeChanged(vItem, nItem);
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
        ngo.Clickable = vgo.IsEnabled;
    }

    protected virtual void OnIsVisibleChanged(VGroundOverlay vgo, NGroundOverlay ngo)
    {
        ngo.Visible = vgo.IsVisible;
    }

    protected virtual void OnZIndexChanged(VGroundOverlay vgo, NGroundOverlay ngo)
    {
        ngo.ZIndex = vgo.ZIndex;
    }

    protected virtual void OnOpacityChanged(VGroundOverlay vgo, NGroundOverlay ngo)
    {
        ngo.Transparency = 1f - (float)vgo.Opacity;
    }

    protected virtual void OnBearingChanged(VGroundOverlay vgo, NGroundOverlay ngo)
    {
        ngo.Bearing = vgo.Bearing;
    }

    protected virtual void OnSizeChanged(VGroundOverlay vgo, NGroundOverlay ngo)
    {
        OnPositionChanged(vgo, ngo);
    }

    protected virtual void OnPositionChanged(VGroundOverlay vgo, NGroundOverlay ngo)
    {
        if (_overlayPositionBanchUpdate) return;

        ngo.Position = vgo.Position.ToLatLng();
        if (vgo.WidthRequest > 0d)
        {
            if (vgo.HeightRequest > 0d)
            {
                ngo.SetDimensions((float)vgo.WidthRequest, (float)vgo.HeightRequest);
            }
            else
            {
                ngo.SetDimensions((float)vgo.WidthRequest);
            }
        }

        _overlayPositionBanchUpdate = true;
        vgo.OverlayBounds = ngo.Bounds.ToCrossPlatform();
        _overlayPositionBanchUpdate = false;
    }

    protected virtual void OnOverlayBoundsChanged(VGroundOverlay vgo, NGroundOverlay ngo)
    {
        if (_overlayPositionBanchUpdate) return;

        ngo.SetPositionFromBounds(vgo.OverlayBounds?.ToNative());

        _overlayPositionBanchUpdate = true;
        vgo.Position = ngo.Position.ToCrossPlatformPoint();
        vgo.WidthRequest = ngo.Width;
        vgo.HeightRequest = ngo.Height;
        _overlayPositionBanchUpdate = false;
    }

    protected virtual async Task OnImageChanged(VGroundOverlay vgo, NGroundOverlay ngo)
    {
        try
        {
            ngo.SetImage(await vgo.Image.ToBitmapDescriptor(Handler!.MauiContext!));
        }
        catch (Exception)
        {
            ngo.SetImage(GroundOverlayExtensions.DummyImage(Handler!.MauiContext!));
        }

        UpdateBoundsAndPosition(vgo, ngo);
    }

    protected virtual void UpdateBoundsAndPosition(VGroundOverlay vgo, NGroundOverlay ngo)
    {
        _overlayPositionBanchUpdate = true;
        vgo.OverlayBounds = ngo.Bounds.ToCrossPlatform();
        vgo.Position = ngo.Position.ToCrossPlatformPoint();
        vgo.WidthRequest = ngo.Width;
        vgo.HeightRequest = ngo.Height;
        _overlayPositionBanchUpdate = false;
    }

    protected virtual void PlatformView_GroundOverlayClick(object? sender, GMap.GroundOverlayClickEventArgs e)
    {
        var groundOverlay = Items.Single(go => (NativeObjectAttachedProperty.GetNativeObject(go) as NGroundOverlay)!.Id == e.GroundOverlay.Id);

        VirtualView!.SendGroundOverlayClick(groundOverlay);
    }
}

public static class GroundOverlayExtensions
{
    public static BitmapDescriptor DummyImage(IMauiContext mauiContext) => BitmapDescriptorFactory.FromBitmap(new ContentView() { WidthRequest = 1, HeightRequest = 1 }.ToBitmap(mauiContext));

    public static GroundOverlayOptions ToNative(this VGroundOverlay groundOverlay, IMauiContext mauiContext)
    {
        var options = new GroundOverlayOptions();

        options.Clickable(groundOverlay.IsEnabled);

        options.InvokeZIndex(groundOverlay.ZIndex);
        options.Visible(groundOverlay.IsVisible);
        options.InvokeTransparency(1f - (float)groundOverlay.Opacity);
        options.InvokeBearing(groundOverlay.Bearing);
        options.Anchor((float)groundOverlay.AnchorX, (float)groundOverlay.AnchorY);
        groundOverlay.SetupPostionForOptions(options);
        // this needs to be here because SDK does not allow to create overlay without any image
        options.InvokeImage(DummyImage(mauiContext));

        return options;
    }

    public static void SetupPostionForOptions(this VGroundOverlay groundOverlay, GroundOverlayOptions options)
    {
        if (groundOverlay.OverlayBounds is LatLngBounds bounds)
        {
            options.PositionFromBounds(bounds.ToNative());
        }
        else if (groundOverlay.Position is Point position && groundOverlay.WidthRequest > 0d)
        {
            if (groundOverlay.HeightRequest > 0d)
            {
                options.Position(position.ToLatLng(), (float)groundOverlay.WidthRequest, (float)groundOverlay.HeightRequest);
            }
            else
            {
                options.Position(position.ToLatLng(), (float)groundOverlay.WidthRequest);
            }
        }
    }
}