using System.ComponentModel;

using Google.Maps;

using UIKit;

using Path = System.IO.Path;

namespace MPowerKit.GoogleMaps;

public class MapManager : MapFeatureManager<GoogleMap, MapView, GoogleMapHandler>
{
    protected override void Init(GoogleMap virtualView, MapView platformView, GoogleMapHandler handler)
    {
        base.Init(virtualView, platformView, handler);

        virtualView.SendMapCapabilitiesChanged(platformView.MapCapabilities.ToCrossPlatform(), false);
        virtualView.SendIndoorBuildingFocused(platformView.IndoorDisplay?.ActiveBuilding?.ToCrossPlatform(platformView), false);
        virtualView.SendIndoorLevelActivated(platformView.IndoorDisplay?.ActiveLevel?.ToCrossPlatform(platformView), false);

        OnHandlePoiClickChanged(virtualView, platformView);
        OnIndoorEnabledChanged(virtualView, platformView);
        OnBuildingsEnabledChanged(virtualView, platformView);
        OnMapTypeChanged(virtualView, platformView);
        OnMyLocationEnabledChanged(virtualView, platformView);
        OnTrafficEnabledChanged(virtualView, platformView);
        OnPaddingChanged(virtualView, platformView);
        OnMapStyleChanged(virtualView, platformView);
    }

    protected override void SubscribeToEvents(GoogleMap virtualView, MapView platformView, GoogleMapHandler handler)
    {
        base.SubscribeToEvents(virtualView, platformView, handler);

        virtualView.MapCoordsToScreenLocationFuncInternal = ProjectMapCoordsToScreenLocation;
        virtualView.ScreenLocationToMapCoordsFuncInternal = ProjectScreenLocationToMapCoords;
        virtualView.TakeSnapshotFuncInternal = TakeSnapshot;

        platformView.CoordinateTapped += PlatformView_CoordinateTapped;
        platformView.CoordinateLongPressed += PlatformView_CoordinateLongPressed;
        platformView.IndoorDisplay.Delegate = new IndoorDisplayDel(this);
        platformView.MapCapabilitiesChanged += PlatformView_MapCapabilitiesChanged;
    }

    protected override void UnsubscribeFromEvents(GoogleMap virtualView, MapView platformView, GoogleMapHandler handler)
    {
        virtualView.MapCoordsToScreenLocationFuncInternal = null;
        virtualView.ScreenLocationToMapCoordsFuncInternal = null;
        virtualView.TakeSnapshotFuncInternal = null;

        platformView.CoordinateTapped -= PlatformView_CoordinateTapped;
        platformView.CoordinateLongPressed -= PlatformView_CoordinateLongPressed;
        platformView.PoiWithPlaceIdTapped -= PlatformView_PoiWithPlaceIdTapped;
        platformView.IndoorDisplay.Delegate = null;
        platformView.MapCapabilitiesChanged -= PlatformView_MapCapabilitiesChanged;

        base.UnsubscribeFromEvents(virtualView, platformView, handler);
    }

    protected override void VirtualViewPropertyChanged(GoogleMap virtualView, MapView platformView, string? propertyName)
    {
        base.VirtualViewPropertyChanged(virtualView, platformView, propertyName);

        if (propertyName == GoogleMap.HandlePoiClickProperty.PropertyName)
        {
            OnHandlePoiClickChanged(virtualView, platformView);
        }
        else if (propertyName == GoogleMap.IndoorEnabledProperty.PropertyName)
        {
            OnIndoorEnabledChanged(virtualView, platformView);
        }
        else if (propertyName == GoogleMap.BuildingsEnabledProperty.PropertyName)
        {
            OnBuildingsEnabledChanged(virtualView, platformView);
        }
        else if (propertyName == GoogleMap.MapTypeProperty.PropertyName)
        {
            OnMapTypeChanged(virtualView, platformView);
        }
        else if (propertyName == GoogleMap.MyLocationEnabledProperty.PropertyName)
        {
            OnMyLocationEnabledChanged(virtualView, platformView);
        }
        else if (propertyName == GoogleMap.TrafficEnabledProperty.PropertyName)
        {
            OnTrafficEnabledChanged(virtualView, platformView);
        }
        else if (propertyName == GoogleMap.PaddingProperty.PropertyName)
        {
            OnPaddingChanged(virtualView, platformView);
        }
        else if (propertyName == GoogleMap.MapStyleJsonProperty.PropertyName)
        {
            OnMapStyleChanged(virtualView, platformView);
        }
    }

