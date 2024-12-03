using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Controls.UserDialogs.Maui;

using MPowerKit.GoogleMaps;

namespace Sample.ViewModels;

public class PinTemplateSelector : DataTemplateSelector
{
    public DataTemplate LocationTemplate { get; set; }
    public DataTemplate CarTemplate { get; set; }
    public DataTemplate DraggableTemplate { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        if (item is PinDataObject pinData)
        {
            return pinData.Type switch
            {
                PinType.Location => LocationTemplate,
                PinType.Car => CarTemplate,
                PinType.Draggable => DraggableTemplate
            };
        }

        return null;
    }
}

public partial class PinDataObject : ObservableObject
{
    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _adress;

    [ObservableProperty]
    private Sample.Models.Location _location = new();

    [ObservableProperty]
    private double _orientation;

    [ObservableProperty]
    private PinType _type;
}

public enum PinType
{
    Location,
    Car,
    Draggable
}

public partial class PinsSourcePageViewModel : ObservableObject
{
    public PinsSourcePageViewModel()
    {
        SetupItems();
    }

    [ObservableProperty]
    private ObservableCollection<PinDataObject> _items = [];

    private void SetupItems()
    {
        ObservableCollection<PinDataObject> items = [];
        items.Add(new()
        {
            Name = "Location",
            Adress = "Adress of pin",
            Orientation = 95,
            Type = PinType.Location
        });
        items.Add(new()
        {
            Name = "Car",
            Adress = "Name of driver",
            Orientation = 160,
            Location = new() { Latitude = 0, Longitude = 10 },
            Type = PinType.Car
        });
        items.Add(new()
        {
            Name = "Draggable",
            Location = new() { Latitude = 10, Longitude = -15 },
            Type = PinType.Draggable
        });
        Items = items;
    }

    [ObservableProperty]
    private string _status;

    [ObservableProperty]
    private Pin _selectedPin;
    partial void OnSelectedPinChanged(Pin? oldValue, Pin newValue)
    {
        if (oldValue is not null) oldValue.ZIndex = 0;
        if (newValue is not null) newValue.ZIndex = 10;
    }

    [ObservableProperty]
    private PinDataObject _selectedPinData;

    [RelayCommand]
    private async Task PinClicked(PinDataObject pinData)
    {
        await UserDialogs.Instance.AlertAsync($"{pinData.Name} pin clicked");
    }

    [RelayCommand]
    private async Task PinDragStarted(PinDataObject pinData)
    {
        Status = $"{pinData.Name} drag started";
    }

    [RelayCommand]
    private async Task PinDrag(PinDataObject pinData)
    {
        Status = $"{pinData.Name} draggin in process";
    }

    [RelayCommand]
    private async Task PinDragEnded(PinDataObject pinData)
    {
        Status = null;
        await UserDialogs.Instance.AlertAsync($"{pinData.Name} drag ended");
    }

    [RelayCommand]
    private async Task InfoWindowClicked(PinDataObject pinData)
    {
        await UserDialogs.Instance.AlertAsync($"{pinData.Name} info window clicked");
    }

    [RelayCommand]
    private async Task InfoWindowLongClicked(PinDataObject pinData)
    {
        await UserDialogs.Instance.AlertAsync($"{pinData.Name} info window long clicked");
    }

    [RelayCommand]
    private async Task InfoWindowClosed(PinDataObject pinData)
    {
        await UserDialogs.Instance.AlertAsync($"{pinData.Name} info window closed");
    }
}