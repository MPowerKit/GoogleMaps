﻿using Android.Gms.Maps.Model;

using GMap = Android.Gms.Maps.GoogleMap;
using NPin = Android.Gms.Maps.Model.Marker;
using VPin = MPowerKit.GoogleMaps.Pin;

namespace MPowerKit.GoogleMaps;

public class PinManager<TV, TN, TH> : ItemsMapFeatureManager<VPin, NPin, TV, TN, TH>
    where TV : GoogleMap
    where TN : GMap
    where TH : GoogleMapHandler
{
    protected override void Init(TV virtualView, TN platformView, TH handler)
    {
        base.Init(virtualView, platformView, handler);

        OnInfoWindowTemplateChanged(virtualView, platformView);
    }

    protected override void Reset(TV virtualView, TN platformView, TH handler)
    {
        platformView.SetInfoWindowAdapter(null);

        base.Reset(virtualView, platformView, handler);
    }

    protected override void SubscribeToEvents(TV virtualView, TN platformView, TH handler)
    {
        base.SubscribeToEvents(virtualView, platformView, handler);

        platformView.MarkerClick += PlatformView_PinClick;
        platformView.MarkerDragStart += PlatformView_MarkerDragStart;
        platformView.MarkerDrag += PlatformView_MarkerDrag;
        platformView.MarkerDragEnd += PlatformView_MarkerDragEnd;
        platformView.InfoWindowClick += PlatformView_InfoWindowClick;
        platformView.InfoWindowLongClick += PlatformView_InfoWindowLongClick;
        platformView.InfoWindowClose += PlatformView_InfoWindowClose;
    }

    protected override void UnsubscribeFromEvents(TV virtualView, TN platformView, TH handler)
    {
        platformView.MarkerClick -= PlatformView_PinClick;
        platformView.MarkerDragStart -= PlatformView_MarkerDragStart;
        platformView.MarkerDrag -= PlatformView_MarkerDrag;
        platformView.MarkerDragEnd -= PlatformView_MarkerDragEnd;
        platformView.InfoWindowClick -= PlatformView_InfoWindowClick;
        platformView.InfoWindowLongClick -= PlatformView_InfoWindowLongClick;
        platformView.InfoWindowClose -= PlatformView_InfoWindowClose;

        base.UnsubscribeFromEvents(virtualView, platformView, handler);
    }

    protected override string GetVirtualViewItemsPropertyName()
    {
        return GoogleMap.PinsProperty.PropertyName;
    }

    protected override void VirtualViewPropertyChanged(TV virtualView, TN platformView, string? propertyName)
    {
        base.VirtualViewPropertyChanged(virtualView, platformView, propertyName);

        if (propertyName == GoogleMap.InfoWindowTemplateProperty.PropertyName)
        {
            OnInfoWindowTemplateChanged(virtualView, platformView);
        }
    }

    protected override IEnumerable<VPin> GetVirtualViewItems()
    {
        return VirtualView!.Pins;
    }

    protected override void RemoveItemFromPlatformView(NPin? nItem)
    {
        nItem?.Remove();
    }

    protected override NPin AddItemToPlatformView(VPin vItem)
    {
        var npin = PlatformView!.AddMarker(vItem.ToNative());
        OnIconChanged(vItem, npin);
        return npin;
    }

    protected override void ItemPropertyChanged(VPin vItem, NPin nItem, string? propertyName)
    {
        base.ItemPropertyChanged(vItem, nItem, propertyName);

        if (propertyName == VisualElement.IsVisibleProperty.PropertyName)
        {
            OnIsVisibleChanged(vItem, nItem);
        }
        else if (propertyName == VisualElement.OpacityProperty.PropertyName)
        {
            OnOpacityChanged(vItem, nItem);
        }
        else if (propertyName == VisualElement.ZIndexProperty.PropertyName)
        {
            OnZIndexChanged(vItem, nItem);
        }
        else if (propertyName == VisualElement.RotationProperty.PropertyName)
        {
            OnRotationChanged(vItem, nItem);
        }
        else if (propertyName == VPin.PositionProperty.PropertyName)
        {
            OnPositionChanged(vItem, nItem);
        }
        else if (propertyName == VPin.SnippetProperty.PropertyName)
        {
            OnSnippetChanged(vItem, nItem);
        }
        else if (propertyName == VPin.TitleProperty.PropertyName)
        {
            OnTitleChanged(vItem, nItem);
        }
        else if (propertyName == VPin.DraggableProperty.PropertyName)
        {
            OnDraggableChanged(vItem, nItem);
        }
        else if (propertyName == VPin.IsFlatProperty.PropertyName)
        {
            OnIsFlatChanged(vItem, nItem);
        }
        else if (propertyName == VPin.AnchorXProperty.PropertyName
            || propertyName == VPin.AnchorYProperty.PropertyName)
        {
            OnAnchorChanged(vItem, nItem);
        }
        else if (propertyName == VPin.InfoWindowAnchorProperty.PropertyName)
        {
            OnInfoWindowAnchorChanged(vItem, nItem);
        }
        else if (propertyName == VPin.IconProperty.PropertyName)
        {
            OnIconChanged(vItem, nItem);
        }
    }

    protected virtual void OnIsVisibleChanged(VPin vPin, NPin nPin)
    {
        nPin.Visible = vPin.IsVisible;
    }

    protected virtual void OnOpacityChanged(VPin vPin, NPin nPin)
    {
        nPin.Alpha = (float)vPin.Opacity;
    }

    protected virtual void OnZIndexChanged(VPin vPin, NPin nPin)
    {
        nPin.ZIndex = vPin.ZIndex;
    }

    protected virtual void OnRotationChanged(VPin vPin, NPin nPin)
    {
        nPin.Rotation = (float)vPin.Rotation;
    }

    protected virtual void OnPositionChanged(VPin vPin, NPin nPin)
    {
        var oldPosition = nPin.Position.ToCrossPlatformPoint();

        if (vPin.Position != oldPosition) nPin.Position = vPin.Position.ToLatLng();
    }

    protected virtual void OnSnippetChanged(VPin vPin, NPin nPin)
    {
        nPin.Snippet = vPin.Snippet;
    }

    protected virtual void OnTitleChanged(VPin vPin, NPin nPin)
    {
        nPin.Title = vPin.Title;
    }

    protected virtual void OnDraggableChanged(VPin vPin, NPin nPin)
    {
        nPin.Draggable = vPin.Draggable;
    }

    protected virtual void OnIsFlatChanged(VPin vPin, NPin nPin)
    {
        nPin.Flat = vPin.IsFlat;
    }

    protected virtual void OnAnchorChanged(VPin vPin, NPin nPin)
    {
        nPin.SetAnchor((float)vPin.AnchorX, (float)vPin.AnchorY);
    }

    protected virtual void OnInfoWindowAnchorChanged(VPin vPin, NPin nPin)
    {
        nPin.SetInfoWindowAnchor((float)vPin.InfoWindowAnchor.X, (float)vPin.InfoWindowAnchor.Y);
    }

    protected virtual async Task OnIconChanged(VPin vPin, NPin nPin)
    {
        if (vPin.Icon is null)
        {
            nPin.SetIcon(null);
            return;
        }

        try
        {
            nPin.SetIcon(await vPin.Icon.ToBitmapDescriptor(Handler!.MauiContext!));
        }
        catch (Exception)
        {
            nPin.SetIcon(null);
        }
    }

    protected virtual void OnInfoWindowTemplateChanged(TV virtualView, TN platformView)
    {
        platformView.SetInfoWindowAdapter(virtualView.InfoWindowTemplate is not null
            ? new InfoWindowAdapter<TV>(virtualView) : null);
    }

    protected virtual void PlatformView_PinClick(object? sender, GMap.MarkerClickEventArgs e)
    {
        e.Handled = true;

        var pin = Items.Single(p => (NativeObjectAttachedProperty.GetNativeObject(p) as NPin)!.Id == e.Marker.Id);

        if (!pin.IsEnabled) return;

        VirtualView!.SendPinClick(pin);
    }

    protected virtual void PlatformView_MarkerDragStart(object? sender, GMap.MarkerDragStartEventArgs e)
    {
        var pin = Items.Single(p => (NativeObjectAttachedProperty.GetNativeObject(p) as NPin)!.Id == e.Marker.Id);

        pin.Position = e.Marker.Position.ToCrossPlatformPoint();

        VirtualView!.SendPinDragStart(pin);
    }

    protected virtual void PlatformView_MarkerDrag(object? sender, GMap.MarkerDragEventArgs e)
    {
        var pin = Items.Single(p => (NativeObjectAttachedProperty.GetNativeObject(p) as NPin)!.Id == e.Marker.Id);

        pin.Position = e.Marker.Position.ToCrossPlatformPoint();

        VirtualView!.SendPinDragging(pin);
    }

    protected virtual void PlatformView_MarkerDragEnd(object? sender, GMap.MarkerDragEndEventArgs e)
    {
        var pin = Items.Single(p => (NativeObjectAttachedProperty.GetNativeObject(p) as NPin)!.Id == e.Marker.Id);

        pin.Position = e.Marker.Position.ToCrossPlatformPoint();

        VirtualView!.SendPinDragEnd(pin);
    }

    protected virtual void PlatformView_InfoWindowClick(object? sender, GMap.InfoWindowClickEventArgs e)
    {
        var pin = Items.Single(p => (NativeObjectAttachedProperty.GetNativeObject(p) as NPin)!.Id == e.Marker.Id);

        VirtualView!.SendInfoWindowClick(pin);
    }

    protected virtual void PlatformView_InfoWindowLongClick(object? sender, GMap.InfoWindowLongClickEventArgs e)
    {
        var pin = Items.Single(p => (NativeObjectAttachedProperty.GetNativeObject(p) as NPin)!.Id == e.Marker.Id);

        VirtualView!.SendInfoWindowLongClick(pin);
    }

    protected virtual void PlatformView_InfoWindowClose(object? sender, GMap.InfoWindowCloseEventArgs e)
    {
        var pin = Items.SingleOrDefault(p => (NativeObjectAttachedProperty.GetNativeObject(p) as NPin)!.Id == e.Marker.Id);
        if (pin is null) return;

        VirtualView!.SendInfoWindowClosed(pin);
    }
}

