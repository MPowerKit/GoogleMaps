using System.ComponentModel;

using GMap = Android.Gms.Maps.GoogleMap;

namespace MPowerKit.GoogleMaps;

public class UiSettingsManager : MapFeatureManager<GoogleMap, GMap, GoogleMapHandler>
{
    protected override void Init(GoogleMap virtualView, GMap platformView, GoogleMapHandler handler)
    {
        base.Init(virtualView, platformView, handler);

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

    protected override void VirtualViewPropertyChanged(GoogleMap virtualView, GMap platformView, string? propertyName)
    {
        base.VirtualViewPropertyChanged(virtualView, platformView, propertyName);

        if (propertyName == GoogleMap.CompassEnabledProperty.PropertyName)
        {
            OnCompassEnabledChanged(virtualView, platformView);
        }
        else if (propertyName == GoogleMap.MapToolbarEnabledProperty.PropertyName)
        {
            OnMapToolbarEnabledChanged(virtualView, platformView);
        }
        else if (propertyName == GoogleMap.ZoomControlsEnabledProperty.PropertyName)
        {
            OnZoomControlsEnabledChanged(virtualView, platformView);
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