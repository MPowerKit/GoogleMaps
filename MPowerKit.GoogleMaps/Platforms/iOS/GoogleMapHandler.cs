using Google.Maps;

using Microsoft.Maui.Handlers;

namespace MPowerKit.GoogleMaps;

public partial class GoogleMapHandler : ViewHandler<GoogleMap, MapView>
{
    public static PropertyMapper<GoogleMap, GoogleMapHandler> GoogleMapHandlerMapper = new(ViewMapper)
    {

    };

    public static CommandMapper<GoogleMap, GoogleMapHandler> GoogleMapHandlerCommandMapper = new(ViewCommandMapper)
    {

    };

    public static Dictionary<string, Func<IMapFeatureManager<MapView>>> ManagerMapper { get; } = new()
    {
        { GoogleMap.MapManagerName, () => new MapManager() },
        { GoogleMap.CameraManagerName, () => new CameraManager() },
        { GoogleMap.UiSettingsManagerName, () => new UiSettingsManager() },
        { GoogleMap.PolylineManagerName, () => new PolylineManager() },
        { GoogleMap.PolygonManagerName, () => new PolygonManager() },
        { GoogleMap.CircleManagerName, () => new CircleManager() },
        { GoogleMap.TileOverlayManagerName, () => new HeatMapTileManager() },
        { GoogleMap.GroundOverlayManagerName, () => new GroundOverlayManager() },
        { GoogleMap.PinManagerName, () => new ClusterManager() },
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

    protected Dictionary<string, IMapFeatureManager<MapView>> Managers { get; set; } = [];

    protected override MapView CreatePlatformView()
    {
        return new MapView(new MapViewOptions() { Camera = new Google.Maps.CameraPosition(0f, 0f, 3f) });
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