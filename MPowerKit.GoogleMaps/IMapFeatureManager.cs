namespace MPowerKit.GoogleMaps;

public interface IMapFeatureManager<TVirtualView, TPlatformView, TPlatformHandler>
{
    void Connect(TVirtualView virtualView, TPlatformView platformView, TPlatformHandler handler);
    void Disconnect(TVirtualView virtualView, TPlatformView platformView, TPlatformHandler handler);
}