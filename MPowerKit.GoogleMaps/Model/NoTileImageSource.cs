namespace MPowerKit.GoogleMaps;

public sealed class NoTileImageSource : ImageSource
{
    public override bool IsEmpty => true;

    public static readonly ImageSource Instance = new NoTileImageSource();
}