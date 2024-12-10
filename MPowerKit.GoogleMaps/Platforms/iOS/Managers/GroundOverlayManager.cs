using System.Collections.Specialized;

using Google.Maps;

using NGroundOverlay = Google.Maps.GroundOverlay;
using VGroundOverlay = MPowerKit.GoogleMaps.GroundOverlay;

namespace MPowerKit.GoogleMaps;

public class GroundOverlayManager : IMapFeatureManager<GoogleMap, MapView, GoogleMapHandler>
{
    protected GoogleMap? VirtualView { get; set; }
    protected MapView? NativeView { get; set; }
    protected GoogleMapHandler? Handler { get; set; }

    protected List<VGroundOverlay> GroundOverlays { get; set; } = [];

    public virtual void Connect(GoogleMap virtualView, MapView platformView, GoogleMapHandler handler)
    {
        VirtualView = virtualView;
        NativeView = platformView;
        Handler = handler;

        ResetGroundOverlays();

        virtualView.PropertyChanged += VirtualView_PropertyChanged;
        virtualView.PropertyChanging += VirtualView_PropertyChanging;

        if (virtualView.GroundOverlays is INotifyCollectionChanged groundOverlays)
        {
            groundOverlays.CollectionChanged += GroundOverlays_CollectionChanged;
        }

        platformView.OverlayTapped += NativeMap_OverlayTapped;
    }

    public virtual void Disconnect(GoogleMap virtualView, MapView platformView, GoogleMapHandler handler)
    {
        platformView.OverlayTapped -= NativeMap_OverlayTapped;

        virtualView.PropertyChanged -= VirtualView_PropertyChanged;
        virtualView.PropertyChanging -= VirtualView_PropertyChanging;

        if (virtualView.GroundOverlays is INotifyCollectionChanged groundOverlays)
        {
            groundOverlays.CollectionChanged -= GroundOverlays_CollectionChanged;
        }

        ClearGroundOverlays();

        VirtualView = null;
        NativeView = null;
        Handler = null;
    }

    protected virtual void NativeMap_OverlayTapped(object? sender, GMSOverlayEventEventArgs e)
    {
        var groundOverlay = GroundOverlays.SingleOrDefault(go => NativeObjectAttachedProperty.GetNativeObject(go) == e.Overlay);

        if (groundOverlay is null) return;

        VirtualView!.SendGroundOverlayClick(groundOverlay);
    }

    protected virtual void VirtualView_PropertyChanging(object sender, PropertyChangingEventArgs e)
    {
        if (e.PropertyName == GoogleMap.GroundOverlaysProperty.PropertyName)
        {
            if (VirtualView!.GroundOverlays is INotifyCollectionChanged groundOverlays)
            {
                groundOverlays.CollectionChanged -= GroundOverlays_CollectionChanged;
            }

            ClearGroundOverlays();
        }
    }

