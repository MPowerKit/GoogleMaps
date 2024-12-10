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

    [RelayCommand]
    private async Task ActivateIndoorLevel()
    {
        if (FocusedBuilding is null) return;

        var res = await UserDialogs.Instance.ActionSheetAsync("Choose level to activate", null, "Cancel",
            buttons: FocusedBuilding.Levels.Select(l => l.Name).ToArray());

        var level = FocusedBuilding.Levels.FirstOrDefault(l => l.Name == res);
        if (level is null) return;

        level.Activate();
    }
}