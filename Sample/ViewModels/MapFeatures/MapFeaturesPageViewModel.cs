using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Controls.UserDialogs.Maui;

using MPowerKit.GoogleMaps;

namespace Sample.ViewModels;

public partial class MapFeaturesPageViewModel : ObservableObject
{
    public MapFeaturesPageViewModel()
    {
    }

    [ObservableProperty]
    private IndoorBuilding? _focusedBuilding;

    [ObservableProperty]
    private IndoorLevel? _activeLevel;

    [ObservableProperty]
    private Func<Task<Stream?>> _takeSnapshotFunc;

    [ObservableProperty]
    private ImageSource? _snapshot;

    [ObservableProperty]
    private Func<Point, Point?> _mapCoordsToScreenLocationFunc;

    [ObservableProperty]
    private bool _enableMyLocation;
    partial void OnEnableMyLocationChanged(bool oldValue, bool newValue)
    {
        if (!newValue)
        {
            MyLocationEnabled = false;
            return;
        }
        Setup();
    }

    private async void Setup()
    {
        var res = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        if (res is PermissionStatus.Granted)
        {
            MyLocationEnabled = true;
            return;
        }

        res = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        if (res is not PermissionStatus.Granted)
        {
            EnableMyLocation = false;
            MyLocationEnabled = false;
            return;
        }

        MyLocationEnabled = true;
    }

    [ObservableProperty]
    private bool _myLocationEnabled;

    [ObservableProperty]
    private MapType _mapType = MapType.Normal;

    [ObservableProperty]
    private MapColorScheme _mapColorScheme;

    [RelayCommand]
    private async Task ActivateIndoorLevel()
    {
        if (FocusedBuilding is null) return;

        var res = await UserDialogs.Instance.ActionSheetAsync(null, "Choose level to activate", "Cancel",
            buttons: FocusedBuilding.Levels.Select(l => l.Name).ToArray());

        var level = FocusedBuilding.Levels.FirstOrDefault(l => l.Name == res);
        if (level is null) return;

        level.Activate();
    }

    [RelayCommand]
    private async Task ChangeMapType()
    {
        var res = await UserDialogs.Instance.ActionSheetAsync(null, "Choose map type", "Cancel",
            buttons: Enum.GetValues<MapType>().Select(t => t.ToString()).ToArray());

        if (res == "Cancel" || MapType.ToString() == res) return;

        MapType = res switch
        {
            "Normal" => MapType.Normal,
            "None" => MapType.None,
            "Satellite" => MapType.Satellite,
            "Terrain" => MapType.Terrain,
            "Hybrid" => MapType.Hybrid,
        };
    }

    [RelayCommand]
    private async Task ChangeMapColorScheme()
    {
        var res = await UserDialogs.Instance.ActionSheetAsync("Choose map color scheme", null, "Cancel",
            buttons: Enum.GetValues<MapColorScheme>().Select(t => t.ToString()).ToArray());

        if (res == "Cancel" || MapColorScheme.ToString() == res) return;

        MapColorScheme = res switch
        {
            "FollowSystem" => MapColorScheme.FollowSystem,
            "Dark" => MapColorScheme.Dark,
            "Light" => MapColorScheme.Light,
        };
    }

    [RelayCommand]
    private async Task PoiClicked(PointOfInterest poi)
    {
        await UserDialogs.Instance.AlertAsync($"{poi.Name} PoI clicked at {{Lat={poi.Position.X}, Lon={poi.Position.Y}}}");
    }

    [RelayCommand]
    private async Task MapClicked(Point position)
    {
        var screenCoords = MapCoordsToScreenLocationFunc(position);
        await UserDialogs.Instance.AlertAsync($"Map clicked at {{Lat={position.X}, Lon={position.Y}}}\r\n\r\nThis is {screenCoords} in screen coords");
    }

    [RelayCommand]
    private async Task MapLongClick(Point position)
    {
        await UserDialogs.Instance.AlertAsync($"Map long click at {{Lat={position.X}, Lon={position.Y}}}");
    }

    [RelayCommand]
    private async Task TakeSnapshot()
    {
        var snapshotStream = await TakeSnapshotFunc();

        if (snapshotStream is null) return;

        Snapshot = ImageSource.FromStream(() => snapshotStream);

        await Task.Delay(5000);

        Snapshot = null;

        snapshotStream?.Dispose();
    }
}