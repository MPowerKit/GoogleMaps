using System.Collections.Specialized;

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
        RemoveGroundOverlaysFromNativeMap([..GroundOverlays]);
    }

    protected virtual void InitGroundOverlays()
    {
        if (VirtualView!.GroundOverlays?.Count() is null or 0) return;

        var groundOverlays = VirtualView!.GroundOverlays.ToList();

        GroundOverlays = new(groundOverlays.Count);

        AddGroundOverlaysToNativeMap(groundOverlays);
    }

    private bool _overlayPositionBanchUpdate;
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
        else if (e.PropertyName == VGroundOverlay.PositionProperty.PropertyName
            || e.PropertyName == VGroundOverlay.WidthRequestProperty.PropertyName
            || e.PropertyName == VGroundOverlay.HeightRequestProperty.PropertyName)
        {
            if (_overlayPositionBanchUpdate) return;

            native.Position = groundOverlay.Position.ToLatLng();
            if (groundOverlay.WidthRequest > 0d)
            {
                if (groundOverlay.HeightRequest > 0d)
                {
                    native.SetDimensions((float)groundOverlay.WidthRequest, (float)groundOverlay.HeightRequest);
                }
                else
                {
                    native.SetDimensions((float)groundOverlay.WidthRequest);
                }
            }

            _overlayPositionBanchUpdate = true;
            groundOverlay.OverlayBounds = native.Bounds.ToCrossPlatform();
            _overlayPositionBanchUpdate = false;
        }
        else if (e.PropertyName == VGroundOverlay.OverlayBoundsProperty.PropertyName)
        {
            if (_overlayPositionBanchUpdate) return;

            native.SetPositionFromBounds(groundOverlay.OverlayBounds?.ToNative());

            _overlayPositionBanchUpdate = true;
            groundOverlay.Position = native.Position.ToCrossPlatformPoint();
            groundOverlay.WidthRequest = native.Width;
            groundOverlay.HeightRequest = native.Height;
            _overlayPositionBanchUpdate = false;
        }
        else if (e.PropertyName == VGroundOverlay.ImageProperty.PropertyName)
        {
            SetGroundOverlayImage(groundOverlay, native)
                .ContinueWith(t =>
                {
                    groundOverlay.OverlayBounds = native.Bounds.ToCrossPlatform();
                    groundOverlay.Position = native.Position.ToCrossPlatformPoint();
                    groundOverlay.WidthRequest = native.Width;
                    groundOverlay.HeightRequest = native.Height;
                });
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
            var ngo = NativeView!.AddGroundOverlay(groundOverlay.ToNative(Handler!.MauiContext!))!;
            NativeObjectAttachedProperty.SetNativeObject(groundOverlay, ngo);
            groundOverlay.OverlayBounds = ngo.Bounds.ToCrossPlatform();
            groundOverlay.Position = ngo.Position.ToCrossPlatformPoint();
            SetGroundOverlayImage(groundOverlay, ngo)
                .ContinueWith(t =>
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        groundOverlay.OverlayBounds = ngo.Bounds.ToCrossPlatform();
                        groundOverlay.Position = ngo.Position.ToCrossPlatformPoint();
                        groundOverlay.WidthRequest = ngo.Width;
                        groundOverlay.HeightRequest = ngo.Height;
                    });
                });
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
            ngo.SetImage(GroundOverlayExtensions.DummyImage(Handler!.MauiContext!));
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