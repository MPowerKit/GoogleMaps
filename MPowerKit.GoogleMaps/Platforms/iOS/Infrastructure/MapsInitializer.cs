using Google.Maps;

namespace MPowerKit.GoogleMaps;

public class MapsInitializer
{
    public static bool IsInitialized { get; private set; }

    public static void Init(string apiKey)
    {
        if (IsInitialized) return;

        MapServices.ProvideApiKey(apiKey);
        IsInitialized = true;
    }
}