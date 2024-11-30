using System.ComponentModel;

using Foundation;

using Google.Maps;

using UIKit;

using Path = System.IO.Path;

namespace MPowerKit.GoogleMaps;

public class MapManager : NSObject, IMapFeatureManager<GoogleMap, MapView, GoogleMapHandler>, IIndoorDisplayDelegate
{
    protected GoogleMap? VirtualView { get; set; }
    protected MapView? NativeView { get; set; }
    protected GoogleMapHandler? Handler { get; set; }

    public void Connect(GoogleMap virtualView, MapView platformView, GoogleMapHandler handler)
    {
        VirtualView = virtualView;
        NativeView = platformView;
        Handler = handler;

        virtualView.PropertyChanged += VirtualView_PropertyChanged;
        virtualView.PropertyChanging += VirtualView_PropertyChanging;

        VirtualView!.MapCoordsToScreenLocationFuncInternal = MapCoordsToScreenLocation;
        VirtualView.ScreenLocationToMapCoordsFuncInternal = ScreenLocationToMapCoords;

        platformView.CoordinateTapped += PlatformView_CoordinateTapped;
        platformView.CoordinateLongPressed += PlatformView_CoordinateLongPressed;
        platformView.PoiWithPlaceIdTapped += PlatformView_PoiWithPlaceIdTapped;
        platformView.IndoorDisplay.Delegate = this;
        platformView.MapCapabilitiesChanged += PlatformView_MapCapabilitiesChanged;

        HandlePoiClick();

        InitMap();
    }

    public void Disconnect(GoogleMap virtualView, MapView platformView, GoogleMapHandler handler)
    {
        VirtualView!.MapCoordsToScreenLocationFuncInternal = null;
        VirtualView.ScreenLocationToMapCoordsFuncInternal = null;

        platformView.CoordinateTapped -= PlatformView_CoordinateTapped;
        platformView.CoordinateLongPressed -= PlatformView_CoordinateLongPressed;
        platformView.PoiWithPlaceIdTapped -= PlatformView_PoiWithPlaceIdTapped;
        platformView.IndoorDisplay.Delegate = null;
        platformView.MapCapabilitiesChanged -= PlatformView_MapCapabilitiesChanged;

        virtualView.PropertyChanged -= VirtualView_PropertyChanged;
        virtualView.PropertyChanging -= VirtualView_PropertyChanging;

        VirtualView = null;
        NativeView = null;
        Handler = null;
    }

