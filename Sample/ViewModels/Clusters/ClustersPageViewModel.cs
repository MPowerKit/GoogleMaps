using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

using MPowerKit.GoogleMaps;

namespace Sample.ViewModels;

public partial class ClustersPageViewModel : ObservableObject
{
    public ClustersPageViewModel()
    {
        SetupPins();
    }

    [ObservableProperty]
    private ObservableCollection<Pin> _pins = [];

    private void SetupPins()
    {
        // Define the geographic bounds of Poland
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
}