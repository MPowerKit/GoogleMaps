using System.Collections.Specialized;

using Google.Maps;

using UIKit;

using NPin = Google.Maps.Marker;
using VPin = MPowerKit.GoogleMaps.Pin;

namespace MPowerKit.GoogleMaps;

public class PinManager : IMapFeatureManager<GoogleMap, MapView, GoogleMapHandler>
{
    protected GoogleMap? VirtualView { get; set; }
    protected MapView? NativeView { get; set; }
    protected GoogleMapHandler? Handler { get; set; }

    protected List<VPin> Pins { get; set; } = [];

    public virtual void Connect(GoogleMap virtualView, MapView platformView, GoogleMapHandler handler)
    {
        VirtualView = virtualView;
        NativeView = platformView;
        Handler = handler;

        ResetPins();

        virtualView.PropertyChanged += VirtualView_PropertyChanged;
        virtualView.PropertyChanging += VirtualView_PropertyChanging;

        if (virtualView.Pins is INotifyCollectionChanged pins)
        {
            pins.CollectionChanged += Pins_CollectionChanged;
        }

        platformView.TappedMarker = NativeMap_PinTapped;
        platformView.DraggingMarkerStarted += PlatformView_DraggingMarkerStarted;
        platformView.DraggingMarker += PlatformView_DraggingMarker;
        platformView.DraggingMarkerEnded += PlatformView_DraggingMarkerEnded;
        platformView.InfoTapped += PlatformView_InfoTapped;
        platformView.InfoLongPressed += PlatformView_InfoLongPressed;
        platformView.InfoClosed += PlatformView_InfoClosed;

        platformView.MarkerInfoWindow = virtualView.InfoWindowTemplate is not null ? GetInfoWindow : null;
    }

    public virtual void Disconnect(GoogleMap virtualView, MapView platformView, GoogleMapHandler handler)
    {
        platformView.MarkerInfoWindow = null;

        platformView.TappedMarker = null;
        platformView.DraggingMarkerStarted -= PlatformView_DraggingMarkerStarted;
        platformView.DraggingMarker -= PlatformView_DraggingMarker;
        platformView.DraggingMarkerEnded -= PlatformView_DraggingMarkerEnded;
        platformView.InfoTapped -= PlatformView_InfoTapped;
        platformView.InfoLongPressed -= PlatformView_InfoLongPressed;
        platformView.InfoClosed -= PlatformView_InfoClosed;

        virtualView.PropertyChanged -= VirtualView_PropertyChanged;
        virtualView.PropertyChanging -= VirtualView_PropertyChanging;

        if (virtualView.Pins is INotifyCollectionChanged pins)
        {
            pins.CollectionChanged -= Pins_CollectionChanged;
        }

        ClearPins();

        VirtualView = null;
        NativeView = null;
        Handler = null;
    }

    protected virtual bool NativeMap_PinTapped(MapView map, NPin native)
    {
        var pin = Pins.Single(p => NativeObjectAttachedProperty.GetNativeObject(p) == native);

        VirtualView!.SendPinClick(pin);

        return true;
    }

    protected virtual void PlatformView_DraggingMarkerStarted(object? sender, GMSMarkerEventEventArgs e)
    {
        var pin = Pins.Single(p => NativeObjectAttachedProperty.GetNativeObject(p) == e.Marker);

        pin.Position = e.Marker.Position.ToCrossPlatformPoint();

        VirtualView!.SendPinDragStart(pin);
    }

    protected virtual void PlatformView_DraggingMarker(object? sender, GMSMarkerEventEventArgs e)
    {
        var pin = Pins.Single(p => NativeObjectAttachedProperty.GetNativeObject(p) == e.Marker);

        pin.Position = e.Marker.Position.ToCrossPlatformPoint();

        VirtualView!.SendPinDragging(pin);
    }

    protected virtual void PlatformView_DraggingMarkerEnded(object? sender, GMSMarkerEventEventArgs e)
    {
        var pin = Pins.Single(p => NativeObjectAttachedProperty.GetNativeObject(p) == e.Marker);

        pin.Position = e.Marker.Position.ToCrossPlatformPoint();

        VirtualView!.SendPinDragEnd(pin);
    }

    protected virtual void PlatformView_InfoTapped(object? sender, GMSMarkerEventEventArgs e)
    {
        var pin = Pins.Single(p => NativeObjectAttachedProperty.GetNativeObject(p) == e.Marker);

        VirtualView!.SendInfoWindowClick(pin);
    }

    protected virtual void PlatformView_InfoLongPressed(object? sender, GMSMarkerEventEventArgs e)
    {
        var pin = Pins.Single(p => NativeObjectAttachedProperty.GetNativeObject(p) == e.Marker);

        VirtualView!.SendInfoWindowLongClick(pin);
    }

    protected virtual void PlatformView_InfoClosed(object? sender, GMSMarkerEventEventArgs e)
    {
        var pin = Pins.Single(p => NativeObjectAttachedProperty.GetNativeObject(p) == e.Marker);

        if (!pin.InfoWindowShown) return;

        VirtualView!.SendInfoWindowClose(pin);
    }

    protected virtual void VirtualView_PropertyChanging(object sender, PropertyChangingEventArgs e)
    {
        if (e.PropertyName == GoogleMap.PinsProperty.PropertyName)
        {
            if (VirtualView!.Pins is INotifyCollectionChanged pins)
            {
                pins.CollectionChanged -= Pins_CollectionChanged;
            }

            ClearPins();
        }
    }

