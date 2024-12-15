using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Controls.UserDialogs.Maui;

using MPowerKit.GoogleMaps;

namespace Sample.ViewModels;

public partial class TileOverlaysPageViewModel : ObservableObject
{
    public TileOverlaysPageViewModel()
    {
        SetupTiles();
    }

    [ObservableProperty]
    private ObservableCollection<TileOverlay> _tiles = [];

    [ObservableProperty]
    private CameraPosition _cameraPosition;

    [ObservableProperty]
    private string _selectedTileOverlay;

    [ObservableProperty]
    private double _opacity = 1;

    [ObservableProperty]
    private bool _isVisible = true;

    [ObservableProperty]
    private bool _fadeIn = true;

    private async void SetupTiles()
    {
        await Task.Yield();
        await ChangeTileOverlay();
    }

    [RelayCommand]
    private async Task ChangeTileOverlay()
    {
        var res = await UserDialogs.Instance.ActionSheetAsync(null,
            "Choose tile source",
            "Cancel",
            buttons:
            [
                "Url",
                "File",
                "Stream",
                "View",
                "Some Fun"
            ]);

        if (res == "Cancel") return;

        Tiles.Clear();

        var tile = res switch
        {
            "Url" => new TileOverlay(GetUrlTiles),
            "File" => new TileOverlay(GetFileTiles),
            "Stream" => new TileOverlay(GetStreamTiles),
            "View" => new TileOverlay(GetViewTiles),
            "Some Fun" => new TileOverlay(GetSomeFunTiles)
        };

        tile.SetBinding(TileOverlay.OpacityProperty, new Binding(nameof(Opacity), source: this));
        tile.SetBinding(TileOverlay.IsVisibleProperty, new Binding(nameof(IsVisible), source: this));
        tile.SetBinding(TileOverlay.FadeInProperty, new Binding(nameof(FadeIn), source: this));

        Tiles.Add(tile);
        SelectedTileOverlay = res;
        await Task.Yield();
        tile.ClearTileCache();
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

    private static int _index;
    private ImageSource? GetSomeFunTiles(Point coord, int zoom, int tileSize)
    {
        var option = _index % 4;
        _index++;

        return option switch
        {
            0 => GetUrlTiles(coord, zoom, tileSize),
            1 => GetFileTiles(coord, zoom, tileSize),
            2 => GetStreamTiles(coord, zoom, tileSize),
            3 => GetViewTiles(coord, zoom, tileSize),
        };
    }
}