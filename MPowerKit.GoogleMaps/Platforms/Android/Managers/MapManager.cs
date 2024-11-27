using System.ComponentModel;

using Microsoft.Maui.Platform;

using GMap = Android.Gms.Maps.GoogleMap;

namespace MPowerKit.GoogleMaps;

public class MapManager : IMapFeatureManager<GoogleMap, GMap, GoogleMapHandler>
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

        VirtualView!.MapCoordsToScreenLocationFuncInternal = MapCoordsToScreenLocation;
        VirtualView.ScreenLocationToMapCoordsFuncInternal = ScreenLocationToMapCoords;

        NativeView.MapClick += NativeMap_MapClick;
        NativeView.MapLongClick += NativeMap_MapLongClick;
        NativeView.MapCapabilitiesChanged += NativeView_MapCapabilitiesChanged;
        NativeView.IndoorLevelActivated += NativeView_IndoorLevelActivated;
        NativeView.IndoorBuildingFocused += NativeView_IndoorBuildingFocused;

        HandlePoiClick();

        InitMap();
    }

    public void Disconnect(GoogleMap virtualView, GMap platformView, GoogleMapHandler handler)
    {
        VirtualView!.MapCoordsToScreenLocationFuncInternal = null;
        VirtualView.ScreenLocationToMapCoordsFuncInternal = null;

        NativeView!.MapClick -= NativeMap_MapClick;
        NativeView.MapLongClick -= NativeMap_MapLongClick;
        NativeView.PoiClick -= NativeMap_PoiClick;
        NativeView.MapCapabilitiesChanged -= NativeView_MapCapabilitiesChanged;
        NativeView.IndoorLevelActivated -= NativeView_IndoorLevelActivated;
        NativeView.IndoorBuildingFocused -= NativeView_IndoorBuildingFocused;

        virtualView.PropertyChanged -= VirtualView_PropertyChanged;
        virtualView.PropertyChanging -= VirtualView_PropertyChanging;

        VirtualView = null;
        NativeView = null;
        Handler = null;
    }

    protected virtual void InitMap()
    {
        VirtualView!.SendMapCapabilitiesChanged(NativeView!.MapCapabilities.ToCrossPlatform());
        VirtualView!.SendIndoorBuildingFocused(NativeView!.FocusedBuilding?.ToCrossPlatform());
        NativeView.SetIndoorEnabled(VirtualView!.IndoorEnabled);
        NativeView.BuildingsEnabled = VirtualView!.BuildingsEnabled;
        NativeView.MapType = (int)VirtualView!.MapType;
        NativeView.MyLocationEnabled = VirtualView!.MyLocationEnabled;
        NativeView.MapColorScheme = (int)VirtualView!.MapColorScheme;
        NativeView.TrafficEnabled = VirtualView!.TrafficEnabled;
        var context = Handler!.Context;
        NativeView.SetPadding((int)context.ToPixels(VirtualView!.Padding.Left), (int)context.ToPixels(VirtualView!.Padding.Top), (int)context.ToPixels(VirtualView!.Padding.Right), (int)context.ToPixels(VirtualView!.Padding.Bottom));

        SetMapStyle();
    }

    protected virtual void VirtualView_PropertyChanging(object sender, Microsoft.Maui.Controls.PropertyChangingEventArgs e)
    {

    }

    protected virtual void VirtualView_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == GoogleMap.HandlePoiClickProperty.PropertyName)
        {
            HandlePoiClick();
        }
        else if (e.PropertyName == GoogleMap.IndoorEnabledProperty.PropertyName)
        {
            NativeView!.SetIndoorEnabled(VirtualView!.IndoorEnabled);
        }
        else if (e.PropertyName == GoogleMap.BuildingsEnabledProperty.PropertyName)
        {
            NativeView!.BuildingsEnabled = VirtualView!.BuildingsEnabled;
        }
        else if (e.PropertyName == GoogleMap.MapTypeProperty.PropertyName)
        {
            NativeView!.MapType = (int)VirtualView!.MapType;
        }
        else if (e.PropertyName == GoogleMap.MyLocationEnabledProperty.PropertyName)
        {
            NativeView!.MyLocationEnabled = VirtualView!.MyLocationEnabled;
        }
        else if (e.PropertyName == GoogleMap.MapColorSchemeProperty.PropertyName)
        {
            NativeView!.MapColorScheme = (int)VirtualView!.MapColorScheme;
        }
        else if (e.PropertyName == GoogleMap.TrafficEnabledProperty.PropertyName)
        {
            NativeView!.TrafficEnabled = VirtualView!.TrafficEnabled;
        }
        else if (e.PropertyName == GoogleMap.PaddingProperty.PropertyName)
        {
            var context = Handler!.Context;
            NativeView!.SetPadding((int)context.ToPixels(VirtualView!.Padding.Left), (int)context.ToPixels(VirtualView!.Padding.Top), (int)context.ToPixels(VirtualView!.Padding.Right), (int)context.ToPixels(VirtualView!.Padding.Bottom));
        }
        else if (e.PropertyName == GoogleMap.MapStyleJsonFileNameProperty.PropertyName)
        {
            SetMapStyle();
        }
    }

    protected virtual void SetMapStyle()
    {
        if (string.IsNullOrWhiteSpace(VirtualView!.MapStyleJsonFileName)) return;

        var fileName = VirtualView!.MapStyleJsonFileName
            .Replace('\\', Path.DirectorySeparatorChar)
            .Replace('/', Path.DirectorySeparatorChar);

        using var stream = Android.App.Application.Context.Assets!.Open(fileName);
        using var reader = new StreamReader(stream);
        var json = reader.ReadToEnd();

        NativeView!.SetMapStyle(new Android.Gms.Maps.Model.MapStyleOptions(json));
    }

    protected virtual Point ScreenLocationToMapCoords(Point point)
    {
        return NativeView!.Projection.FromScreenLocation(point.ToNativePoint(Handler!.Context)).ToCrossPlatformPoint();
    }

    protected virtual Point MapCoordsToScreenLocation(Point point)
    {
        return NativeView!.Projection.ToScreenLocation(point.ToLatLng()).ToCrossPlatformPoint(Handler!.Context);
    }

    protected virtual void NativeMap_MapClick(object? sender, GMap.MapClickEventArgs e)
    {
        VirtualView!.SendMapClick(e.Point.ToCrossPlatformPoint());
    }

    protected virtual void NativeMap_MapLongClick(object? sender, GMap.MapLongClickEventArgs e)
    {
        VirtualView!.SendMapLongClick(e.Point.ToCrossPlatformPoint());
    }

    protected virtual void NativeMap_PoiClick(object? sender, GMap.PoiClickEventArgs e)
    {
        if (e.Poi is null) return;

        VirtualView!.SendPoiClick(e.Poi.ToCrossPlatform());
    }

    protected virtual void HandlePoiClick()
    {
        if (VirtualView!.HandlePoiClick)
        {
            NativeView!.PoiClick += NativeMap_PoiClick;
        }
        else
        {
            NativeView!.PoiClick -= NativeMap_PoiClick;
        }
    }

    protected virtual void NativeView_MapCapabilitiesChanged(object? sender, GMap.MapCapabilitiesChangedEventArgs e)
    {
        VirtualView!.SendMapCapabilitiesChanged(e.P0.ToCrossPlatform());
    }

    protected virtual void NativeView_IndoorBuildingFocused(object? sender, EventArgs e)
    {
        VirtualView!.SendIndoorBuildingFocused(NativeView!.FocusedBuilding?.ToCrossPlatform());
    }

    protected virtual void NativeView_IndoorLevelActivated(object? sender, GMap.IndoorLevelActivatedEventArgs e)
    {
        VirtualView!.SendIndoorLevelActivated(NativeView!.FocusedBuilding?.ToCrossPlatform());
    }
}