namespace MPowerKit.GoogleMaps;

public interface IMapFeatureManager<TN>
{
    void Connect(GoogleMap virtualView, TN platformView, GoogleMapHandler handler);
    void Disconnect(GoogleMap virtualView, TN platformView, GoogleMapHandler handler);
}