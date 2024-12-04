using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Controls.UserDialogs.Maui;

using MPowerKit.GoogleMaps;

namespace Sample.ViewModels;

public class CircleTemplateSelector : DataTemplateSelector
{
    public DataTemplate TransparentTemplate { get; set; }
    public DataTemplate OpaqueTemplate { get; set; }
    public DataTemplate DisabledTemplate { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        if (item is CircleDataObject circle)
        {
            return circle.Type switch
            {
                CircleType.Transparent => TransparentTemplate,
                CircleType.Opaque => OpaqueTemplate,
                CircleType.Disabled => DisabledTemplate,
            };
        }

        return null;
    }
}

public enum CircleType
{
    Transparent,
    Opaque,
    Disabled
}

public partial class CircleDataObject : ObservableObject
{
    [ObservableProperty]
    private Sample.Models.Location _location = new();

    [ObservableProperty]
    private CircleType _type;

    [ObservableProperty]
    private double _radius;

    [ObservableProperty]
    private int _zIndex;
}

public partial class CirclesSourcePageViewModel : ObservableObject
{
    public CirclesSourcePageViewModel()
    {
        SetupItems();
    }

    [ObservableProperty]
    private ObservableCollection<CircleDataObject> _items = [];

    private void SetupItems()
    {
        ObservableCollection<CircleDataObject> items = [];
        items.Add(new()
        {
            Radius = Distance.FromKMeters(2000),
            Type = CircleType.Transparent
        });
        items.Add(new()
        {
            Radius = Distance.FromKMeters(700),
            Location = new() { Latitude = -10, Longitude = -10 },
            Type = CircleType.Opaque
        });
        items.Add(new()
        {
            Radius = Distance.FromKMeters(300),
            Location = new() { Latitude = 20, Longitude = 20 },
            Type = CircleType.Disabled
        });

        Items = items;
    }

    [RelayCommand]
    private async Task CircleClicked(CircleDataObject circleData)
    {
        foreach (var item in Items)
        {
            item.ZIndex = 0;
        }
        circleData.ZIndex = 10;
        await UserDialogs.Instance.AlertAsync($"{circleData.Type} circle was clicked");
    }
}