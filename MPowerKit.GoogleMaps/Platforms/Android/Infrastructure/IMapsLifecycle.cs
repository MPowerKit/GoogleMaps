using Android.OS;

namespace MPowerKit.GoogleMaps;

public interface IMapsLifecycle
{
    event Action<Bundle?>? OnCreate;
    event Action? OnStart;
    event Action? OnResume;
    event Action? OnPause;
    event Action? OnStop;
    event Action? OnDestroy;
    event Action? OnLowMemory;
    event Action<Bundle>? OnSaveInstanceState;

    Bundle GetBundleFromOnCreate();
    void SendOnCreate(Bundle? bundle);
    void SendOnResume();
    void SendOnPause();
    void SendOnStart();
    void SendOnStop();
    void SendOnDestroy();
    void SendOnLowMemory();
    void SendOnSaveInstanceState(Bundle bundle);
}