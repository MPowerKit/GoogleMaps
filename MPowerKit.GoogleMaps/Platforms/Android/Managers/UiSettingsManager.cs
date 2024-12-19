using System.ComponentModel;

using GMap = Android.Gms.Maps.GoogleMap;

namespace MPowerKit.GoogleMaps;

public class UiSettingsManager : IMapFeatureManager<GoogleMap, GMap, GoogleMapHandler>
{
    protected GoogleMap? VirtualView { get; set; }
    protected GMap? PlatformView { get; set; }
    protected GoogleMapHandler? Handler { get; set; }

    public virtual void Connect(GoogleMap virtualView, GMap platformView, GoogleMapHandler handler)
    {
        VirtualView = virtualView;
        PlatformView = platformView;
        Handler = handler;

        InitUiSettings(virtualView, platformView, handler);

        SubscribeToEvents(virtualView, platformView, handler);
    }

    public virtual void Disconnect(GoogleMap virtualView, GMap platformView, GoogleMapHandler handler)
    {
        UnsubscribeFromEvents(virtualView, platformView, handler);

        VirtualView = null;
        PlatformView = null;
        Handler = null;
    }

    protected virtual void InitUiSettings(GoogleMap virtualView, GMap platformView, GoogleMapHandler handler)
    {
        OnCompassEnabledChanged(virtualView, platformView);
        OnMapToolbarEnabledChanged(virtualView, platformView);
        OnZoomControlsEnabledChanged(virtualView, platformView);
        OnZoomGesturesEnabledChanged(virtualView, platformView);
        OnScrollGesturesEnabledChanged(virtualView, platformView);
        OnTiltGesturesEnabledChanged(virtualView, platformView);
        OnRotateGesturesEnabledChanged(virtualView, platformView);
        OnMyLocationButtonEnabledChanged(virtualView, platformView);
        OnIndoorLevelPickerEnabledChanged(virtualView, platformView);
    }

    protected virtual void SubscribeToEvents(GoogleMap virtualView, GMap platformView, GoogleMapHandler handler)
    {
        virtualView.PropertyChanged += VirtualView_PropertyChanged;
        virtualView.PropertyChanging += VirtualView_PropertyChanging;
    }

    protected virtual void UnsubscribeFromEvents(GoogleMap virtualView, GMap platformView, GoogleMapHandler handler)
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
        else if (e.PropertyName == GoogleMap.MapToolbarEnabledProperty.PropertyName)
        {
            OnMapToolbarEnabledChanged(virtualView, platformView);
        }
        else if (e.PropertyName == GoogleMap.ZoomControlsEnabledProperty.PropertyName)
        {
            OnZoomControlsEnabledChanged(virtualView, platformView);
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

    protected virtual void OnCompassEnabledChanged(GoogleMap virtualView, GMap platformView)
    {
        platformView.UiSettings.CompassEnabled = virtualView.CompassEnabled;
    }

    protected virtual void OnMapToolbarEnabledChanged(GoogleMap virtualView, GMap platformView)
    {
        platformView.UiSettings.MapToolbarEnabled = virtualView.MapToolbarEnabled;
    }

    protected virtual void OnZoomControlsEnabledChanged(GoogleMap virtualView, GMap platformView)
    {
        platformView.UiSettings.ZoomControlsEnabled = virtualView.ZoomControlsEnabled;
    }

    protected virtual void OnZoomGesturesEnabledChanged(GoogleMap virtualView, GMap platformView)
    {
        platformView.UiSettings.ZoomGesturesEnabled = virtualView.ZoomGesturesEnabled;
    }

    protected virtual void OnScrollGesturesEnabledChanged(GoogleMap virtualView, GMap platformView)
    {
        platformView.UiSettings.ScrollGesturesEnabled = virtualView.ScrollGesturesEnabled;
    }

    protected virtual void OnTiltGesturesEnabledChanged(GoogleMap virtualView, GMap platformView)
    {
        platformView.UiSettings.TiltGesturesEnabled = virtualView.TiltGesturesEnabled;
    }

    protected virtual void OnRotateGesturesEnabledChanged(GoogleMap virtualView, GMap platformView)
    {
        platformView.UiSettings.RotateGesturesEnabled = virtualView.RotateGesturesEnabled;
    }

    protected virtual void OnMyLocationButtonEnabledChanged(GoogleMap virtualView, GMap platformView)
    {
        platformView.UiSettings.MyLocationButtonEnabled = virtualView.MyLocationButtonEnabled;
    }

    protected virtual void OnIndoorLevelPickerEnabledChanged(GoogleMap virtualView, GMap platformView)
    {
        platformView.UiSettings.IndoorLevelPickerEnabled = virtualView.IndoorLevelPickerEnabled;
    }
}