using Foundation;
using System.Web;
using Google.Maps;

using Microsoft.Maui.Platform;

using UIKit;

using NPin = Google.Maps.Marker;
using VPin = MPowerKit.GoogleMaps.Pin;

namespace MPowerKit.GoogleMaps;

public class PinManager : ItemsMapFeatureManager<VPin, NPin, MapView>
{
    protected override IEnumerable<VPin> VirtualViewItems => VirtualView!.Pins;
    protected override string VirtualViewItemsPropertyName => GoogleMap.PinsProperty.PropertyName;

    protected override void Init(GoogleMap virtualView, MapView platformView, GoogleMapHandler handler)
    {
        base.Init(virtualView, platformView, handler);

        platformView.MarkerInfoWindow = GetInfoWindow;
    }

    protected override void Reset(GoogleMap virtualView, MapView platformView, GoogleMapHandler handler)
    {
        platformView.MarkerInfoWindow = null;

        base.Reset(virtualView, platformView, handler);
    }

    protected override void SubscribeToEvents(GoogleMap virtualView, MapView platformView, GoogleMapHandler handler)
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

    protected override void UnsubscribeFromEvents(GoogleMap virtualView, MapView platformView, GoogleMapHandler handler)
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
        else if (propertyName == VPin.DefaultIconColorProperty.PropertyName)
        {
            OnIconChanged(vItem, nItem);
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
            nPin.IconView = null;

            nPin.Icon = pin.DefaultIconColor is not null
                ? NPin.MarkerImage(pin.DefaultIconColor.ToPlatform())
                : null;

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

    protected virtual UIView? GetInfoWindow(MapView map, NPin marker)
    {
        var pin = Items.Single(p => NativeObjectAttachedProperty.GetNativeObject(p) == marker);

        View infoWindow;
        if (pin.InfoWindow is not null)
        {
            infoWindow = pin.InfoWindow;
        }
        else
        {

            var template = VirtualView!.InfoWindowTemplate;

            while (template is DataTemplateSelector selector)
            {
                template = selector.SelectTemplate(pin.BindingContext, VirtualView);
            }

            if (template?.CreateContent() is not View view) return null;

            view.BindingContext = pin.BindingContext ?? pin;

            infoWindow = view;
        }

        var virtualView = VirtualView!;
        infoWindow.MaximumWidthRequest = virtualView.Width;
        infoWindow.MaximumHeightRequest = virtualView.Height / 2d;

        var platformView = infoWindow.ToNative(Handler!.MauiContext!);

        return platformView;
    }

    protected virtual bool PlatformView_PinTapped(MapView map, NPin native)
    {
        var pin = Items.SingleOrDefault(p => NativeObjectAttachedProperty.GetNativeObject(p) == native);
        if (pin?.IsEnabled is not true) return true;

        VirtualView!.SendPinClick(pin);

        return true;
    }

    protected virtual void PlatformView_DraggingMarkerStarted(object? sender, GMSMarkerEventEventArgs e)
    {
        var pin = Items.SingleOrDefault(p => NativeObjectAttachedProperty.GetNativeObject(p) == e.Marker);
        if (pin is null) return;

        pin.Position = e.Marker.Position.ToCrossPlatformPoint();

        VirtualView!.SendPinDragStart(pin);
    }

    protected virtual void PlatformView_DraggingMarker(object? sender, GMSMarkerEventEventArgs e)
    {
        var pin = Items.SingleOrDefault(p => NativeObjectAttachedProperty.GetNativeObject(p) == e.Marker);
        if (pin is null) return;

        pin.Position = e.Marker.Position.ToCrossPlatformPoint();

        VirtualView!.SendPinDragging(pin);
    }

    protected virtual void PlatformView_DraggingMarkerEnded(object? sender, GMSMarkerEventEventArgs e)
    {
        var pin = Items.SingleOrDefault(p => NativeObjectAttachedProperty.GetNativeObject(p) == e.Marker);
        if (pin is null) return;

        pin.Position = e.Marker.Position.ToCrossPlatformPoint();

        VirtualView!.SendPinDragEnd(pin);
    }

    protected virtual void PlatformView_InfoTapped(object? sender, GMSMarkerEventEventArgs e)
    {
        var pin = Items.SingleOrDefault(p => NativeObjectAttachedProperty.GetNativeObject(p) == e.Marker);
        if (pin is null) return;

        VirtualView!.SendInfoWindowClick(pin);
    }

    protected virtual void PlatformView_InfoLongPressed(object? sender, GMSMarkerEventEventArgs e)
    {
        var pin = Items.SingleOrDefault(p => NativeObjectAttachedProperty.GetNativeObject(p) == e.Marker);
        if (pin is null) return;

        VirtualView!.SendInfoWindowLongClick(pin);
    }

    protected virtual void PlatformView_InfoClosed(object? sender, GMSMarkerEventEventArgs e)
    {
        var pin = Items.SingleOrDefault(p => NativeObjectAttachedProperty.GetNativeObject(p) == e.Marker);
        if (pin?.InfoWindowShown is not true) return;

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