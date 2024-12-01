using Google.Maps;

using Microsoft.Maui.Handlers;

namespace MPowerKit.GoogleMaps;

public class GoogleMapHandler : ViewHandler<GoogleMap, MapView>
{
    public static PropertyMapper<GoogleMap, GoogleMapHandler> GoogleMapHandlerMapper = new(ViewMapper)
    {

    };

    public static CommandMapper<GoogleMap, GoogleMapHandler> GoogleMapHandlerCommandMapper = new(ViewCommandMapper)
    {

    };

    public static Dictionary<string, Func<IMapFeatureManager<GoogleMap, MapView, GoogleMapHandler>>> ManagerMapper = new()
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

    protected Dictionary<string, IMapFeatureManager<GoogleMap, MapView, GoogleMapHandler>> Managers { get; set; } = [];

    protected override MapView CreatePlatformView()
    {
        return new MapView(new MapViewOptions() { Camera = new Google.Maps.CameraPosition(0f, 0f, 3.5f) });
    }

    protected override void ConnectHandler(MapView platformView)
    {
        base.ConnectHandler(platformView);

        foreach (var kvp in ManagerMapper)
        {
            var manager = kvp.Value.Invoke();

            Managers.Add(kvp.Key, manager);

            manager.Connect(VirtualView, platformView, this);
        }

        VirtualView.SendNativeMapReady();
    }

    protected override void DisconnectHandler(MapView platformView)
    {
        foreach (var manager in Managers)
        {
            manager.Value.Disconnect(VirtualView, platformView, this);
        }

        Managers.Clear();

        base.DisconnectHandler(platformView);
    }
}