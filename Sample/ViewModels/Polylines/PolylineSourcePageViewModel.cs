using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Controls.UserDialogs.Maui;

namespace Sample.ViewModels;

public class PolylineTemplateSelector : DataTemplateSelector
{
    public DataTemplate DefaultTemplate { get; set; }
    public DataTemplate GradientTemplate { get; set; }
    public DataTemplate DashedTemplate { get; set; }
    public DataTemplate DisabledTemplate { get; set; }
    public DataTemplate TexturedTemplate { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        if (item is PolylineDataObject pData)
        {
            return pData.Type switch
            {
                PolylineType.Default => DefaultTemplate,
                PolylineType.Gradient => GradientTemplate,
                PolylineType.Dashed => DashedTemplate,
                PolylineType.Disabled => DisabledTemplate,
                PolylineType.Textured => TexturedTemplate,
            };
        }

        return null;
    }
}

public enum PolylineType
{
    Default,
    Gradient,
    Dashed,
    Disabled,
    Textured
}

public partial class PolylineDataObject : ObservableObject
{
    [ObservableProperty]
    private List<Sample.Models.Location> _path = [];
    [ObservableProperty]
    private PolylineType _type;
    [ObservableProperty]
    private int _zIndex;
}

public partial class PolylinesSourcePageViewModel : ObservableObject
{
    public PolylinesSourcePageViewModel()
    {
        SetupItems();
    }

    [ObservableProperty]
    private ObservableCollection<PolylineDataObject> _items = [];

    private void SetupItems()
    {
        ObservableCollection<PolylineDataObject> items = [];
        items.Add(new()
        {
            Path =
            [
                new Sample.Models.Location() { Latitude = 40, Longitude = -60 },
                new Sample.Models.Location() { Latitude = 0, Longitude = 20 },
                new Sample.Models.Location() { Latitude = -30, Longitude = -40 }
            ],
            Type = PolylineType.Default
        });
        items.Add(new()
        {
            Path =
            [
                new Sample.Models.Location() { Latitude = 0, Longitude = 0 },
                new Sample.Models.Location() { Latitude = -10, Longitude = -10 },
                new Sample.Models.Location() { Latitude = -10, Longitude = 10 }
            ],
            Type = PolylineType.Gradient
        });
        items.Add(new()
        {
            Path =
            [
                new Sample.Models.Location() { Latitude = 0, Longitude = 0 },
                new Sample.Models.Location() { Latitude = -10, Longitude = 10 },
                new Sample.Models.Location() { Latitude = 10, Longitude = 10 }
            ],
            Type = PolylineType.Disabled
        });
        items.Add(new()
        {
            Path =
            [
                new Sample.Models.Location() { Latitude = 0, Longitude = 0 },
                new Sample.Models.Location() { Latitude = 10, Longitude = 10 },
                new Sample.Models.Location() { Latitude = 10, Longitude = -10 }
            ],
            Type = PolylineType.Dashed
        });
        items.Add(new()
        {
            Path =
            [
                new Sample.Models.Location() { Latitude = 0, Longitude = 0 },
                new Sample.Models.Location() { Latitude = 10, Longitude = -10 },
                new Sample.Models.Location() { Latitude = -10, Longitude = -10 }
            ],
            Type = PolylineType.Textured
        });

        Items = items;
    }

    [RelayCommand]
    private async Task PolylineClicked(PolylineDataObject polylineData)
    {
        foreach (var item in Items)
        {
            item.ZIndex = 0;
        }
        polylineData.ZIndex = 10;
        await UserDialogs.Instance.AlertAsync($"{polylineData.Type} polyline was clicked");
    }
}