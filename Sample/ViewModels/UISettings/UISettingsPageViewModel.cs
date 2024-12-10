using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Sample.ViewModels;

public partial class UISettingsPageViewModel : ObservableObject
{
    public UISettingsPageViewModel()
    {
        Setup();
    }

    [ObservableProperty]
    private bool _locationPermissionsGranted;

    [RelayCommand]
    private async void Setup()
    {
        var res = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        if (res is PermissionStatus.Granted)
        {
            LocationPermissionsGranted = true;
            return;
        }

        res = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        if (res is not PermissionStatus.Granted) return;

        LocationPermissionsGranted = true;
    }
}