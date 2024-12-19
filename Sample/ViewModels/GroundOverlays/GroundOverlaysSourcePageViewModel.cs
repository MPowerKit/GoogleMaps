using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Controls.UserDialogs.Maui;

using MPowerKit.GoogleMaps;

namespace Sample.ViewModels;

public class GroundOverlaysTemplateSelector : DataTemplateSelector
{
    public DataTemplate UrlTemplate { get; set; }
    public DataTemplate FileTemplate { get; set; }
    public DataTemplate ViewTemplate { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        if (item is GroundOverlayDataObject overlay)
        {
            return overlay.Type switch
            {
                GroundOverlayType.Url => UrlTemplate,
                GroundOverlayType.File => FileTemplate,
                GroundOverlayType.View => ViewTemplate,
            };
        }

        return null;
    }
}

public partial class GroundOverlayDataObject : ObservableObject
{
    [ObservableProperty]
    private int _zIndex;
    public LatLngBounds Bounds { get; set; }
    public GroundOverlayType Type { get; set; }
}

public enum GroundOverlayType
{
    Url,
    File,
    View
}

public partial class GroundOverlaysSourcePageViewModel : ObservableObject
{
    public GroundOverlaysSourcePageViewModel()
    {
        SetupItems();
    }

    [ObservableProperty]
    private ObservableCollection<GroundOverlayDataObject> _items = [];

    private void SetupItems()
    {
        ObservableCollection<GroundOverlayDataObject> items = [];
        items.Add(new()
        {
            Bounds = new(new(-20, -20), new(10, 10)),
            Type = GroundOverlayType.Url
        });
        items.Add(new()
        {
            Bounds = new(new(-20, -10), new(10, 20)),
            Type = GroundOverlayType.File
        });
        items.Add(new()
        {
            Bounds = new(new(-10, -10), new(20, 20)),
            Type = GroundOverlayType.View
        });

        Items = items;
    }

    [RelayCommand]
    private async Task GroundOverlayClicked(GroundOverlayDataObject overlayData)
    {
        foreach (var item in Items)
        {
            item.ZIndex = 0;
        }
        overlayData.ZIndex = 10;
        await UserDialogs.Instance.AlertAsync($"{overlayData.Type} ground overlay was clicked");
    }
}