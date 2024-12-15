namespace MPowerKit.GoogleMaps;

public class TileOverlay : VisualElement
{
    public Func<Point, int, int, ImageSource?> GetTileFunc { get; }
    public int TileSize { get; } = 256;

    public TileOverlay(Func<Point, int, int, ImageSource?> getTileFunc, int tileSize = 256)
    {
        GetTileFunc = getTileFunc;
        TileSize = tileSize;
    }

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
}