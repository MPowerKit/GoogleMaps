using Microsoft.Maui.LifecycleEvents;

namespace MPowerKit.GoogleMaps;

public static class BuilderExtensions
{
    public static MauiAppBuilder UseMPowerKitGoogleMaps(this MauiAppBuilder builder
#if IOS
        , string iosApiKey
#endif
        )
    {
        builder
            .ConfigureLifecycleEvents(events =>
            {
#if ANDROID
                events.AddAndroid(android =>
                {
                    var service = IPlatformApplication.Current?.Services.GetRequiredService<IMapsLifecycle>();
                    android.OnCreate((a, b) =>
                    {
                        if (a is not MauiAppCompatActivity) return;

                        MapsInitializer.Init(a);

                        IPlatformApplication.Current?.Services.GetRequiredService<IMapsLifecycle>()?.SendOnCreate(b);
                    })
                    .OnResume(a =>
                    {
                        if (a is not MauiAppCompatActivity) return;

                        IPlatformApplication.Current?.Services.GetRequiredService<IMapsLifecycle>()?.SendOnResume();
                    })
                    .OnStart(a =>
                    {
                        if (a is not MauiAppCompatActivity) return;

                        IPlatformApplication.Current?.Services.GetRequiredService<IMapsLifecycle>()?.SendOnStart();
                    })
                    .OnDestroy(a =>
                    {
                        if (a is not MauiAppCompatActivity) return;

                        IPlatformApplication.Current?.Services.GetRequiredService<IMapsLifecycle>()?.SendOnDestroy();
                    })
                    .OnStop(a =>
                    {
                        if (a is not MauiAppCompatActivity) return;

                        IPlatformApplication.Current?.Services.GetRequiredService<IMapsLifecycle>()?.SendOnStop();
                    })
                    .OnSaveInstanceState((a, b) =>
                    {
                        if (a is not MauiAppCompatActivity) return;

                        IPlatformApplication.Current?.Services.GetRequiredService<IMapsLifecycle>()?.SendOnSaveInstanceState(b);
                    })
                    .OnApplicationLowMemory(a =>
                    {
                        IPlatformApplication.Current?.Services.GetRequiredService<IMapsLifecycle>()?.SendOnLowMemory();
                    });
                });
#endif

#if IOS
                events.AddiOS(b => b.FinishedLaunching((a, o) =>
                {
                    MapsInitializer.Init(iosApiKey);
                    return true;
                }));
#endif
            })
            .ConfigureMauiHandlers(h => h.AddHandler<GoogleMap, GoogleMapHandler>())
            .ConfigureImageSources(static services =>
            {
#if ANDROID
                services.AddService<IViewImageSource, ViewImageSourceService>();
                services.AddService<ViewImageSource, ViewImageSourceService>();
#endif
            });

#if ANDROID
        builder.Services.AddSingleton<IMapsLifecycle, MapsLifecycle>();
#endif

        return builder;
    }
}