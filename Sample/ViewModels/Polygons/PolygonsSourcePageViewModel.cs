using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Controls.UserDialogs.Maui;

namespace Sample.ViewModels;

public class PolygonTemplateSelector : DataTemplateSelector
{
    public DataTemplate TransparentTemplate { get; set; }
    public DataTemplate OpaqueTemplate { get; set; }
    public DataTemplate DisabledTemplate { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        if (item is PolygonDataObject polygon)
        {
            return polygon.Type switch
            {
                PolygonType.Transparent => TransparentTemplate,
                PolygonType.Opaque => OpaqueTemplate,
                PolygonType.Disabled => DisabledTemplate,
            };
        }

        return null;
    }
}

public enum PolygonType
{
    Transparent,
    Opaque,
    Disabled
}

public partial class PolygonDataObject : ObservableObject
{
    [ObservableProperty]
    private List<Sample.Models.Location> _path = [];

    [ObservableProperty]
    private PolygonType _type;

    [ObservableProperty]
    private int _zIndex;
}

public partial class PolygonsSourcePageViewModel : ObservableObject
{
    public PolygonsSourcePageViewModel()
    {
        SetupItems();
    }

    [ObservableProperty]
    private ObservableCollection<PolygonDataObject> _items = [];

    private void SetupItems()
    {
        ObservableCollection<PolygonDataObject> items = [];
        items.Add(new()
        {
            Path =
            [
                new Sample.Models.Location() { Latitude = 40, Longitude = -60 },
                new Sample.Models.Location() { Latitude = 0, Longitude = 20 },
                new Sample.Models.Location() { Latitude = -30, Longitude = -40 }
            ],
            Type = PolygonType.Transparent
        });
        items.Add(new()
        {
            Path =
            [
                new Sample.Models.Location() { Latitude = 0, Longitude = 0 },
                new Sample.Models.Location() { Latitude = -10, Longitude = -10 },
                new Sample.Models.Location() { Latitude = -10, Longitude = 10 }
            ],
            Type = PolygonType.Opaque
        });
        items.Add(new()
        {
            Path =
            [
                new Sample.Models.Location() { Latitude = 0, Longitude = 0 },
                new Sample.Models.Location() { Latitude = 10, Longitude = 10 },
                new Sample.Models.Location() { Latitude = -10, Longitude = 10 }
            ],
            Type = PolygonType.Disabled
        });

        Items = items;
    }

    [RelayCommand]
    private async Task PolygonClicked(PolygonDataObject polygonData)
    {
        foreach (var item in Items)
        {
            item.ZIndex = 0;
        }
        polygonData.ZIndex = 10;
        await UserDialogs.Instance.AlertAsync($"{polygonData.Type} polygon was clicked");
    }
}