﻿using Microsoft.Maui.Platform;

using GMap = Android.Gms.Maps.GoogleMap;

namespace MPowerKit.GoogleMaps;

public class MapManager : MapFeatureManager<GMap>
{
    protected override void Init(GoogleMap virtualView, GMap platformView, GoogleMapHandler handler)
    {
        base.Init(virtualView, platformView, handler);

        virtualView.SendMapCapabilitiesChanged(platformView.MapCapabilities.ToCrossPlatform(), false);
        virtualView.SendIndoorBuildingFocused(platformView.FocusedBuilding?.ToCrossPlatform(), false);
        virtualView.SendIndoorLevelActivated(platformView.FocusedBuilding?.Levels.ElementAtOrDefault(platformView.FocusedBuilding?.ActiveLevelIndex ?? 0)?.ToCrossPlatform(), false);

        OnHandlePoiClickChanged(virtualView, platformView);
        OnIndoorEnabledChanged(virtualView, platformView);
        OnBuildingsEnabledChanged(virtualView, platformView);
        OnMapTypeChanged(virtualView, platformView);
        OnMyLocationEnabledChanged(virtualView, platformView);
        OnMapColorSchemeChanged(virtualView, platformView);
        OnTrafficEnabledChanged(virtualView, platformView);
        OnPaddingChanged(virtualView, platformView);
        OnMapStyleChanged(virtualView, platformView);
    }

    protected override void SubscribeToEvents(GoogleMap virtualView, GMap platformView, GoogleMapHandler handler)
    {
        base.SubscribeToEvents(virtualView, platformView, handler);

        virtualView.ProjectMapCoordsToScreenLocationFuncInternal = ProjectMapCoordsToScreenLocation;
        virtualView.ProjectScreenLocationToMapCoordsFuncInternal = ProjectScreenLocationToMapCoords;
        virtualView.TakeSnapshotFuncInternal = TakeSnapshot;

        platformView.MapClick += PlatformView_MapClick;
        platformView.MapLongClick += PlatformView_MapLongClick;
        platformView.MapCapabilitiesChanged += PlatformView_MapCapabilitiesChanged;
        platformView.IndoorLevelActivated += PlatformView_IndoorLevelActivated;
        platformView.IndoorBuildingFocused += PlatformView_IndoorBuildingFocused;
    }

    protected override void UnsubscribeFromEvents(GoogleMap virtualView, GMap platformView, GoogleMapHandler handler)
    {
        virtualView.ProjectMapCoordsToScreenLocationFuncInternal = null;
        virtualView.ProjectScreenLocationToMapCoordsFuncInternal = null;
        virtualView.TakeSnapshotFuncInternal = null;

        platformView.MapClick -= PlatformView_MapClick;
        platformView.MapLongClick -= PlatformView_MapLongClick;
        platformView.PoiClick -= PlatformView_PoiClick;
        platformView.MapCapabilitiesChanged -= PlatformView_MapCapabilitiesChanged;
        platformView.IndoorLevelActivated -= PlatformView_IndoorLevelActivated;
        platformView.IndoorBuildingFocused -= PlatformView_IndoorBuildingFocused;

        base.UnsubscribeFromEvents(virtualView, platformView, handler);
    }