public class InfoWindowAdapter<TV> : Java.Lang.Object, GMap.IInfoWindowAdapter
    where TV : GoogleMap
{
    protected TV Map { get; }

    public InfoWindowAdapter(TV map)
    {
        Map = map;
    }

    public virtual Android.Views.View? GetInfoContents(NPin marker)
    {
        return null;
    }

    public virtual Android.Views.View? GetInfoWindow(NPin marker)
    {
        var pin = Map.Pins.Single(p => (NativeObjectAttachedProperty.GetNativeObject(p) as NPin)!.Id == marker.Id);

        var template = Map.InfoWindowTemplate;

        while (template is DataTemplateSelector selector)
        {
            template = selector.SelectTemplate(pin.BindingContext, Map);
        }

        if (template?.CreateContent() is not View view) return null;

        view.BindingContext = pin.BindingContext ?? pin;

        var platformView = view.ToNative(Map.Handler!.MauiContext!);

        return platformView;
    }
}

public static class PinExtensions
{
    public static MarkerOptions ToNative(this VPin pin)
    {
        var options = new AdvancedMarkerOptions();

        options.SetPosition(pin.Position.ToLatLng());
        options.SetAlpha((float)pin.Opacity);
        options.SetRotation((float)pin.Rotation);
        options.SetSnippet(pin.Snippet);
        options.SetTitle(pin.Title);
        options.Visible(pin.IsVisible);
        options.InvokeZIndex(pin.ZIndex);
        options.Anchor((float)pin.AnchorX, (float)pin.AnchorY);
        options.Draggable(pin.Draggable);
        options.Flat(pin.IsFlat);
        options.InfoWindowAnchor((float)pin.InfoWindowAnchor.X, (float)pin.InfoWindowAnchor.Y);

        return options;
    }
}