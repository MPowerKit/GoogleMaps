using System.Collections.Specialized;

using Android.Gms.Maps.Model;

using GMap = Android.Gms.Maps.GoogleMap;
using NPin = Android.Gms.Maps.Model.Marker;
using VPin = MPowerKit.GoogleMaps.Pin;

namespace MPowerKit.GoogleMaps;

public class PinManager : IMapFeatureManager<GoogleMap, GMap, GoogleMapHandler>
{
    protected GoogleMap? VirtualView { get; set; }
    protected GMap? NativeView { get; set; }
    protected GoogleMapHandler? Handler { get; set; }

    protected List<VPin> Pins { get; set; } = [];

    public virtual void Connect(GoogleMap virtualView, GMap platformView, GoogleMapHandler handler)
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

        platformView.MarkerClick += NativeMap_PinClick;
        platformView.MarkerDragStart += PlatformView_MarkerDragStart;
        platformView.MarkerDrag += PlatformView_MarkerDrag;
        platformView.MarkerDragEnd += PlatformView_MarkerDragEnd;
        platformView.InfoWindowClick += PlatformView_InfoWindowClick;
        platformView.InfoWindowLongClick += PlatformView_InfoWindowLongClick;
        platformView.InfoWindowClose += PlatformView_InfoWindowClose;

        platformView.SetInfoWindowAdapter(virtualView.InfoWindowTemplate is not null ? new InfoWindowAdapter(virtualView) : null);
    }

    public virtual void Disconnect(GoogleMap virtualView, GMap platformView, GoogleMapHandler handler)
    {
        platformView.SetInfoWindowAdapter(null);

        platformView.MarkerClick -= NativeMap_PinClick;
        platformView.MarkerDragStart -= PlatformView_MarkerDragStart;
        platformView.MarkerDrag -= PlatformView_MarkerDrag;
        platformView.MarkerDragEnd -= PlatformView_MarkerDragEnd;
        platformView.InfoWindowClick -= PlatformView_InfoWindowClick;
        platformView.InfoWindowLongClick -= PlatformView_InfoWindowLongClick;
        platformView.InfoWindowClose -= PlatformView_InfoWindowClose;

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

    protected virtual void NativeMap_PinClick(object? sender, GMap.MarkerClickEventArgs e)
    {
        e.Handled = true;

        var pin = Pins.Single(p => (NativeObjectAttachedProperty.GetNativeObject(p) as NPin)!.Id == e.Marker.Id);

        if (!pin.IsEnabled) return;

        VirtualView!.SendPinClick(pin);
    }

    protected virtual void PlatformView_MarkerDragStart(object? sender, GMap.MarkerDragStartEventArgs e)
    {
        var pin = Pins.Single(p => (NativeObjectAttachedProperty.GetNativeObject(p) as NPin)!.Id == e.Marker.Id);

        pin.Position = e.Marker.Position.ToCrossPlatformPoint();

        VirtualView!.SendPinDragStart(pin);
    }

    protected virtual void PlatformView_MarkerDrag(object? sender, GMap.MarkerDragEventArgs e)
    {
        var pin = Pins.Single(p => (NativeObjectAttachedProperty.GetNativeObject(p) as NPin)!.Id == e.Marker.Id);

        pin.Position = e.Marker.Position.ToCrossPlatformPoint();

        VirtualView!.SendPinDragging(pin);
    }

    protected virtual void PlatformView_MarkerDragEnd(object? sender, GMap.MarkerDragEndEventArgs e)
    {
        var pin = Pins.Single(p => (NativeObjectAttachedProperty.GetNativeObject(p) as NPin)!.Id == e.Marker.Id);

        pin.Position = e.Marker.Position.ToCrossPlatformPoint();

        VirtualView!.SendPinDragEnd(pin);
    }

    protected virtual void PlatformView_InfoWindowClick(object? sender, GMap.InfoWindowClickEventArgs e)
    {
        var pin = Pins.Single(p => (NativeObjectAttachedProperty.GetNativeObject(p) as NPin)!.Id == e.Marker.Id);

        VirtualView!.SendInfoWindowClick(pin);
    }

    protected virtual void PlatformView_InfoWindowLongClick(object? sender, GMap.InfoWindowLongClickEventArgs e)
    {
        var pin = Pins.Single(p => (NativeObjectAttachedProperty.GetNativeObject(p) as NPin)!.Id == e.Marker.Id);

        VirtualView!.SendInfoWindowLongClick(pin);
    }

    protected virtual void PlatformView_InfoWindowClose(object? sender, GMap.InfoWindowCloseEventArgs e)
    {
        var pin = Pins.SingleOrDefault(p => (NativeObjectAttachedProperty.GetNativeObject(p) as NPin)!.Id == e.Marker.Id);
        if (pin is null) return;

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
            NativeView!.SetInfoWindowAdapter(VirtualView!.InfoWindowTemplate is not null ? new InfoWindowAdapter(VirtualView!) : null);
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

        if (e.PropertyName == VisualElement.IsVisibleProperty.PropertyName)
        {
            native.Visible = pin.IsVisible;
        }
        else if (e.PropertyName == VisualElement.OpacityProperty.PropertyName)
        {
            native.Alpha = (float)pin.Opacity;
        }
        else if (e.PropertyName == VisualElement.ZIndexProperty.PropertyName)
        {
            native.ZIndex = pin.ZIndex;
        }
        else if (e.PropertyName == VisualElement.RotationProperty.PropertyName)
        {
            native.Rotation = (float)pin.Rotation;
        }
        else if (e.PropertyName == VPin.PositionProperty.PropertyName)
        {
            var oldPosition = native.Position.ToCrossPlatformPoint();

            if (pin.Position != oldPosition) native.Position = pin.Position.ToLatLng();
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
            native.SetAnchor((float)pin.Anchor.X, (float)pin.Anchor.Y);
        }
        else if (e.PropertyName == VPin.InfoWindowAnchorProperty.PropertyName)
        {
            native.SetInfoWindowAnchor((float)pin.InfoWindowAnchor.X, (float)pin.InfoWindowAnchor.Y);
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
        var context = Platform.AppContext;

        foreach (var pin in pins)
        {
            var npin = NativeView!.AddMarker(pin.ToNative());

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
            nPin.SetIcon(null);
            return;
        }

        try
        {
            nPin.SetIcon(await pin.Icon.ToBitmapDescriptor(Handler!.MauiContext!));
        }
        catch (Exception)
        {
            nPin.SetIcon(null);
        }
    }

    protected virtual void RemovePinsFromNativeMap(IEnumerable<VPin> overlays)
    {
        foreach (var pin in overlays)
        {
            pin.PropertyChanged -= Pin_PropertyChanged;

            var native = NativeObjectAttachedProperty.GetNativeObject(pin) as NPin;
            native?.Remove();
            Pins.Remove(pin);
        }
    }
}

public class InfoWindowAdapter : Java.Lang.Object, GMap.IInfoWindowAdapter
{
    protected GoogleMap Map { get; }

    public InfoWindowAdapter(GoogleMap map)
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
            template = selector.SelectTemplate(pin.BindingContext, null);
        }

        if (template?.CreateContent() is not View view) return null;

        view.BindingContext = pin.BindingContext;

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
        options.Anchor((float)pin.Anchor.X, (float)pin.Anchor.Y);
        options.Draggable(pin.Draggable);
        options.Flat(pin.IsFlat);
        options.InfoWindowAnchor((float)pin.InfoWindowAnchor.X, (float)pin.InfoWindowAnchor.Y);

        return options;
    }
}