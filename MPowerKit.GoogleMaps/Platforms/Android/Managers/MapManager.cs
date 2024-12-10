using System.ComponentModel;

using Microsoft.Maui.Platform;

using GMap = Android.Gms.Maps.GoogleMap;

namespace MPowerKit.GoogleMaps;

public class MapManager : IMapFeatureManager<GoogleMap, GMap, GoogleMapHandler>
{
    protected GoogleMap? VirtualView { get; set; }
    protected GMap? NativeView { get; set; }
    protected GoogleMapHandler? Handler { get; set; }

    public virtual void Connect(GoogleMap virtualView, GMap platformView, GoogleMapHandler handler)
    {
        VirtualView = virtualView;
        NativeView = platformView;
        Handler = handler;

        virtualView.PropertyChanged += VirtualView_PropertyChanged;
        virtualView.PropertyChanging += VirtualView_PropertyChanging;

        virtualView.MapCoordsToScreenLocationFuncInternal = MapCoordsToScreenLocation;
        virtualView.ScreenLocationToMapCoordsFuncInternal = ScreenLocationToMapCoords;
        virtualView.TakeSnapshotFuncInternal = TakeSnapshot;

        platformView.MapClick += NativeMap_MapClick;
        platformView.MapLongClick += NativeMap_MapLongClick;
        platformView.MapCapabilitiesChanged += NativeView_MapCapabilitiesChanged;
        platformView.IndoorLevelActivated += NativeView_IndoorLevelActivated;
        platformView.IndoorBuildingFocused += NativeView_IndoorBuildingFocused;

        HandlePoiClick();

        InitMap();
    }

    public virtual void Disconnect(GoogleMap virtualView, GMap platformView, GoogleMapHandler handler)
    {
        virtualView.MapCoordsToScreenLocationFuncInternal = null;
        virtualView.ScreenLocationToMapCoordsFuncInternal = null;
        virtualView.TakeSnapshotFuncInternal = null;

        platformView.MapClick -= NativeMap_MapClick;
        platformView.MapLongClick -= NativeMap_MapLongClick;
        platformView.PoiClick -= NativeMap_PoiClick;
        platformView.MapCapabilitiesChanged -= NativeView_MapCapabilitiesChanged;
        platformView.IndoorLevelActivated -= NativeView_IndoorLevelActivated;
        platformView.IndoorBuildingFocused -= NativeView_IndoorBuildingFocused;

        virtualView.PropertyChanged -= VirtualView_PropertyChanged;
        virtualView.PropertyChanging -= VirtualView_PropertyChanging;

        VirtualView = null;
        NativeView = null;
        Handler = null;
    }

    protected virtual void InitMap()
    {
        NativeView!.SetIndoorEnabled(VirtualView!.IndoorEnabled);
        NativeView.BuildingsEnabled = VirtualView.BuildingsEnabled;
        NativeView.MapType = (int)VirtualView.MapType;
        NativeView.MyLocationEnabled = VirtualView.MyLocationEnabled;
        NativeView.MapColorScheme = (int)VirtualView.MapColorScheme;
        NativeView.TrafficEnabled = VirtualView.TrafficEnabled;
        var context = Handler!.Context;
        NativeView.SetPadding(
            (int)context.ToPixels(VirtualView.Padding.Left),
            (int)context.ToPixels(VirtualView.Padding.Top),
            (int)context.ToPixels(VirtualView.Padding.Right),
            (int)context.ToPixels(VirtualView.Padding.Bottom)
            );

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
                using var stream = Android.App.Application.Context.Assets!.Open(json);
                using var reader = new StreamReader(stream);
                json = await reader.ReadToEndAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return;
        }

        NativeView!.SetMapStyle(new(json));
    }

    protected virtual Task<Stream?> TakeSnapshot()
    {
        var tcs = new TaskCompletionSource<Stream?>(TaskCreationOptions.RunContinuationsAsynchronously);
        NativeView!.Snapshot(new TakeSnapshotCallback(tcs.SetResult));
        return tcs.Task;
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
        var activeLevel = NativeView!.FocusedBuilding?.Levels.ElementAtOrDefault(NativeView.FocusedBuilding.ActiveLevelIndex);

        VirtualView!.SendIndoorLevelActivated(activeLevel?.ToCrossPlatform());
    }
}

public static class MapExtensions
{
    public static PointOfInterest ToCrossPlatform(this Android.Gms.Maps.Model.PointOfInterest poi)
    {
        return new(poi.LatLng.ToCrossPlatformPoint(), poi.PlaceId, poi.Name);
    }

    public static IndoorLevel ToCrossPlatform(this Android.Gms.Maps.Model.IndoorLevel level)
    {
        return new()
        {
            NativeIndoorLevel = level,
            Name = level.Name,
            ShortName = level.ShortName
        };
    }

    public static IndoorBuilding ToCrossPlatform(this Android.Gms.Maps.Model.IndoorBuilding building)
    {
        return new IndoorBuilding
        {
            DefaultLevelIndex = building.DefaultLevelIndex,
            IsUnderground = building.IsUnderground,
            Levels = building.Levels.Select(l => l.ToCrossPlatform()).ToList()
        };
    }

    public static MapCapabilities ToCrossPlatform(this Android.Gms.Maps.Model.MapCapabilities capabilities)
    {
        return new(capabilities.IsAdvancedMarkersAvailable, capabilities.IsDataDrivenStylingAvailable);
    }
}

public class TakeSnapshotCallback : Java.Lang.Object, GMap.ISnapshotReadyCallback
{
    private readonly Action<Stream?> _onSnapshot;

    public TakeSnapshotCallback(Action<Stream?> onSnapshot)
    {
        _onSnapshot = onSnapshot;
    }

    public void OnSnapshotReady(Android.Graphics.Bitmap? snapshot)
    {
        if (snapshot is null)
        {
            _onSnapshot?.Invoke(null);
            return;
        }

        var stream = new MemoryStream();
        snapshot.Compress(Android.Graphics.Bitmap.CompressFormat.Png!, 0, stream);
        stream.Position = 0;
        _onSnapshot?.Invoke(stream);
    }
}