    protected virtual void OnHandlePoiClickChanged(GoogleMap virtualView, MapView platformView)
    {
        if (virtualView.HandlePoiClick)
        {
            platformView.PoiWithPlaceIdTapped += PlatformView_PoiWithPlaceIdTapped;
        }
        else
        {
            platformView.PoiWithPlaceIdTapped -= PlatformView_PoiWithPlaceIdTapped;
        }
    }

    protected virtual void OnIndoorEnabledChanged(GoogleMap virtualView, MapView platformView)
    {
        platformView.IndoorEnabled = virtualView.IndoorEnabled;
    }

    protected virtual void OnBuildingsEnabledChanged(GoogleMap virtualView, MapView platformView)
    {
        platformView.BuildingsEnabled = virtualView.BuildingsEnabled;
    }

    protected virtual void OnMapTypeChanged(GoogleMap virtualView, MapView platformView)
    {
        platformView.MapType = virtualView.MapType.ToNative();
    }

    protected virtual void OnMyLocationEnabledChanged(GoogleMap virtualView, MapView platformView)
    {
        platformView.MyLocationEnabled = virtualView.MyLocationEnabled;
    }

    protected virtual void OnTrafficEnabledChanged(GoogleMap virtualView, MapView platformView)
    {
        platformView.TrafficEnabled = virtualView.TrafficEnabled;
    }

    protected virtual void OnPaddingChanged(GoogleMap virtualView, MapView platformView)
    {
        platformView.Padding = virtualView.Padding.ToNative();
    }

    protected virtual async Task OnMapStyleChanged(GoogleMap virtualView, MapView platformView)
    {
        if (string.IsNullOrWhiteSpace(virtualView.MapStyleJson))
        {
            platformView.MapStyle = null;
            return;
        }

        var json = virtualView.MapStyleJson;

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

        platformView.MapStyle = MapStyle.FromJson(json, null);
    }

    protected virtual Task<Stream?> TakeSnapshot()
    {
        var view = PlatformView!.SnapshotView(true);

        var image = view.ToImage();

        return Task.FromResult(image?.AsPNG()?.AsStream());
    }

    protected virtual Point ProjectScreenLocationToMapCoords(Point point)
    {
        return PlatformView!.Projection.CoordinateForPoint(point).ToCrossPlatformPoint();
    }

    protected virtual Point ProjectMapCoordsToScreenLocation(Point point)
    {
        return PlatformView!.Projection.PointForCoordinate(point.ToCoord()).ToCrossPlatformPoint();
    }

    protected virtual void PlatformView_CoordinateTapped(object? sender, GMSCoordEventArgs e)
    {
        VirtualView!.SendMapClick(e.Coordinate.ToCrossPlatformPoint());
    }

    protected virtual void PlatformView_CoordinateLongPressed(object? sender, GMSCoordEventArgs e)
    {
        VirtualView!.SendMapLongClick(e.Coordinate.ToCrossPlatformPoint());
    }

    protected virtual void PlatformView_PoiWithPlaceIdTapped(object? sender, GMSPoiWithPlaceIdEventEventArgs e)
    {
        if (e is null) return;

        VirtualView!.SendPoiClick(e.ToCrossPlatform());
    }

    protected virtual void PlatformView_MapCapabilitiesChanged(object? sender, GMSMapCapabilitiesEventArgs e)
    {
        VirtualView!.SendMapCapabilitiesChanged(e.MapCapabilities.ToCrossPlatform());
    }

    public virtual void PlatformView_DidChangeActiveBuilding(Google.Maps.IndoorBuilding? building)
    {
        VirtualView!.SendIndoorBuildingFocused(building?.ToCrossPlatform(PlatformView!));
    }

    public virtual void PlatformView_DidChangeActiveLevel(Google.Maps.IndoorLevel? level)
    {
        VirtualView!.SendIndoorLevelActivated(level?.ToCrossPlatform(PlatformView!));
    }

    public class IndoorDisplayDel : IndoorDisplayDelegate
    {
        private readonly MapManager _mapManager;

        public IndoorDisplayDel(MapManager mapManager)
        {
            _mapManager = mapManager;
        }

        public override void DidChangeActiveBuilding(Google.Maps.IndoorBuilding? building)
        {
            _mapManager.PlatformView_DidChangeActiveBuilding(building);
        }

        public override void DidChangeActiveLevel(Google.Maps.IndoorLevel? level)
        {
            _mapManager.PlatformView_DidChangeActiveLevel(level);
        }
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