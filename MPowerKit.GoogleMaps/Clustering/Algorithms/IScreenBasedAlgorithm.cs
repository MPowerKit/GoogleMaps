namespace MPowerKit.GoogleMaps;

public interface IScreenBasedAlgorithm : IAlgorithm
{
    bool ShouldReclusterOnMapMovement { get; }

    void OnCameraChange(CameraPosition position);
}