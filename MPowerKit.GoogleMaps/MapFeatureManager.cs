using System.ComponentModel;

namespace MPowerKit.GoogleMaps;

public abstract class MapFeatureManager<TVMap, TNMap, THandler>
    : IMapFeatureManager<TVMap, TNMap, THandler>
    where TVMap : GoogleMap
    where TNMap : class
    where THandler :
#if ANDROID || IOS
        GoogleMapHandler
#else
        class
#endif
{
    protected TVMap? VirtualView { get; set; }
    protected TNMap? PlatformView { get; set; }
    protected THandler? Handler { get; set; }

    public virtual void Connect(TVMap virtualView, TNMap platformView, THandler handler)
    {
        VirtualView = virtualView;
        PlatformView = platformView;
        Handler = handler;

        Init(virtualView, platformView, handler);

        SubscribeToEvents(virtualView, platformView, handler);
    }

    public virtual void Disconnect(TVMap virtualView, TNMap platformView, THandler handler)
    {
        UnsubscribeFromEvents(virtualView, platformView, handler);

        Reset(virtualView, platformView, handler);

        VirtualView = null;
        PlatformView = null;
        Handler = null;
    }

    protected virtual void Init(TVMap virtualView, TNMap platformView, THandler handler)
    {

    }

    protected virtual void Reset(TVMap virtualView, TNMap platformView, THandler handler)
    {

    }

    protected virtual void SubscribeToEvents(TVMap virtualView, TNMap platformView, THandler handler)
    {
        virtualView.PropertyChanged += VirtualView_PropertyChanged;
        virtualView.PropertyChanging += VirtualView_PropertyChanging;
    }

    protected virtual void UnsubscribeFromEvents(TVMap virtualView, TNMap platformView, THandler handler)
    {
        virtualView.PropertyChanged -= VirtualView_PropertyChanged;
        virtualView.PropertyChanging -= VirtualView_PropertyChanging;
    }

    protected virtual void VirtualView_PropertyChanging(object sender, Microsoft.Maui.Controls.PropertyChangingEventArgs e)
    {
        VirtualViewPropertyChanging(VirtualView!, PlatformView!, e.PropertyName);
    }

    protected virtual void VirtualViewPropertyChanging(TVMap virtualView, TNMap platformView, string? propertyName)
    {

    }

    protected virtual void VirtualView_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        VirtualViewPropertyChanged(VirtualView!, PlatformView!, e.PropertyName);
    }

    protected virtual void VirtualViewPropertyChanged(TVMap virtualView, TNMap platformView, string? propertyName)
    {

    }
}