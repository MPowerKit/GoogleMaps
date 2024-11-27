namespace MPowerKit.GoogleMaps;

public abstract class TileOverlay : VisualElement
{
    public Func<Point, int, ImageSource?> GetTileFunc { get; }
    public int TileSize { get; } = 256;

    public TileOverlay(Func<Point, int, ImageSource?> getTileFunc, int tileSize = 256)
    {
        GetTileFunc = getTileFunc;
        TileSize = tileSize;
    }

    #region FadeIn
    public bool FadeIn
    {
        get { return (bool)GetValue(FadeInProperty); }
        set { SetValue(FadeInProperty, value); }
    }

    public static readonly BindableProperty FadeInProperty =
        BindableProperty.Create(
            nameof(FadeIn),
            typeof(bool),
            typeof(TileOverlay),
            true);
    #endregion
}

public class UrlTileOverlay : TileOverlay
{
    public UrlTileOverlay(Func<Point, int, UriImageSource?> getTileFunc, int tileSize = 256) : base(getTileFunc, tileSize)
    {

    }
}

public class FileTileOverlay : TileOverlay
{
    public FileTileOverlay(Func<Point, int, FileImageSource?> getTileFunc, int tileSize = 256) : base(getTileFunc, tileSize)
    {

    }
}

public class ViewTileOverlay : TileOverlay
{
    public ViewTileOverlay(Func<Point, int, ViewImageSource?> getTileFunc, int tileSize = 256) : base(getTileFunc, tileSize)
    {

    }
}