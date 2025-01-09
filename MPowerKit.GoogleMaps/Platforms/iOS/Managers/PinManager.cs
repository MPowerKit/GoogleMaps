using Google.Maps;

using UIKit;

using NPin = Google.Maps.Marker;
using VPin = MPowerKit.GoogleMaps.Pin;

namespace MPowerKit.GoogleMaps;

public class PinManager<TV, TN, TH> : ItemsMapFeatureManager<VPin, NPin, TV, TN, TH>
    where TV : GoogleMap
    where TN : MapView
    where TH : GoogleMapHandler
{
    protected override void Init(TV virtualView, TN platformView, TH handler)
    {
        base.Init(virtualView, platformView, handler);

        OnInfoWindowTemplateChanged(virtualView, platformView);
    }

    protected override void Reset(TV virtualView, TN platformView, TH handler)
    {
        platformView.MarkerInfoWindow = null;

        base.Reset(virtualView, platformView, handler);
    }

    protected override void SubscribeToEvents(TV virtualView, TN platformView, TH handler)
    {
        base.SubscribeToEvents(virtualView, platformView, handler);

        platformView.TappedMarker = PlatformView_PinTapped;
        platformView.DraggingMarkerStarted += PlatformView_DraggingMarkerStarted;
        platformView.DraggingMarker += PlatformView_DraggingMarker;
        platformView.DraggingMarkerEnded += PlatformView_DraggingMarkerEnded;
        platformView.InfoTapped += PlatformView_InfoTapped;
        platformView.InfoLongPressed += PlatformView_InfoLongPressed;
        platformView.InfoClosed += PlatformView_InfoClosed;
    }

    protected override void UnsubscribeFromEvents(TV virtualView, TN platformView, TH handler)
    {
        platformView.TappedMarker = null;
        platformView.DraggingMarkerStarted -= PlatformView_DraggingMarkerStarted;
        platformView.DraggingMarker -= PlatformView_DraggingMarker;
        platformView.DraggingMarkerEnded -= PlatformView_DraggingMarkerEnded;
        platformView.InfoTapped -= PlatformView_InfoTapped;
        platformView.InfoLongPressed -= PlatformView_InfoLongPressed;
        platformView.InfoClosed -= PlatformView_InfoClosed;

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
        if (nItem is not null)
        {
            nItem.Map = null;
        }
    }

    protected override NPin AddItemToPlatformView(VPin vItem)
    {
        var npin = vItem.ToNative(PlatformView!);
        OnIconChanged(vItem, npin);
        return npin;
    }

    protected override void ItemPropertyChanged(VPin vItem, NPin nItem, string? propertyName)
    {
        base.ItemPropertyChanged(vItem, nItem, propertyName);

        if (propertyName == VisualElement.IsEnabledProperty.PropertyName)
        {
            OnIsEnabledChanged(vItem, nItem);
        }
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

    protected virtual void OnIsEnabledChanged(VPin vPin, NPin nPin)
    {
        nPin.Tappable = vPin.IsEnabled;
    }

    protected virtual void OnIsVisibleChanged(VPin vPin, NPin nPin)
    {
        nPin.Map = vPin.IsVisible ? PlatformView! : null;
    }

    protected virtual void OnOpacityChanged(VPin vPin, NPin nPin)
    {
        nPin.Opacity = (float)vPin.Opacity;
    }

    protected virtual void OnZIndexChanged(VPin vPin, NPin nPin)
    {
        nPin.ZIndex = vPin.ZIndex;
    }

    protected virtual void OnRotationChanged(VPin vPin, NPin nPin)
    {
        nPin.Rotation = vPin.Rotation;
    }

    protected virtual void OnPositionChanged(VPin vPin, NPin nPin)
    {
        var oldPosition = nPin.Position.ToCrossPlatformPoint();

        if (vPin.Position != oldPosition) nPin.Position = vPin.Position.ToCoord();
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
        nPin.GroundAnchor = new(vPin.AnchorX, vPin.AnchorY);
    }

    protected virtual void OnInfoWindowAnchorChanged(VPin vPin, NPin nPin)
    {
        nPin.InfoWindowAnchor = vPin.InfoWindowAnchor;
    }

    protected virtual async Task OnIconChanged(VPin pin, NPin nPin)
    {
        if (pin.Icon is null)
        {
            nPin.Icon = null;
            nPin.IconView = null;
            return;
        }

        try
        {
            if (pin.Icon is ViewImageSource viewSource)
            {
                nPin.Icon = null;
                nPin.IconView = viewSource.View?.ToNative(Handler!.MauiContext!);
            }
            else
            {
                nPin.IconView = null;
                nPin.Icon = (await pin.Icon.GetPlatformImageAsync(Handler!.MauiContext!))?.Value;
            }
        }
        catch (Exception)
        {
            nPin.Icon = null;
            nPin.IconView = null;
        }
    }

    protected virtual void OnInfoWindowTemplateChanged(TV virtualView, TN platformView)
    {
        platformView.MarkerInfoWindow = virtualView.InfoWindowTemplate is not null ? GetInfoWindow : null;
    }

    protected virtual UIView? GetInfoWindow(MapView map, NPin marker)
    {
        var pin = Items.Single(p => NativeObjectAttachedProperty.GetNativeObject(p) == marker);

        var template = VirtualView!.InfoWindowTemplate;

        while (template is DataTemplateSelector selector)
        {
            template = selector.SelectTemplate(pin.BindingContext, VirtualView);
        }

        if (template?.CreateContent() is not View view) return null;

        view.BindingContext = pin.BindingContext ?? pin;

        var platformView = view.ToNative(Handler!.MauiContext!);

        return platformView;
    }

    protected virtual bool PlatformView_PinTapped(MapView map, NPin native)
    {
        var pin = Items.Single(p => NativeObjectAttachedProperty.GetNativeObject(p) == native);

        VirtualView!.SendPinClick(pin);

        return true;
    }

    protected virtual void PlatformView_DraggingMarkerStarted(object? sender, GMSMarkerEventEventArgs e)
    {
        var pin = Items.Single(p => NativeObjectAttachedProperty.GetNativeObject(p) == e.Marker);

        pin.Position = e.Marker.Position.ToCrossPlatformPoint();

        VirtualView!.SendPinDragStart(pin);
    }

    protected virtual void PlatformView_DraggingMarker(object? sender, GMSMarkerEventEventArgs e)
    {
        var pin = Items.Single(p => NativeObjectAttachedProperty.GetNativeObject(p) == e.Marker);

        pin.Position = e.Marker.Position.ToCrossPlatformPoint();

        VirtualView!.SendPinDragging(pin);
    }

    protected virtual void PlatformView_DraggingMarkerEnded(object? sender, GMSMarkerEventEventArgs e)
    {
        var pin = Items.Single(p => NativeObjectAttachedProperty.GetNativeObject(p) == e.Marker);

        pin.Position = e.Marker.Position.ToCrossPlatformPoint();

        VirtualView!.SendPinDragEnd(pin);
    }

    protected virtual void PlatformView_InfoTapped(object? sender, GMSMarkerEventEventArgs e)
    {
        var pin = Items.Single(p => NativeObjectAttachedProperty.GetNativeObject(p) == e.Marker);

        VirtualView!.SendInfoWindowClick(pin);
    }

    protected virtual void PlatformView_InfoLongPressed(object? sender, GMSMarkerEventEventArgs e)
    {
        var pin = Items.Single(p => NativeObjectAttachedProperty.GetNativeObject(p) == e.Marker);

        VirtualView!.SendInfoWindowLongClick(pin);
    }

    protected virtual void PlatformView_InfoClosed(object? sender, GMSMarkerEventEventArgs e)
    {
        var pin = Items.Single(p => NativeObjectAttachedProperty.GetNativeObject(p) == e.Marker);

        if (!pin.InfoWindowShown) return;

        VirtualView!.SendInfoWindowClosed(pin);
    }
}

public static class PinExtensions
{
    public static NPin ToNative(this VPin pin, MapView map)
    {
        var native = NPin.FromPosition(pin.Position.ToCoord());
        native.Tappable = pin.IsEnabled;
        native.Opacity = (float)pin.Opacity;
        native.Rotation = pin.Rotation;
        native.Snippet = pin.Snippet;
        native.Title = pin.Title;
        native.ZIndex = pin.ZIndex;
        native.GroundAnchor = new(pin.AnchorX, pin.AnchorY);
        native.Draggable = pin.Draggable;
        native.Flat = pin.IsFlat;
        native.InfoWindowAnchor = pin.InfoWindowAnchor;

        if (pin.IsVisible)
        {
            native.Map = map;
        }

        return native;
    }
}