    protected virtual void InitMap()
    {
        NativeView!.IndoorEnabled = VirtualView!.IndoorEnabled;
        NativeView.BuildingsEnabled = VirtualView.BuildingsEnabled;
        NativeView.MapType = VirtualView.MapType.ToNative();
        NativeView.MyLocationEnabled = VirtualView.MyLocationEnabled;
        NativeView.TrafficEnabled = VirtualView.TrafficEnabled;
        NativeView.Padding = VirtualView.Padding.ToNative();

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
            NativeView!.IndoorEnabled = VirtualView!.IndoorEnabled;
        }
        else if (e.PropertyName == GoogleMap.BuildingsEnabledProperty.PropertyName)
        {
            NativeView!.BuildingsEnabled = VirtualView!.BuildingsEnabled;
        }
        else if (e.PropertyName == GoogleMap.MapTypeProperty.PropertyName)
        {
            NativeView!.MapType = VirtualView!.MapType.ToNative();
        }
        else if (e.PropertyName == GoogleMap.MyLocationEnabledProperty.PropertyName)
        {
            NativeView!.MyLocationEnabled = VirtualView!.MyLocationEnabled;
        }
        else if (e.PropertyName == GoogleMap.TrafficEnabledProperty.PropertyName)
        {
            NativeView!.TrafficEnabled = VirtualView!.TrafficEnabled;
        }
        else if (e.PropertyName == GoogleMap.PaddingProperty.PropertyName)
        {
            NativeView!.Padding = VirtualView!.Padding.ToNative();
        }
        else if (e.PropertyName == GoogleMap.MapStyleJsonProperty.PropertyName)
        {
            SetMapStyle();
        }
    }

    protected virtual async Task SetMapStyle()
    {
        if (string.IsNullOrWhiteSpace(VirtualView!.MapStyleJson)) return;

        var json = VirtualView!.MapStyleJson;

        try
        {
            if (Uri.TryCreate(json, UriKind.Absolute, out var uri))
            {
                using var client = new HttpClient() { Timeout = TimeSpan.FromSeconds(30) };
                json = await client.GetStringAsync(uri);
            }
            else if (json.EndsWith(".json", StringComparison.InvariantCultureIgnoreCase))
            {
                json = json
                    .Replace('\\', Path.DirectorySeparatorChar)
                    .Replace('/', Path.DirectorySeparatorChar);
                using var stream = await FileSystem.OpenAppPackageFileAsync(json);
                using var reader = new StreamReader(stream);
                json = await reader.ReadToEndAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return;
        }

        NativeView!.MapStyle = MapStyle.FromJson(json, null);
    }

    protected virtual Point ScreenLocationToMapCoords(Point point)
    {
        return NativeView!.Projection.CoordinateForPoint(point).ToCrossPlatformPoint();
    }

    protected virtual Point MapCoordsToScreenLocation(Point point)
    {
        return NativeView!.Projection.PointForCoordinate(point.ToCoord()).ToCrossPlatformPoint();
    }

    private void PlatformView_CoordinateTapped(object? sender, GMSCoordEventArgs e)
    {
        VirtualView!.SendMapClick(e.Coordinate.ToCrossPlatformPoint());
    }

    private void PlatformView_CoordinateLongPressed(object? sender, GMSCoordEventArgs e)
    {
        VirtualView!.SendMapLongClick(e.Coordinate.ToCrossPlatformPoint());
    }

    private void PlatformView_PoiWithPlaceIdTapped(object? sender, GMSPoiWithPlaceIdEventEventArgs e)
    {
        if (e is null) return;

        VirtualView!.SendPoiClick(e.ToCrossPlatform());
    }

    protected virtual void HandlePoiClick()
    {
        if (VirtualView!.HandlePoiClick)
        {
            NativeView!.PoiWithPlaceIdTapped += PlatformView_PoiWithPlaceIdTapped;
        }
        else
        {
            NativeView!.PoiWithPlaceIdTapped -= PlatformView_PoiWithPlaceIdTapped;
        }
    }

    private void PlatformView_MapCapabilitiesChanged(object? sender, GMSMapCapabilitiesEventArgs e)
    {
        VirtualView!.SendMapCapabilitiesChanged(e.MapCapabilities.ToCrossPlatform());
    }

    public void DidChangeActiveBuilding(Google.Maps.IndoorBuilding? building)
    {
        VirtualView!.SendIndoorBuildingFocused(building?.ToCrossPlatform(NativeView!));
    }

    public void DidChangeActiveLevel(Google.Maps.IndoorLevel? level)
    {
        VirtualView!.SendIndoorLevelActivated(level?.ToCrossPlatform(NativeView!));
    }
}

public static class MapExtensions
{
    public static MapViewType ToNative(this MapType type)
    {
        return type switch
        {
            MapType.None => MapViewType.None,
            MapType.Normal => MapViewType.Normal,
            MapType.Satellite => MapViewType.Satellite,
            MapType.Terrain => MapViewType.Terrain,
            MapType.Hybrid => MapViewType.Hybrid
        };
    }

    public static UIEdgeInsets ToNative(this Thickness padding)
    {
        return new((float)padding.Top, (float)padding.Left, (float)padding.Bottom, (float)padding.Right);
    }

    public static PointOfInterest ToCrossPlatform(this GMSPoiWithPlaceIdEventEventArgs poi)
    {
        return new(poi.Location.ToCrossPlatformPoint(), poi.PlaceId, poi.Name);
    }

    public static IndoorLevel ToCrossPlatform(this Google.Maps.IndoorLevel level, MapView map)
    {
        return new()
        {
            NativeMap = map,
            NativeIndoorLevel = level,
            Name = level.Name,
            ShortName = level.ShortName
        };
    }

    public static IndoorBuilding ToCrossPlatform(this Google.Maps.IndoorBuilding building, MapView map)
    {
        return new IndoorBuilding
        {
            DefaultLevelIndex = (int)building.DefaultLevelIndex,
            IsUnderground = building.Underground,
            Levels = building.Levels.Select(l => l.ToCrossPlatform(map)).ToList()
        };
    }

    public static MapCapabilities ToCrossPlatform(this MapCapabilityFlags capabilities)
    {
        return new(capabilities.HasFlag(MapCapabilityFlags.AdvancedMarkers), capabilities.HasFlag(MapCapabilityFlags.DataDrivenStyling));
    }
}