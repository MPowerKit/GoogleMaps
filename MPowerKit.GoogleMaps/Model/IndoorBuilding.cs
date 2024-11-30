namespace MPowerKit.GoogleMaps;

public class IndoorBuilding
{
    public int DefaultLevelIndex { get; set; }
    public bool IsUnderground { get; set; }
    public required IList<IndoorLevel> Levels { get; set; }
}

public class IndoorLevel
{
#if IOS
    public required Google.Maps.MapView NativeMap { get; set; }
#endif
    public required object NativeIndoorLevel { get; set; }
    public required string Name { get; set; }
    public required string ShortName { get; set; }

    public void Activate()
    {
#if ANDROID
        (NativeIndoorLevel as Android.Gms.Maps.Model.IndoorLevel)?.Activate();
#else
        //NativeMap.IndoorDisplay.ActiveLevel = (NativeIndoorLevel as Google.Maps.IndoorLevel);
#endif
    }
}