using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

using MPowerKit.GoogleMaps;

namespace Sample.ViewModels;

public class TileOverlaysTemplateSelector : DataTemplateSelector
{
    public DataTemplate UrlTemplate { get; set; }
    public DataTemplate FileTemplate { get; set; }
    public DataTemplate StreamTemplate { get; set; }
    public DataTemplate ViewTemplate { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        if (item is TileOverlayDataObject tile)
        {
            return tile.Type switch
            {
                TileOverlayType.Url => UrlTemplate,
                TileOverlayType.File => FileTemplate,
                TileOverlayType.Stream => StreamTemplate,
                TileOverlayType.View => ViewTemplate,
            };
        }

        return null;
    }
}

public partial class TileOverlayDataObject : ObservableObject
{
    public TileOverlayType Type { get; set; }
}

public enum TileOverlayType
{
    Url,
    File,
    Stream,
    View
}

public partial class TileOverlaysSourcePageViewModel : ObservableObject
{
    public TileOverlaysSourcePageViewModel()
    {
        SetupItems();
    }

    [ObservableProperty]
    private ObservableCollection<TileOverlayDataObject> _items = [];

    [ObservableProperty]
    private CameraPosition _cameraPosition;

    private void SetupItems()
    {
        Items.Add(new TileOverlayDataObject() { Type = TileOverlayType.Url });
        Items.Add(new TileOverlayDataObject() { Type = TileOverlayType.File });
        Items.Add(new TileOverlayDataObject() { Type = TileOverlayType.Stream });
        Items.Add(new TileOverlayDataObject() { Type = TileOverlayType.View });
    }
}