    protected virtual void VirtualView_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == GoogleMap.PinsProperty.PropertyName)
        {
            InitPins();

            if (VirtualView!.Pins is INotifyCollectionChanged pins)
            {
                pins.CollectionChanged += Pins_CollectionChanged;
            }
        }
        else if (e.PropertyName == GoogleMap.InfoWindowTemplateProperty.PropertyName)
        {
            NativeView!.MarkerInfoWindow = VirtualView!.InfoWindowTemplate is not null ? GetInfoWindow : null;
        }
    }

    protected virtual void Pins_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                AddPins(e);
                break;
            case NotifyCollectionChangedAction.Remove:
                RemovePins(e);
                break;
            case NotifyCollectionChangedAction.Replace:
                ReplacePins(e);
                break;
            case NotifyCollectionChangedAction.Move:
                break;
            case NotifyCollectionChangedAction.Reset:
            default:
                ResetPins();
                break;
        }
    }

    protected virtual void ResetPins()
    {
        ClearPins();

        InitPins();
    }

    protected virtual void ClearPins()
    {
        RemovePinsFromNativeMap([..Pins]);
    }

    protected virtual void InitPins()
    {
        if (VirtualView!.Pins?.Count() is null or 0) return;

        var pins = VirtualView!.Pins.ToList();

        Pins = new(pins.Count);

        AddPinsToNativeMap(pins);
    }

    protected virtual void Pin_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        var pin = (sender as VPin)!;

        if (NativeObjectAttachedProperty.GetNativeObject(pin) is not NPin native) return;

        if (e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
        {
            native.Tappable = pin.IsEnabled;
        }
        if (e.PropertyName == VisualElement.IsVisibleProperty.PropertyName)
        {
            native.Map = pin.IsVisible ? NativeView! : null;
        }
        else if (e.PropertyName == VisualElement.OpacityProperty.PropertyName)
        {
            native.Opacity = (float)pin.Opacity;
        }
        else if (e.PropertyName == VisualElement.ZIndexProperty.PropertyName)
        {
            native.ZIndex = pin.ZIndex;
        }
        else if (e.PropertyName == VisualElement.RotationProperty.PropertyName)
        {
            native.Rotation = pin.Rotation;
        }
        else if (e.PropertyName == VPin.PositionProperty.PropertyName)
        {
            var oldPosition = native.Position.ToCrossPlatformPoint();

            if (pin.Position != oldPosition) native.Position = pin.Position.ToCoord();
        }
        else if (e.PropertyName == VPin.SnippetProperty.PropertyName)
        {
            native.Snippet = pin.Snippet;
        }
        else if (e.PropertyName == VPin.TitleProperty.PropertyName)
        {
            native.Title = pin.Title;
        }
        else if (e.PropertyName == VPin.DraggableProperty.PropertyName)
        {
            native.Draggable = pin.Draggable;
        }
        else if (e.PropertyName == VPin.IsFlatProperty.PropertyName)
        {
            native.Flat = pin.IsFlat;
        }
        else if (e.PropertyName == VPin.AnchorProperty.PropertyName)
        {
            native.GroundAnchor = pin.Anchor;
        }
        else if (e.PropertyName == VPin.InfoWindowAnchorProperty.PropertyName)
        {
            native.InfoWindowAnchor = pin.InfoWindowAnchor;
        }
        else if (e.PropertyName == VPin.IconProperty.PropertyName)
        {
            SetPinIcon(pin, native);
        }
    }

    protected virtual void AddPins(NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems?.Count is null or 0 || NativeView is null) return;

        AddPinsToNativeMap(e.NewItems.Cast<VPin>());
    }

    protected virtual void RemovePins(NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems?.Count is null or 0 || NativeView is null) return;

        RemovePinsFromNativeMap(e.OldItems.Cast<VPin>());
    }

    protected virtual void ReplacePins(NotifyCollectionChangedEventArgs e)
    {
        RemovePins(e);
        AddPins(e);
    }

    protected virtual void AddPinsToNativeMap(IEnumerable<VPin> pins)
    {
        foreach (var pin in pins)
        {
            var npin = pin.ToNative(NativeView!);

            NativeObjectAttachedProperty.SetNativeObject(pin, npin);
            SetPinIcon(pin, npin);

            pin.PropertyChanged += Pin_PropertyChanged;
            Pins.Add(pin);
        }
    }

    protected virtual async Task SetPinIcon(VPin pin, NPin nPin)
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

    protected virtual void RemovePinsFromNativeMap(IEnumerable<VPin> overlays)
    {
        foreach (var pin in overlays)
        {
            pin.PropertyChanged -= Pin_PropertyChanged;

            var native = NativeObjectAttachedProperty.GetNativeObject(pin) as NPin;

            if (native is not null)
            {
                native.Map = null;
            }

            Pins.Remove(pin);
        }
    }

    protected virtual UIView? GetInfoWindow(MapView map, NPin marker)
    {
        var pin = Pins.Single(p => NativeObjectAttachedProperty.GetNativeObject(p) == marker);

        var template = VirtualView!.InfoWindowTemplate;

        while (template is DataTemplateSelector selector)
        {
            template = selector.SelectTemplate(pin.BindingContext, null);
        }

        if (template?.CreateContent() is not View view) return null;

        view.BindingContext = pin.BindingContext;

        var platformView = view.ToNative(Handler!.MauiContext!);

        return platformView;
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
        native.GroundAnchor = pin.Anchor;
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