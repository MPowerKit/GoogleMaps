using System.ComponentModel;

using Android.Gms.Maps;
using Android.OS;

using Microsoft.Maui.Handlers;

using GMap = Android.Gms.Maps.GoogleMap;

namespace MPowerKit.GoogleMaps;

public class GoogleMapHandler : ViewHandler<GoogleMap, MapView>
{
    public static PropertyMapper<GoogleMap, GoogleMapHandler> GoogleMapHandlerMapper = new(ViewMapper)
    {

    };

    public static CommandMapper<GoogleMap, GoogleMapHandler> GoogleMapHandlerCommandMapper = new(ViewCommandMapper)
    {

    };

    public static Dictionary<string, Func<IMapFeatureManager<GoogleMap, GMap, GoogleMapHandler>>> ManagerMapper = new()
    {
        { nameof(MapManager), () => new MapManager() },
        { nameof(CameraManager), () => new CameraManager() },
        { nameof(UiSettingsManager), () => new UiSettingsManager() },
        { nameof(PolylineManager), () => new PolylineManager() },
        { nameof(PolygonManager), () => new PolygonManager() },
        { nameof(CircleManager), () => new CircleManager() },
        { nameof(TileOverlayManager), () => new TileOverlayManager() },
        { nameof(GroundOverlayManager), () => new GroundOverlayManager() },
        { nameof(PinManager), () => new PinManager() },
    };

    public GoogleMapHandler() : base(GoogleMapHandlerMapper, GoogleMapHandlerCommandMapper)
    {
    }

    public GoogleMapHandler(IPropertyMapper? mapper)
        : base(mapper ?? GoogleMapHandlerMapper, GoogleMapHandlerCommandMapper)
    {
    }

    public GoogleMapHandler(IPropertyMapper? mapper, CommandMapper? commandMapper)
        : base(mapper ?? GoogleMapHandlerMapper, commandMapper ?? GoogleMapHandlerCommandMapper)
    {
    }

    protected IMapsLifecycle? Lifecycle { get; set; }
    protected GMap? NativeMap { get; set; }

    protected Dictionary<string, IMapFeatureManager<GoogleMap, GMap, GoogleMapHandler>> Managers { get; set; } = [];

    protected override MapView CreatePlatformView()
    {
        Lifecycle = MauiContext!.Services.GetRequiredService<IMapsLifecycle>();

        var mapView = new MapView(Context);

        mapView.OnCreate(Lifecycle.GetBundleFromOnCreate());
        mapView.GetMapAsync(new OnMapReadyCallback(this));

        return mapView;
    }

    protected override void ConnectHandler(MapView platformView)
    {
        base.ConnectHandler(platformView);

        Lifecycle!.OnStart += MapsLifecycle_OnStart;
        Lifecycle.OnResume += MapsLifecycle_OnResume;
        Lifecycle.OnPause += MapsLifecycle_OnPause;
        Lifecycle.OnStop += MapsLifecycle_OnStop;
        Lifecycle.OnDestroy += MapsLifecycle_OnDestroy;
        Lifecycle.OnLowMemory += MapsLifecycle_OnLowMemory;
        Lifecycle.OnSaveInstanceState += MapsLifecycle_OnSaveInstanceState;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void OnMapReady(GMap googleMap)
    {
        NativeMap = googleMap;

        if (NativeMap is null) return;

        foreach (var kvp in ManagerMapper)
        {
            var manager = kvp.Value.Invoke();

            Managers.Add(kvp.Key, manager);

            manager.Connect(VirtualView, NativeMap, this);
        }

        VirtualView.SendNativeMapReady();
    }

    protected override void DisconnectHandler(MapView platformView)
    {
        Lifecycle!.OnStart -= MapsLifecycle_OnStart;
        Lifecycle.OnResume -= MapsLifecycle_OnResume;
        Lifecycle.OnPause -= MapsLifecycle_OnPause;
        Lifecycle.OnStop -= MapsLifecycle_OnStop;
        Lifecycle.OnDestroy -= MapsLifecycle_OnDestroy;
        Lifecycle.OnLowMemory -= MapsLifecycle_OnLowMemory;
        Lifecycle.OnSaveInstanceState -= MapsLifecycle_OnSaveInstanceState;

        if (NativeMap is not null)
        {
            foreach (var manager in Managers)
            {
                manager.Value.Disconnect(VirtualView, NativeMap, this);
            }
        }

        Managers.Clear();

        NativeMap = null;
        Lifecycle = null;

        base.DisconnectHandler(platformView);
    }

    protected virtual void MapsLifecycle_OnStart() => PlatformView?.OnStart();
    protected virtual void MapsLifecycle_OnResume() => PlatformView?.OnResume();
    protected virtual void MapsLifecycle_OnPause() => PlatformView?.OnPause();
    protected virtual void MapsLifecycle_OnStop() => PlatformView?.OnStop();
    protected virtual void MapsLifecycle_OnDestroy() => PlatformView?.OnDestroy();
    protected virtual void MapsLifecycle_OnLowMemory() => PlatformView?.OnLowMemory();
    protected virtual void MapsLifecycle_OnSaveInstanceState(Bundle bundle) => PlatformView?.OnSaveInstanceState(bundle);
}

public class OnMapReadyCallback : Java.Lang.Object, IOnMapReadyCallback
{
    protected readonly GoogleMapHandler Handler;

    public OnMapReadyCallback(GoogleMapHandler handler)
    {
        Handler = handler;
    }

    public virtual void OnMapReady(GMap googleMap)
    {
        Handler.OnMapReady(googleMap);
    }
}