namespace MPowerKit.GoogleMaps.Utils;

public static class BuilderExtensions
{
    public static MauiAppBuilder UseMPowerKitGoogleMapsUtils(this MauiAppBuilder builder
#if IOS
        , string iosApiKey
#endif
        )
    {
        builder.UseMPowerKitGoogleMaps
        (
#if IOS
            iosApiKey
#endif
        );

#if ANDROID || IOS
        GoogleMapHandler.ManagerMapper[nameof(TileOverlayManager)] = () => new HeatMapTileManager();
#endif
        return builder;
    }
}