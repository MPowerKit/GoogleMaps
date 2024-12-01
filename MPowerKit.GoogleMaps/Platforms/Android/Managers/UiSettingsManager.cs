using System.ComponentModel;

using GMap = Android.Gms.Maps.GoogleMap;

namespace MPowerKit.GoogleMaps;

public class UiSettingsManager : IMapFeatureManager<GoogleMap, GMap, GoogleMapHandler>
{
    protected GoogleMap? VirtualView { get; set; }
    protected GMap? NativeView { get; set; }
    protected GoogleMapHandler? Handler { get; set; }

    public void Connect(GoogleMap virtualView, GMap platformView, GoogleMapHandler handler)
    {
        VirtualView = virtualView;
        NativeView = platformView;
        Handler = handler;

        virtualView.PropertyChanged += VirtualView_PropertyChanged;
        virtualView.PropertyChanging += VirtualView_PropertyChanging;

        InitUiSettings();
    }

    public void Disconnect(GoogleMap virtualView, GMap platformView, GoogleMapHandler handler)
    {
        virtualView.PropertyChanged -= VirtualView_PropertyChanged;
        virtualView.PropertyChanging -= VirtualView_PropertyChanging;

        VirtualView = null;
        NativeView = null;
        Handler = null;
    }

    protected virtual void InitUiSettings()
    {
        NativeView!.UiSettings.CompassEnabled = VirtualView!.CompassEnabled;
        NativeView.UiSettings.MapToolbarEnabled = VirtualView.MapToolbarEnabled;
        NativeView.UiSettings.ZoomControlsEnabled = VirtualView.ZoomControlsEnabled;
        NativeView.UiSettings.ZoomGesturesEnabled = VirtualView.ZoomGesturesEnabled;
        NativeView.UiSettings.ScrollGesturesEnabled = VirtualView.ScrollGesturesEnabled;
        NativeView.UiSettings.TiltGesturesEnabled = VirtualView.TiltGesturesEnabled;
        NativeView.UiSettings.RotateGesturesEnabled = VirtualView.RotateGesturesEnabled;
        NativeView.UiSettings.MyLocationButtonEnabled = VirtualView.MyLocationButtonEnabled;
        NativeView.UiSettings.IndoorLevelPickerEnabled = VirtualView.IndoorLevelPickerEnabled;
    }

    protected virtual void VirtualView_PropertyChanging(object sender, Microsoft.Maui.Controls.PropertyChangingEventArgs e)
    {

    }

    protected virtual void VirtualView_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == GoogleMap.CompassEnabledProperty.PropertyName)
        {
            NativeView!.UiSettings.CompassEnabled = VirtualView!.CompassEnabled;
        }
        else if (e.PropertyName == GoogleMap.MapToolbarEnabledProperty.PropertyName)
        {
            NativeView!.UiSettings.MapToolbarEnabled = VirtualView!.MapToolbarEnabled;
        }
        else if (e.PropertyName == GoogleMap.ZoomControlsEnabledProperty.PropertyName)
        {
            NativeView!.UiSettings.ZoomControlsEnabled = VirtualView!.ZoomControlsEnabled;
        }
        else if (e.PropertyName == GoogleMap.ZoomGesturesEnabledProperty.PropertyName)
        {
            NativeView!.UiSettings.ZoomGesturesEnabled = VirtualView!.ZoomGesturesEnabled;
        }
        else if (e.PropertyName == GoogleMap.ScrollGesturesEnabledProperty.PropertyName)
        {
            NativeView!.UiSettings.ScrollGesturesEnabled = VirtualView!.ScrollGesturesEnabled;
        }
        else if (e.PropertyName == GoogleMap.TiltGesturesEnabledProperty.PropertyName)
        {
            NativeView!.UiSettings.TiltGesturesEnabled = VirtualView!.TiltGesturesEnabled;
        }
        else if (e.PropertyName == GoogleMap.RotateGesturesEnabledProperty.PropertyName)
        {
            NativeView!.UiSettings.RotateGesturesEnabled = VirtualView!.RotateGesturesEnabled;
        }
        else if (e.PropertyName == GoogleMap.MyLocationButtonEnabledProperty.PropertyName)
        {
            NativeView!.UiSettings.MyLocationButtonEnabled = VirtualView!.MyLocationButtonEnabled;
        }
        else if (e.PropertyName == GoogleMap.IndoorLevelPickerEnabledProperty.PropertyName)
        {
            NativeView!.UiSettings.IndoorLevelPickerEnabled = VirtualView!.IndoorLevelPickerEnabled;
        }
    }
}