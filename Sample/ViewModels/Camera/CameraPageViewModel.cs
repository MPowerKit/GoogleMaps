using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Controls.UserDialogs.Maui;

using MPowerKit.GoogleMaps;

namespace Sample.ViewModels;

public partial class CameraPageViewModel : ObservableObject
{
    private readonly Random _rnd = new();

    public CameraPageViewModel(bool initialPosition)
    {
        if (initialPosition)
        {
            InitialCameraPosition = RandomizeCameraPosition();
        }
    }

    [ObservableProperty]
    private CameraUpdate? _initialCameraPosition;

    [ObservableProperty]
    private Action<CameraUpdate> _moveCameraAction;

    [ObservableProperty]
    private Func<CameraUpdate, int, Task> _animateCameraFunc;

    [ObservableProperty]
    private Action _resetMinMaxZoomAction;

    [ObservableProperty]
    private double _minZoom;

    [ObservableProperty]
    private double _maxZoom;

    [ObservableProperty]
    private double _mapWidth;

    [ObservableProperty]
    private double _mapHeight;

    [ObservableProperty]
    private string? _cameraStartReason;

    [ObservableProperty]
    private string? _cameraMoving;

    [ObservableProperty]
    private LatLngBounds? _restrictPanningToArea;

    [ObservableProperty]
    private CameraPosition _cameraPosition;

    [ObservableProperty]
    private VisibleRegion _visibleRegion;

    private CameraUpdate RandomizeCameraPosition()
    {
        var lat = _rnd.Next(-90, 90);
        var lon = _rnd.Next(-180, 180);
        var radius = _rnd.Next(500, 2_000_000);
        return CameraUpdateFactory.FromCenterAndRadius(new(lat, lon), radius);
    }

    [RelayCommand]
    private async Task MoveCamera(string animateStr)
    {
        var animate = bool.Parse(animateStr);

        var res = await UserDialogs.Instance.ActionSheetAsync(null,
            "Choose CameraUpdate to move the camera",
            "Cancel",
            buttons:
            [
                nameof(CameraUpdateFactory.ZoomIn),
                nameof(CameraUpdateFactory.ZoomOut),
                nameof(CameraUpdateFactory.ZoomTo),
                nameof(CameraUpdateFactory.ZoomBy),
                nameof(CameraUpdateFactory.ZoomBy) + " and focus at point on the screen",
                nameof(CameraUpdateFactory.ScrollBy),
                nameof(CameraUpdateFactory.NewCameraPosition),
                nameof(CameraUpdateFactory.NewLatLng),
                nameof(CameraUpdateFactory.NewLatLngZoom),
                nameof(CameraUpdateFactory.NewLatLngBounds),
                nameof(CameraUpdateFactory.NewLatLngBounds) + " with size",
                nameof(CameraUpdateFactory.FromCenterAndRadius)
            ]);

        if (res == "Cancel") return;

        var cu = res switch
        {
            nameof(CameraUpdateFactory.ZoomIn) => CameraUpdateFactory.ZoomIn(),
            nameof(CameraUpdateFactory.ZoomOut) => CameraUpdateFactory.ZoomOut(),
            nameof(CameraUpdateFactory.ZoomTo) => CameraUpdateFactory.ZoomTo(_rnd.Next((int)MinZoom, (int)MaxZoom)),
            nameof(CameraUpdateFactory.ZoomBy) =>
                CameraUpdateFactory.ZoomBy(_rnd.Next((int)(MinZoom - CameraPosition.Zoom), (int)(MaxZoom - CameraPosition.Zoom))),
            nameof(CameraUpdateFactory.ZoomBy) + " and focus at point on the screen" =>
                CameraUpdateFactory.ZoomBy(_rnd.Next((int)(MinZoom - CameraPosition.Zoom), (int)(MaxZoom - CameraPosition.Zoom)), new(_rnd.Next(0, (int)MapWidth), _rnd.Next(0, (int)MapHeight))),
            nameof(CameraUpdateFactory.ScrollBy) => CameraUpdateFactory.ScrollBy(_rnd.Next(0, (int)MapWidth), _rnd.Next(0, (int)MapHeight)),
            nameof(CameraUpdateFactory.NewCameraPosition) =>
                CameraUpdateFactory.NewCameraPosition(new CameraPosition.Builder().Bearing(_rnd.Next(-180, 180)).Tilt(_rnd.Next(0, 90)).Zoom(_rnd.Next((int)MinZoom, (int)MaxZoom)).Target(new(_rnd.Next(-90, 90), _rnd.Next(-180, 180))).Build()),
            nameof(CameraUpdateFactory.NewLatLng) => CameraUpdateFactory.NewLatLng(new(_rnd.Next(-90, 90), _rnd.Next(-180, 180))),
            nameof(CameraUpdateFactory.NewLatLngZoom) => CameraUpdateFactory.NewLatLngZoom(new(_rnd.Next(-90, 90), _rnd.Next(-180, 180)), _rnd.Next((int)MinZoom, (int)MaxZoom)),
            nameof(CameraUpdateFactory.NewLatLngBounds) =>
                CameraUpdateFactory.NewLatLngBounds(new LatLngBounds(new(_rnd.Next(-90, 0), _rnd.Next(-180, 0)), new(_rnd.Next(0, 90), _rnd.Next(0, 180))), _rnd.Next(0, 100)),
            nameof(CameraUpdateFactory.NewLatLngBounds) + " with size" =>
                CameraUpdateFactory.NewLatLngBounds(new LatLngBounds(new(_rnd.Next(-90, 0), _rnd.Next(-180, 0)), new(_rnd.Next(0, 90), _rnd.Next(0, 180))), _rnd.Next(0, 100), new(_rnd.Next(0, (int)MapWidth), _rnd.Next(0, (int)MapHeight))),
            nameof(CameraUpdateFactory.FromCenterAndRadius) => RandomizeCameraPosition()
        };

        try
        {
            await MoveCamera(animate, cu);
        }
        catch (Exception ex)
        {
            await Task.Delay(500);
            await UserDialogs.Instance.AlertAsync(ex.Message);
        }
    }

    private async Task MoveCamera(bool animate, CameraUpdate newPosition)
    {
        if (animate)
        {
            var rnd = new Random();
            var durationMs = rnd.Next(300, 4000);
            await AnimateCameraFunc(newPosition, durationMs);
        }
        else
        {
            MoveCameraAction(newPosition);
        }
    }

    [RelayCommand]
    private async Task NativeMapReady()
    {
        await Task.Yield();

        await UserDialogs.Instance.AlertAsync("Native map ready. You can now animate, or move camera or do smth else with it.");
    }

    [RelayCommand]
    private void CameraMoveStarted(CameraMoveReason reason)
    {
        CameraStartReason = $"Camera move started by {reason}";
    }

    [RelayCommand]
    private void CameraMove()
    {
        CameraMoving = $"Camera is moving right now";
    }

    [RelayCommand]
    private void CameraPositionChanged(CameraPosition position)
    {

    }

    [RelayCommand]
    private void CameraIdle(VisibleRegion visibleRegion)
    {
        CameraStartReason = null;
        CameraMoving = null;
    }

    [RelayCommand]
    private void RestrictPanning()
    {
        RestrictPanningToArea = VisibleRegion.Bounds;
    }

    [RelayCommand]
    private void CancelRestriction()
    {
        RestrictPanningToArea = null;
    }

    [RelayCommand]
    private void ResetMinMaxZoom()
    {
        ResetMinMaxZoomAction();
    }
}