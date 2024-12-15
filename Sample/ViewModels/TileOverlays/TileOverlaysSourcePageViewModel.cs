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
    public Func<Point, int, int, ImageSource?> GetTileFunc { get; set; }
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
        Items.Add(new TileOverlayDataObject() { GetTileFunc = GetUrlTiles, Type = TileOverlayType.Url });
        Items.Add(new TileOverlayDataObject() { GetTileFunc = GetFileTiles, Type = TileOverlayType.File });
        Items.Add(new TileOverlayDataObject() { GetTileFunc = GetStreamTiles, Type = TileOverlayType.Stream });
        Items.Add(new TileOverlayDataObject() { GetTileFunc = GetViewTiles, Type = TileOverlayType.View });
    }

    private ImageSource? GetUrlTiles(Point coord, int zoom, int tileSize)
    {
        if (CameraPosition.Zoom is not >= 2 or not <= 21)
        {
            // This means that there is no tile at current location
            return NoTileImageSource.Instance;
        }

        return ImageSource.FromUri(new Uri($"https://mt1.google.com/vt/lyrs=s&x={coord.X}&y={coord.Y}&z={zoom}")) as UriImageSource;
    }

    private ImageSource? GetFileTiles(Point coord, int zoom, int tileSize)
    {
        if (CameraPosition.Zoom is not >= 2 or not <= 21)
        {
            // This means that there is no tile at current location
            return NoTileImageSource.Instance;
        }

        return ImageSource.FromFile("tile.png") as FileImageSource;
    }

    private ImageSource? GetStreamTiles(Point coord, int zoom, int tileSize)
    {
        if (CameraPosition.Zoom is not >= 2 or not <= 21)
        {
            // This means that there is no tile at current location
            return NoTileImageSource.Instance;
        }

        return ImageSource.FromStream(ct => FileSystem.Current.OpenAppPackageFileAsync("bot.png")) as StreamImageSource;
    }

    private ImageSource? GetViewTiles(Point coord, int zoom, int tileSize)
    {
        if (CameraPosition.Zoom is not >= 2 or not <= 21)
        {
            // This means that there is no tile at current location
            return NoTileImageSource.Instance;
        }

        View? view = null;
        try
        {
            view = new Border()
            {
                WidthRequest = tileSize,
                HeightRequest = tileSize,
                Stroke = Colors.Red,
                StrokeThickness = 4d,
                Content = new Label()
                {
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    HorizontalTextAlignment = TextAlignment.Center,
                    TextColor = Colors.Black,
                    FontSize = 20d,
                    FontAttributes = FontAttributes.Bold,
                    Text = $"Point={coord}, Zoom={zoom}"
                }
            };

            return (ViewImageSource)view;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        // Providing a null tile result will tell the Map
        // that data is currently unavailable
        // but that it may be available in the future
        return null;
    }
}