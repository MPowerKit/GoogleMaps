using System.ComponentModel;

using Google.Maps;

namespace MPowerKit.GoogleMaps;

public class UiSettingsManager : MapFeatureManager<GoogleMap, MapView, GoogleMapHandler>
{
    protected override void Init(GoogleMap virtualView, MapView platformView, GoogleMapHandler handler)
    {
        base.Init(virtualView, platformView, handler);

        OnCompassEnabledChanged(virtualView, platformView);
        OnZoomGesturesEnabledChanged(virtualView, platformView);
        OnScrollGesturesEnabledChanged(virtualView, platformView);
        OnTiltGesturesEnabledChanged(virtualView, platformView);
        OnRotateGesturesEnabledChanged(virtualView, platformView);
        OnMyLocationButtonEnabledChanged(virtualView, platformView);
        OnIndoorLevelPickerEnabledChanged(virtualView, platformView);
    }

    protected override void VirtualViewPropertyChanged(GoogleMap virtualView, MapView platformView, string? propertyName)
    {
        base.VirtualViewPropertyChanged(virtualView, platformView, propertyName);

        if (propertyName == GoogleMap.CompassEnabledProperty.PropertyName)
        {
            OnCompassEnabledChanged(virtualView, platformView);
        }
        else if (propertyName == GoogleMap.ZoomGesturesEnabledProperty.PropertyName)
        {
            OnZoomGesturesEnabledChanged(virtualView, platformView);
        }
        else if (propertyName == GoogleMap.ScrollGesturesEnabledProperty.PropertyName)
        {
            OnScrollGesturesEnabledChanged(virtualView, platformView);
        }
        else if (propertyName == GoogleMap.TiltGesturesEnabledProperty.PropertyName)
        {
            OnTiltGesturesEnabledChanged(virtualView, platformView);
        }
        else if (propertyName == GoogleMap.RotateGesturesEnabledProperty.PropertyName)
        {
            OnRotateGesturesEnabledChanged(virtualView, platformView);
        }
        else if (propertyName == GoogleMap.MyLocationButtonEnabledProperty.PropertyName)
        {
            OnMyLocationButtonEnabledChanged(virtualView, platformView);
        }
        else if (propertyName == GoogleMap.IndoorLevelPickerEnabledProperty.PropertyName)
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