using System.Collections.Specialized;

using Android.Content;
using Android.Gms.Maps.Model;

using GMap = Android.Gms.Maps.GoogleMap;
using NGroundOverlay = Android.Gms.Maps.Model.GroundOverlay;
using VGroundOverlay = MPowerKit.GoogleMaps.GroundOverlay;

namespace MPowerKit.GoogleMaps;

public class GroundOverlayManager : IMapFeatureManager<GoogleMap, GMap, GoogleMapHandler>
{
    protected GoogleMap? VirtualView { get; set; }
    protected GMap? NativeView { get; set; }
    protected GoogleMapHandler? Handler { get; set; }

    protected List<VGroundOverlay> GroundOverlays { get; set; } = [];

    public virtual void Connect(GoogleMap virtualView, GMap platformView, GoogleMapHandler handler)
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

        platformView.GroundOverlayClick += NativeMap_GroundOverlayClick;
    }

    public virtual void Disconnect(GoogleMap virtualView, GMap platformView, GoogleMapHandler handler)
    {
        platformView.GroundOverlayClick -= NativeMap_GroundOverlayClick;

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

    protected virtual void NativeMap_GroundOverlayClick(object? sender, GMap.GroundOverlayClickEventArgs e)
    {
        var groundOverlay = GroundOverlays.Single(go => (NativeObjectAttachedProperty.GetNativeObject(go) as NGroundOverlay)!.Id == e.GroundOverlay.Id);

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
        RemoveGroundOverlaysFromNativeMap([.. GroundOverlays]);
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
            native.Clickable = groundOverlay.IsEnabled;
        }
        else if (e.PropertyName == VisualElement.IsVisibleProperty.PropertyName)
        {
            native.Visible = groundOverlay.IsVisible;
        }
        else if (e.PropertyName == VisualElement.ZIndexProperty.PropertyName)
        {
            native.ZIndex = groundOverlay.ZIndex;
        }
        else if (e.PropertyName == VisualElement.OpacityProperty.PropertyName)
        {
            native.Transparency = 1f - (float)groundOverlay.Opacity;
        }
        else if (e.PropertyName == VGroundOverlay.BearingProperty.PropertyName)
        {
            native.Bearing = groundOverlay.Bearing;
        }
        else if (e.PropertyName == VGroundOverlay.GroundOverlayPositionProperty.PropertyName)
        {
            groundOverlay.GroundOverlayPosition?.SetupPostionForOverlay(native);
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
        var context = Platform.AppContext;

        foreach (var groundOverlay in overlays)
        {
            var ngo = NativeView!.AddGroundOverlay(groundOverlay.ToNative());
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
            ngo.SetImage(await vgo.Image.ToBitmapDescriptor(Handler!.MauiContext!));
        }
        catch (Exception)
        {
            ngo.SetImage(null);
        }
    }

    protected virtual void RemoveGroundOverlaysFromNativeMap(IEnumerable<VGroundOverlay> overlays)
    {
        foreach (var groundOverlay in overlays)
        {
            groundOverlay.PropertyChanged -= GroundOverlay_PropertyChanged;

            var native = NativeObjectAttachedProperty.GetNativeObject(groundOverlay) as NGroundOverlay;
            native?.Remove();

            GroundOverlays.Remove(groundOverlay);
        }
    }
}

public static class GroundOverlayExtensions
{
    public static GroundOverlayOptions ToNative(this VGroundOverlay groundOverlay)
    {
        var options = new GroundOverlayOptions();

        options.Clickable(groundOverlay.IsEnabled);

        options.InvokeZIndex(groundOverlay.ZIndex);
        options.Visible(groundOverlay.IsVisible);
        options.InvokeTransparency(1f - (float)groundOverlay.Opacity);
        options.InvokeBearing(groundOverlay.Bearing);
        options.Anchor((float)groundOverlay.Anchor.X, (float)groundOverlay.Anchor.Y);
        groundOverlay.GroundOverlayPosition?.SetupPostionForOptions(options);

        return options;
    }

    public static void SetupPostionForOptions(this GroundOverlayPosition groundOverlayPosition, GroundOverlayOptions options)
    {
        switch (groundOverlayPosition)
        {
            case BoundsPosition bounds:
                options.PositionFromBounds(bounds.Bounds.ToNative());
                break;
            case CenterAndWidthPosition centerAndWidth:
                options.Position(centerAndWidth.Center.ToLatLng(), centerAndWidth.Width);
                break;
            case CenterAndSizePosition centerAndSize:
                options.Position(centerAndSize.Center.ToLatLng(), (float)centerAndSize.Size.Width, (float)centerAndSize.Size.Height);
                break;
        }
    }

    public static void SetupPostionForOverlay(this GroundOverlayPosition groundOverlayPosition, NGroundOverlay overlay)
    {
        switch (groundOverlayPosition)
        {
            case BoundsPosition bounds:
                overlay.SetPositionFromBounds(new Android.Gms.Maps.Model.LatLngBounds(bounds.Bounds.SouthWest.ToLatLng(), bounds.Bounds.NorthEast.ToLatLng()));
                break;
            case CenterAndWidthPosition centerAndWidth:
                overlay.Position = centerAndWidth.Center.ToLatLng();
                overlay.SetDimensions(centerAndWidth.Width);
                break;
            case CenterAndSizePosition centerAndSize:
                overlay.Position = centerAndSize.Center.ToLatLng();
                overlay.SetDimensions((float)centerAndSize.Size.Width, (float)centerAndSize.Size.Height);
                break;
        }
    }
}