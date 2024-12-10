using System.ComponentModel;

using Google.Maps;

namespace MPowerKit.GoogleMaps;

public class UiSettingsManager : IMapFeatureManager<GoogleMap, MapView, GoogleMapHandler>
{
    protected GoogleMap? VirtualView { get; set; }
    protected MapView? NativeView { get; set; }
    protected GoogleMapHandler? Handler { get; set; }

    public virtual void Connect(GoogleMap virtualView, MapView platformView, GoogleMapHandler handler)
    {
        VirtualView = virtualView;
        NativeView = platformView;
        Handler = handler;

        virtualView.PropertyChanged += VirtualView_PropertyChanged;
        virtualView.PropertyChanging += VirtualView_PropertyChanging;

        InitUiSettings();
    }

    public virtual void Disconnect(GoogleMap virtualView, MapView platformView, GoogleMapHandler handler)
    {
        virtualView.PropertyChanged -= VirtualView_PropertyChanged;
        virtualView.PropertyChanging -= VirtualView_PropertyChanging;

        VirtualView = null;
        NativeView = null;
        Handler = null;
    }

    protected virtual void InitUiSettings()
    {
        NativeView!.Settings.CompassButton = VirtualView!.CompassEnabled;
        NativeView.Settings.ZoomGestures = VirtualView.ZoomGesturesEnabled;
        NativeView.Settings.ScrollGestures = VirtualView.ScrollGesturesEnabled;
        NativeView.Settings.TiltGestures = VirtualView.TiltGesturesEnabled;
        NativeView.Settings.RotateGestures = VirtualView.RotateGesturesEnabled;
        NativeView.Settings.MyLocationButton = VirtualView.MyLocationButtonEnabled;
        NativeView.Settings.IndoorPicker = VirtualView.IndoorLevelPickerEnabled;
    }

    protected virtual void VirtualView_PropertyChanging(object sender, Microsoft.Maui.Controls.PropertyChangingEventArgs e)
    {

    }

    protected virtual void VirtualView_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == GoogleMap.CompassEnabledProperty.PropertyName)
        {
            NativeView!.Settings.CompassButton = VirtualView!.CompassEnabled;
        }
        else if (e.PropertyName == GoogleMap.ZoomGesturesEnabledProperty.PropertyName)
        {
            NativeView!.Settings.ZoomGestures = VirtualView!.ZoomGesturesEnabled;
        }
        else if (e.PropertyName == GoogleMap.ScrollGesturesEnabledProperty.PropertyName)
        {
            NativeView!.Settings.ScrollGestures = VirtualView!.ScrollGesturesEnabled;
        }
        else if (e.PropertyName == GoogleMap.TiltGesturesEnabledProperty.PropertyName)
        {
            NativeView!.Settings.TiltGestures = VirtualView!.TiltGesturesEnabled;
        }
        else if (e.PropertyName == GoogleMap.RotateGesturesEnabledProperty.PropertyName)
        {
            NativeView!.Settings.RotateGestures = VirtualView!.RotateGesturesEnabled;
        }
        else if (e.PropertyName == GoogleMap.MyLocationButtonEnabledProperty.PropertyName)
        {
            NativeView!.Settings.MyLocationButton = VirtualView!.MyLocationButtonEnabled;
        }
        else if (e.PropertyName == GoogleMap.IndoorLevelPickerEnabledProperty.PropertyName)
        {
            NativeView!.Settings.IndoorPicker = VirtualView!.IndoorLevelPickerEnabled;
        }
    }
}