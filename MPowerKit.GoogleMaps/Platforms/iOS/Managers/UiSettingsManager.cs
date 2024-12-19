using System.ComponentModel;

using Google.Maps;

namespace MPowerKit.GoogleMaps;

public class UiSettingsManager : IMapFeatureManager<GoogleMap, MapView, GoogleMapHandler>
{
    protected GoogleMap? VirtualView { get; set; }
    protected MapView? PlatformView { get; set; }
    protected GoogleMapHandler? Handler { get; set; }

    public virtual void Connect(GoogleMap virtualView, MapView platformView, GoogleMapHandler handler)
    {
        VirtualView = virtualView;
        PlatformView = platformView;
        Handler = handler;

        InitUiSettings(virtualView, platformView, handler);

        SubscribeToEvents(virtualView, platformView, handler);
    }

    public virtual void Disconnect(GoogleMap virtualView, MapView platformView, GoogleMapHandler handler)
    {
        UnsubscribeFromEvents(virtualView, platformView, handler);

        VirtualView = null;
        PlatformView = null;
        Handler = null;
    }

    protected virtual void InitUiSettings(GoogleMap virtualView, MapView platformView, GoogleMapHandler handler)
    {
        OnCompassEnabledChanged(virtualView, platformView);
        OnZoomGesturesEnabledChanged(virtualView, platformView);
        OnScrollGesturesEnabledChanged(virtualView, platformView);
        OnTiltGesturesEnabledChanged(virtualView, platformView);
        OnRotateGesturesEnabledChanged(virtualView, platformView);
        OnMyLocationButtonEnabledChanged(virtualView, platformView);
        OnIndoorLevelPickerEnabledChanged(virtualView, platformView);
    }

    protected virtual void SubscribeToEvents(GoogleMap virtualView, MapView platformView, GoogleMapHandler handler)
    {
        virtualView.PropertyChanged += VirtualView_PropertyChanged;
        virtualView.PropertyChanging += VirtualView_PropertyChanging;
    }

    protected virtual void UnsubscribeFromEvents(GoogleMap virtualView, MapView platformView, GoogleMapHandler handler)
    {
        virtualView.PropertyChanged -= VirtualView_PropertyChanged;
        virtualView.PropertyChanging -= VirtualView_PropertyChanging;
    }

    protected virtual void VirtualView_PropertyChanging(object sender, Microsoft.Maui.Controls.PropertyChangingEventArgs e)
    {

    }

    protected virtual void VirtualView_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        var virtualView = VirtualView!;
        var platformView = PlatformView!;

        if (e.PropertyName == GoogleMap.CompassEnabledProperty.PropertyName)
        {
            OnCompassEnabledChanged(virtualView, platformView);
        }
        else if (e.PropertyName == GoogleMap.ZoomGesturesEnabledProperty.PropertyName)
        {
            OnZoomGesturesEnabledChanged(virtualView, platformView);
        }
        else if (e.PropertyName == GoogleMap.ScrollGesturesEnabledProperty.PropertyName)
        {
            OnScrollGesturesEnabledChanged(virtualView, platformView);
        }
        else if (e.PropertyName == GoogleMap.TiltGesturesEnabledProperty.PropertyName)
        {
            OnTiltGesturesEnabledChanged(virtualView, platformView);
        }
        else if (e.PropertyName == GoogleMap.RotateGesturesEnabledProperty.PropertyName)
        {
            OnRotateGesturesEnabledChanged(virtualView, platformView);
        }
        else if (e.PropertyName == GoogleMap.MyLocationButtonEnabledProperty.PropertyName)
        {
            OnMyLocationButtonEnabledChanged(virtualView, platformView);
        }
        else if (e.PropertyName == GoogleMap.IndoorLevelPickerEnabledProperty.PropertyName)
        {
            OnIndoorLevelPickerEnabledChanged(virtualView, platformView);
        }
    }

    protected virtual void OnCompassEnabledChanged(GoogleMap virtualView, MapView platformView)
    {
        platformView.Settings.CompassButton = virtualView.CompassEnabled;
    }

    protected virtual void OnZoomGesturesEnabledChanged(GoogleMap virtualView, MapView platformView)
    {
        platformView.Settings.ZoomGestures = virtualView.ZoomGesturesEnabled;
    }

    protected virtual void OnScrollGesturesEnabledChanged(GoogleMap virtualView, MapView platformView)
    {
        platformView.Settings.ScrollGestures = virtualView.ScrollGesturesEnabled;
    }

    protected virtual void OnTiltGesturesEnabledChanged(GoogleMap virtualView, MapView platformView)
    {
        platformView.Settings.TiltGestures = virtualView.TiltGesturesEnabled;
    }

    protected virtual void OnRotateGesturesEnabledChanged(GoogleMap virtualView, MapView platformView)
    {
        platformView.Settings.RotateGestures = virtualView.RotateGesturesEnabled;
    }

    protected virtual void OnMyLocationButtonEnabledChanged(GoogleMap virtualView, MapView platformView)
    {
        platformView.Settings.MyLocationButton = virtualView.MyLocationButtonEnabled;
    }

    protected virtual void OnIndoorLevelPickerEnabledChanged(GoogleMap virtualView, MapView platformView)
    {
        platformView.Settings.IndoorPicker = virtualView.IndoorLevelPickerEnabled;
    }
}