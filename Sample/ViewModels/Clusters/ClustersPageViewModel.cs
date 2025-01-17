using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Controls.UserDialogs.Maui;

using MPowerKit.GoogleMaps;

namespace Sample.ViewModels;

public partial class ClustersPageViewModel : ObservableObject
{
    public ClustersPageViewModel()
    {
        RandomizePins();
    }

    [ObservableProperty]
    private ObservableCollection<Pin> _pins = [];

    [ObservableProperty]
    private ClusterAlgorithm _selectedAlgorithm = ClusterAlgorithm.NonHierarchicalView;

    [RelayCommand]
    private void RandomizePins()
    {
        var minLatitude = -5d;
        var maxLatitude = 5d;
        var minLongitude = -5d;
        var maxLongitude = 5d;

        // Create a list to store the pins
        ObservableCollection<Pin> pins = [];

        // Create a random number generator
        var random = new Random();
        
        for (int i = 0; i < 1000; i++)
        {
            var latitude = minLatitude + (maxLatitude - minLatitude) * random.NextDouble();
            var longitude = minLongitude + (maxLongitude - minLongitude) * random.NextDouble();

            var pin = new Pin
            {
                Position = new(latitude, longitude),
                Title = $"{latitude} {longitude}",
                Icon = "map_pin.png"
            };

            pins.Add(pin);
        }

        Pins = pins;
    }

    [RelayCommand]
    private async Task ChangeAlgorithm()
    {
        var res = await UserDialogs.Instance.ActionSheetAsync(null, "Choose cluster algorithm", "Cancel",
            buttons: Enum.GetValues<ClusterAlgorithm>().Select(t => t.ToString()).ToArray());

        if (res == "Cancel" || SelectedAlgorithm.ToString() == res) return;

        SelectedAlgorithm = res switch
        {
            "None" => ClusterAlgorithm.None,
            "Grid" => ClusterAlgorithm.Grid,
            "GridPreCaching" => ClusterAlgorithm.GridPreCaching,
            "NonHierarchicalDistance" => ClusterAlgorithm.NonHierarchicalDistance,
            "NonHierarchicalDistancePreCaching" => ClusterAlgorithm.NonHierarchicalDistancePreCaching,
            "NonHierarchicalView" => ClusterAlgorithm.NonHierarchicalView,
        };
    }

    [RelayCommand]
    private async Task ClusterClicked(Cluster cluster)
    {
        await UserDialogs.Instance.AlertAsync($"Cluster {cluster.Title} {cluster.Snippet} was clicked");
    }

    [RelayCommand]
    private async Task ClusterInfoWindowClicked(Cluster cluster)
    {
        await UserDialogs.Instance.AlertAsync($"Cluster {cluster.Title} {cluster.Snippet} info window clicked");
    }

    [RelayCommand]
    private async Task ClusterInfoWindowLongClicked(Cluster cluster)
    {
        await UserDialogs.Instance.AlertAsync($"Cluster {cluster.Title} {cluster.Snippet} info window long clicked");
    }

    [RelayCommand]
    private async Task ClusterInfoWindowClosed(Cluster cluster)
    {
        await UserDialogs.Instance.AlertAsync($"Cluster {cluster.Title} {cluster.Snippet} info window closed");
    }
}