    protected virtual void VirtualView_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == GoogleMap.GroundOverlaysProperty.PropertyName)
        {
            InitGroundOverlays();

            if (VirtualView!.GroundOverlays is INotifyCollectionChanged groundOverlays)
            {
                groundOverlays.CollectionChanged += GroundOverlays_CollectionChanged;
            }
        }
    }

    protected virtual void GroundOverlays_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                AddGroundOverlays(e);
                break;
            case NotifyCollectionChangedAction.Remove:
                RemoveGroundOverlays(e);
                break;
            case NotifyCollectionChangedAction.Replace:
                ReplaceGroundOverlays(e);
                break;
            case NotifyCollectionChangedAction.Move:
                break;
            case NotifyCollectionChangedAction.Reset:
            default:
                ResetGroundOverlays();
                break;
        }
    }

    protected virtual void ResetGroundOverlays()
    {
        ClearGroundOverlays();

        InitGroundOverlays();
    }

    protected virtual void ClearGroundOverlays()
    {
        RemoveGroundOverlaysFromNativeMap([..GroundOverlays]);
    }

    protected virtual void InitGroundOverlays()
    {
        if (VirtualView!.GroundOverlays?.Count() is null or 0) return;

        var groundOverlays = VirtualView!.GroundOverlays.ToList();

        GroundOverlays = new(groundOverlays.Count);

        AddGroundOverlaysToNativeMap(groundOverlays);
    }

    protected virtual void GroundOverlay_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        var groundOverlay = (sender as VGroundOverlay)!;

        if (NativeObjectAttachedProperty.GetNativeObject(groundOverlay) is not NGroundOverlay native) return;

        if (e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
        {
            native.Tappable = groundOverlay.IsEnabled;
        }
        else if (e.PropertyName == VisualElement.IsVisibleProperty.PropertyName)
        {
            native.Map = groundOverlay.IsVisible ? NativeView! : null;
        }
        else if (e.PropertyName == VisualElement.ZIndexProperty.PropertyName)
        {
            native.ZIndex = groundOverlay.ZIndex;
        }
        else if (e.PropertyName == VisualElement.OpacityProperty.PropertyName)
        {
            native.Opacity = (float)groundOverlay.Opacity;
        }
        else if (e.PropertyName == VGroundOverlay.BearingProperty.PropertyName)
        {
            native.Bearing = groundOverlay.Bearing;
        }
        else if (e.PropertyName == VGroundOverlay.GroundOverlayPositionProperty.PropertyName)
        {
            groundOverlay.GroundOverlayPosition?.SetupPostion(native);
        }
        else if (e.PropertyName == VGroundOverlay.ImageProperty.PropertyName)
        {
            SetGroundOverlayImage(groundOverlay, native);
        }
    }

    protected virtual void AddGroundOverlays(NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems?.Count is null or 0 || NativeView is null) return;

        AddGroundOverlaysToNativeMap(e.NewItems.Cast<VGroundOverlay>());
    }

    protected virtual void RemoveGroundOverlays(NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems?.Count is null or 0 || NativeView is null) return;

        RemoveGroundOverlaysFromNativeMap(e.OldItems.Cast<VGroundOverlay>());
    }

    protected virtual void ReplaceGroundOverlays(NotifyCollectionChangedEventArgs e)
    {
        RemoveGroundOverlays(e);
        AddGroundOverlays(e);
    }

    protected virtual void AddGroundOverlaysToNativeMap(IEnumerable<VGroundOverlay> overlays)
    {
        foreach (var groundOverlay in overlays)
        {
            var ngo = groundOverlay.ToNative(NativeView!);
            NativeObjectAttachedProperty.SetNativeObject(groundOverlay, ngo);
            SetGroundOverlayImage(groundOverlay, ngo);
            groundOverlay.PropertyChanged += GroundOverlay_PropertyChanged;
            GroundOverlays.Add(groundOverlay);
        }
    }

    protected virtual async Task SetGroundOverlayImage(VGroundOverlay vgo, NGroundOverlay ngo)
    {
        try
        {
            ngo.Icon = (await vgo.Image.GetPlatformImageAsync(Handler!.MauiContext!))?.Value;
        }
        catch (Exception)
        {
            ngo.Icon = null;
        }
    }

    protected virtual void RemoveGroundOverlaysFromNativeMap(IEnumerable<VGroundOverlay> overlays)
    {
        foreach (var groundOverlay in overlays)
        {
            groundOverlay.PropertyChanged -= GroundOverlay_PropertyChanged;

            var native = NativeObjectAttachedProperty.GetNativeObject(groundOverlay) as NGroundOverlay;

            if (native is not null)
            {
                native.Map = null;
            }

            GroundOverlays.Remove(groundOverlay);
        }
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
            Anchor = groundOverlay.Anchor,
            Opacity = (float)groundOverlay.Opacity
        };

        groundOverlay.GroundOverlayPosition?.SetupPostion(native);

        if (groundOverlay.IsVisible)
        {
            native.Map = map;
        }

        return native;
    }

    public static void SetupPostion(this GroundOverlayPosition groundOverlayPosition, NGroundOverlay native)
    {
        native.Bounds = groundOverlayPosition switch
        {
            BoundsPosition bounds => new CoordinateBounds(bounds.Bounds.SouthWest.ToCoord(), bounds.Bounds.NorthEast.ToCoord()),
            _ => throw new NotImplementedException($"{groundOverlayPosition.GetType().Name} is not supported on iOS for GroundOverlayPosition"),
        };
    }
}