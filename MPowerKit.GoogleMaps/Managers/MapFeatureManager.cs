using System.ComponentModel;

namespace MPowerKit.GoogleMaps;

public abstract class MapFeatureManager<TNMap> : IMapFeatureManager<TNMap>
{
    protected GoogleMap? VirtualView { get; set; }
    protected TNMap? PlatformView { get; set; }
    protected GoogleMapHandler? Handler { get; set; }

    public virtual void Connect(GoogleMap virtualView, TNMap platformView, GoogleMapHandler handler)
    {
        VirtualView = virtualView;
        PlatformView = platformView;
        Handler = handler;

        Init(virtualView, platformView, handler);

        SubscribeToEvents(virtualView, platformView, handler);
    }

    public virtual void Disconnect(GoogleMap virtualView, TNMap platformView, GoogleMapHandler handler)
    {
        UnsubscribeFromEvents(virtualView, platformView, handler);

        Reset(virtualView, platformView, handler);

        VirtualView = null;
        PlatformView = default;
        Handler = null;
    }

    protected virtual void Init(GoogleMap virtualView, TNMap platformView, GoogleMapHandler handler)
    {

    }

    protected virtual void Reset(GoogleMap virtualView, TNMap platformView, GoogleMapHandler handler)
    {

    }

    protected virtual void SubscribeToEvents(GoogleMap virtualView, TNMap platformView, GoogleMapHandler handler)
    {
        virtualView.PropertyChanged += VirtualView_PropertyChanged;
        virtualView.PropertyChanging += VirtualView_PropertyChanging;
    }

    protected virtual void UnsubscribeFromEvents(GoogleMap virtualView, TNMap platformView, GoogleMapHandler handler)
    {
        virtualView.PropertyChanged -= VirtualView_PropertyChanged;
        virtualView.PropertyChanging -= VirtualView_PropertyChanging;
    }

    protected virtual void VirtualView_PropertyChanging(object sender, Microsoft.Maui.Controls.PropertyChangingEventArgs e)
    {
        VirtualViewPropertyChanging(VirtualView!, PlatformView!, e.PropertyName);
    }

    protected virtual void VirtualViewPropertyChanging(GoogleMap virtualView, TNMap platformView, string? propertyName)
    {

    }

    protected virtual void VirtualView_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        VirtualViewPropertyChanged(VirtualView!, PlatformView!, e.PropertyName);
    }

    protected virtual void VirtualViewPropertyChanged(GoogleMap virtualView, TNMap platformView, string? propertyName)
    {

    }
}