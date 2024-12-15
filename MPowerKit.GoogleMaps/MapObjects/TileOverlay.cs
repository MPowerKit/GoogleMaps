namespace MPowerKit.GoogleMaps;

public class TileOverlay : VisualElement
{
    public virtual void ClearTileCache()
    {
        var native = NativeObjectAttachedProperty.GetNativeObject(this);
#if ANDROID
        (native as Android.Gms.Maps.Model.TileOverlay)?.ClearTileCache();
#else
        (native as Google.Maps.TileLayer)?.ClearTileCache();
#endif
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

    #region TileSize
    public int TileSize
    {
        get { return (int)GetValue(TileSizeProperty); }
        set { SetValue(TileSizeProperty, value); }
    }

    public static readonly BindableProperty TileSizeProperty =
        BindableProperty.Create(
            nameof(TileSize),
            typeof(int),
            typeof(TileOverlay),
            256);
    #endregion

    #region GetTileFunc
    public Func<Point, int, int, ImageSource?> GetTileFunc
    {
        get { return (Func<Point, int, int, ImageSource?>)GetValue(GetTileFuncProperty); }
        set { SetValue(GetTileFuncProperty, value); }
    }

    public static readonly BindableProperty GetTileFuncProperty =
        BindableProperty.Create(
            nameof(GetTileFunc),
            typeof(Func<Point, int, int, ImageSource?>),
            typeof(TileOverlay));
    #endregion
}