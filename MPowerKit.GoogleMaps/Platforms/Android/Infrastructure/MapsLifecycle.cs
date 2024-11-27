using Android.OS;

namespace MPowerKit.GoogleMaps;

public class MapsLifecycle : IMapsLifecycle
{
    protected Bundle? Bundle;

    public event Action<Bundle?>? OnCreate;
    public event Action? OnStart;
    public event Action? OnResume;
    public event Action? OnPause;
    public event Action? OnStop;
    public event Action? OnDestroy;
    public event Action? OnLowMemory;
    public event Action<Bundle>? OnSaveInstanceState;

    public virtual Bundle GetBundleFromOnCreate()
    {
        return Bundle!;
    }

    public virtual void SendOnCreate(Bundle? bundle)
    {
        Bundle = bundle;

        OnCreate?.Invoke(bundle);
    }

    public virtual void SendOnResume()
    {
        OnResume?.Invoke();
    }

    public virtual void SendOnPause()
    {
        OnPause?.Invoke();
    }

    public virtual void SendOnStart()
    {
        OnStart?.Invoke();
    }

    public virtual void SendOnStop()
    {
        OnStop?.Invoke();
    }

    public virtual void SendOnDestroy()
    {
        OnDestroy?.Invoke();
    }

    public virtual void SendOnLowMemory()
    {
        OnLowMemory?.Invoke();
    }

    public virtual void SendOnSaveInstanceState(Bundle bundle)
    {
        OnSaveInstanceState?.Invoke(bundle);
    }
}