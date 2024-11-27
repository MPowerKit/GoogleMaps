namespace MPowerKit.GoogleMaps;

public class IndoorBuilding
{
    public int ActiveLevelIndex { get; set; }
    public int DefaultLevelIndex { get; set; }
    public bool IsUnderground { get; set; }
    public required IList<IndoorLevel> Levels { get; set; }
}

public class IndoorLevel
{
    public required object NativeIndoorLevel { get; set; }
    public required string Name { get; set; }
    public required string ShortName { get; set; }

    public void Activate()
    {
#if ANDROID
        (NativeIndoorLevel as Android.Gms.Maps.Model.IndoorLevel)?.Activate();
#else
#endif
    }
}