    protected override void VirtualViewPropertyChanged(GoogleMap virtualView, GMap platformView, string? propertyName)
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
        else if (propertyName == GoogleMap.MapColorSchemeProperty.PropertyName)
        {
            OnMapColorSchemeChanged(virtualView, platformView);
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

    protected virtual void OnHandlePoiClickChanged(GoogleMap virtualView, GMap platformView)
    {
        if (virtualView.HandlePoiClick)
        {
            platformView.PoiClick += PlatformView_PoiClick;
        }
        else
        {
            platformView.PoiClick -= PlatformView_PoiClick;
        }
    }

    protected virtual void OnIndoorEnabledChanged(GoogleMap virtualView, GMap platformView)
    {
        platformView.SetIndoorEnabled(virtualView.IndoorEnabled);
    }

    protected virtual void OnBuildingsEnabledChanged(GoogleMap virtualView, GMap platformView)
    {
        platformView.BuildingsEnabled = virtualView.BuildingsEnabled;
    }

    protected virtual void OnMapTypeChanged(GoogleMap virtualView, GMap platformView)
    {
        platformView.MapType = (int)virtualView.MapType;
    }

    protected virtual void OnMyLocationEnabledChanged(GoogleMap virtualView, GMap platformView)
    {
        platformView.MyLocationEnabled = virtualView.MyLocationEnabled;
    }

    protected virtual void OnMapColorSchemeChanged(GoogleMap virtualView, GMap platformView)
    {
        platformView.MapColorScheme = (int)virtualView.MapColorScheme;
    }

    protected virtual void OnTrafficEnabledChanged(GoogleMap virtualView, GMap platformView)
    {
        platformView.TrafficEnabled = virtualView.TrafficEnabled;
    }

    protected virtual void OnPaddingChanged(GoogleMap virtualView, GMap platformView)
    {
        var context = Handler!.Context;
        platformView.SetPadding(
            (int)context.ToPixels(virtualView.Padding.Left),
            (int)context.ToPixels(virtualView.Padding.Top),
            (int)context.ToPixels(virtualView.Padding.Right),
            (int)context.ToPixels(virtualView.Padding.Bottom)
            );
    }

    protected virtual async Task OnMapStyleChanged(GoogleMap virtualView, GMap platformView)
    {
        if (string.IsNullOrWhiteSpace(virtualView.MapStyleJson))
        {
            platformView.SetMapStyle(null);
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

        platformView.SetMapStyle(new(json));
    }

    protected virtual Task<Stream?> TakeSnapshot()
    {
        var tcs = new TaskCompletionSource<Stream?>(TaskCreationOptions.RunContinuationsAsynchronously);
        PlatformView!.Snapshot(new TakeSnapshotCallback(tcs.SetResult));
        return tcs.Task;
    }

    protected virtual Point ProjectScreenLocationToMapCoords(Point point)
    {
        return PlatformView!.Projection.FromScreenLocation(point.ToNativePoint(Handler!.Context)).ToCrossPlatformPoint();
    }

    protected virtual Point ProjectMapCoordsToScreenLocation(Point point)
    {
        return PlatformView!.Projection.ToScreenLocation(point.ToLatLng()).ToCrossPlatformPoint(Handler!.Context);
    }

    protected virtual void PlatformView_MapClick(object? sender, GMap.MapClickEventArgs e)
    {
        VirtualView!.SendMapClick(e.Point.ToCrossPlatformPoint());
    }

    protected virtual void PlatformView_MapLongClick(object? sender, GMap.MapLongClickEventArgs e)
    {
        VirtualView!.SendMapLongClick(e.Point.ToCrossPlatformPoint());
    }

    protected virtual void PlatformView_PoiClick(object? sender, GMap.PoiClickEventArgs e)
    {
        if (e.Poi is null) return;

        VirtualView!.SendPoiClick(e.Poi.ToCrossPlatform());
    }

    protected virtual void PlatformView_MapCapabilitiesChanged(object? sender, GMap.MapCapabilitiesChangedEventArgs e)
    {
        VirtualView!.SendMapCapabilitiesChanged(e.P0.ToCrossPlatform());
    }

    protected virtual void PlatformView_IndoorBuildingFocused(object? sender, EventArgs e)
    {
        VirtualView!.SendIndoorBuildingFocused(PlatformView!.FocusedBuilding?.ToCrossPlatform());
    }

    protected virtual void PlatformView_IndoorLevelActivated(object? sender, GMap.IndoorLevelActivatedEventArgs e)
    {
        var activeLevel = PlatformView!.FocusedBuilding?.Levels.ElementAtOrDefault(PlatformView.FocusedBuilding.ActiveLevelIndex);

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
        return new
        (
            building.DefaultLevelIndex,
            building.IsUnderground,
            [.. building.Levels.Select(l => l.ToCrossPlatform())]
        );
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
        snapshot.Compress(Android.Graphics.Bitmap.CompressFormat.Png!, 100, stream);
        stream.Position = 0;
        _onSnapshot?.Invoke(stream);
    }
}