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
        { "Map", () => new MapManager() },
        //{ "Camera", () => new CameraManager() },
        //{ nameof(GMap.UiSettings), () => new UiSettingsManager() },
        { nameof(GoogleMap.Polylines), () => new PolylineManager() },
        { nameof(GoogleMap.Polygons), () => new PolygonManager() },
        { nameof(GoogleMap.Circles), () => new CircleManager() },
        { nameof(GoogleMap.TileOverlays), () => new TileOverlayManager() },
        { nameof(GoogleMap.GroundOverlays), () => new GroundOverlayManager() },
        { nameof(GoogleMap.Pins), () => new PinManager() },
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