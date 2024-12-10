using System.Collections.Specialized;

using Android.Gms.Maps.Model;

using Java.Net;

using GMap = Android.Gms.Maps.GoogleMap;
using NTileOverlay = Android.Gms.Maps.Model.TileOverlay;
using Point = Microsoft.Maui.Graphics.Point;
using VTileOverlay = MPowerKit.GoogleMaps.TileOverlay;

namespace MPowerKit.GoogleMaps;

public class TileOverlayManager : IMapFeatureManager<GoogleMap, GMap, GoogleMapHandler>
{
    protected GoogleMap? VirtualView { get; set; }
    protected GMap? NativeView { get; set; }
    protected GoogleMapHandler? Handler { get; set; }

    protected List<VTileOverlay> TileOverlays { get; set; } = [];

    public virtual void Connect(GoogleMap virtualView, GMap platformView, GoogleMapHandler handler)
    {
        VirtualView = virtualView;
        NativeView = platformView;
        Handler = handler;

        ResetTileOverlays();

        virtualView.PropertyChanged += VirtualView_PropertyChanged;
        virtualView.PropertyChanging += VirtualView_PropertyChanging;

        if (virtualView.TileOverlays is INotifyCollectionChanged tileOverlays)
        {
            tileOverlays.CollectionChanged += TileOverlays_CollectionChanged;
        }
    }

    public virtual void Disconnect(GoogleMap virtualView, GMap platformView, GoogleMapHandler handler)
    {
        virtualView.PropertyChanged -= VirtualView_PropertyChanged;
        virtualView.PropertyChanging -= VirtualView_PropertyChanging;

        if (virtualView.TileOverlays is INotifyCollectionChanged tileOverlays)
        {
            tileOverlays.CollectionChanged -= TileOverlays_CollectionChanged;
        }

        ClearTileOverlays();

        VirtualView = null;
        NativeView = null;
        Handler = null;
    }

    protected virtual void VirtualView_PropertyChanging(object sender, PropertyChangingEventArgs e)
    {
        if (e.PropertyName == GoogleMap.TileOverlaysProperty.PropertyName)
        {
            if (VirtualView!.TileOverlays is INotifyCollectionChanged tileOverlays)
            {
                tileOverlays.CollectionChanged -= TileOverlays_CollectionChanged;
            }

            ClearTileOverlays();
        }
    }

    protected virtual void VirtualView_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == GoogleMap.TileOverlaysProperty.PropertyName)
        {
            InitTileOverlays();

            if (VirtualView!.TileOverlays is INotifyCollectionChanged tileOverlays)
            {
                tileOverlays.CollectionChanged += TileOverlays_CollectionChanged;
            }
        }
    }

    protected virtual void TileOverlays_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                AddTileOverlays(e);
                break;
            case NotifyCollectionChangedAction.Remove:
                RemoveTileOverlays(e);
                break;
            case NotifyCollectionChangedAction.Replace:
                ReplaceTileOverlays(e);
                break;
            case NotifyCollectionChangedAction.Move:
                break;
            case NotifyCollectionChangedAction.Reset:
            default:
                ResetTileOverlays();
                break;
        }
    }

    protected virtual void ResetTileOverlays()
    {
        ClearTileOverlays();

        InitTileOverlays();
    }

    protected virtual void ClearTileOverlays()
    {
        RemoveTileOverlaysFromNativeMap([..TileOverlays]);
    }

    protected virtual void InitTileOverlays()
    {
        if (VirtualView!.TileOverlays?.Count() is null or 0) return;

        var tileOverlays = VirtualView!.TileOverlays.ToList();

        TileOverlays = new(tileOverlays.Count);

        AddTileOverlaysToNativeMap(tileOverlays);
    }

    protected virtual void TileOverlay_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        var tileOverlay = (sender as VTileOverlay)!;

        if (NativeObjectAttachedProperty.GetNativeObject(tileOverlay) is not NTileOverlay native) return;

        if (e.PropertyName == VisualElement.IsVisibleProperty.PropertyName)
        {
            native.Visible = tileOverlay.IsVisible;
        }
        else if (e.PropertyName == VisualElement.ZIndexProperty.PropertyName)
        {
            native.ZIndex = tileOverlay.ZIndex;
        }
        else if (e.PropertyName == VisualElement.OpacityProperty.PropertyName)
        {
            native.Transparency = 1f - (float)tileOverlay.Opacity;
        }
        else if (e.PropertyName == VTileOverlay.FadeInProperty.PropertyName)
        {
            native.FadeIn = tileOverlay.FadeIn;
        }
    }

    protected virtual void AddTileOverlays(NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems?.Count is null or 0 || NativeView is null) return;

        AddTileOverlaysToNativeMap(e.NewItems.Cast<VTileOverlay>());
    }

    protected virtual void RemoveTileOverlays(NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems?.Count is null or 0 || NativeView is null) return;

        RemoveTileOverlaysFromNativeMap(e.OldItems.Cast<VTileOverlay>());
    }

    protected virtual void ReplaceTileOverlays(NotifyCollectionChangedEventArgs e)
    {
        RemoveTileOverlays(e);
        AddTileOverlays(e);
    }

    protected virtual void AddTileOverlaysToNativeMap(IEnumerable<VTileOverlay> overlays)
    {
        foreach (var tileOverlay in overlays)
        {
            NativeObjectAttachedProperty.SetNativeObject(tileOverlay, NativeView!.AddTileOverlay(tileOverlay.ToNative(Handler!.MauiContext!)));
            tileOverlay.PropertyChanged += TileOverlay_PropertyChanged;
            TileOverlays.Add(tileOverlay);
        }
    }

    protected virtual void RemoveTileOverlaysFromNativeMap(IEnumerable<VTileOverlay> overlays)
    {
        foreach (var tileOverlay in overlays)
        {
            tileOverlay.PropertyChanged -= TileOverlay_PropertyChanged;

            var native = NativeObjectAttachedProperty.GetNativeObject(tileOverlay) as NTileOverlay;
            native?.Remove();

            TileOverlays.Remove(tileOverlay);
        }
    }
}

public static class TileOverlayExtensions
{
    public static TileOverlayOptions ToNative(this VTileOverlay tileOverlay, IMauiContext context)
    {
        var options = new TileOverlayOptions();

        options.InvokeZIndex(tileOverlay.ZIndex);
        options.Visible(tileOverlay.IsVisible);
        options.InvokeTransparency(1f - (float)tileOverlay.Opacity);
        options.InvokeFadeIn(tileOverlay.FadeIn);

        options.InvokeTileProvider(tileOverlay.ToTileProvider(context));

        return options;
    }

    public static ITileProvider ToTileProvider(this VTileOverlay tileOverlay, IMauiContext context)
    {
        return tileOverlay switch
        {
            UrlTileOverlay urlTileOverlay => new CustomUriTileProvider(urlTileOverlay.GetTileFunc, urlTileOverlay.TileSize),
            FileTileOverlay fileTileOverlay => new FileTileProvider(fileTileOverlay.GetTileFunc, fileTileOverlay.TileSize, context),
            ViewTileOverlay viewTileOverlay => new ViewTileProvider(viewTileOverlay.GetTileFunc, viewTileOverlay.TileSize, context),
            _ => throw new NotSupportedException("TileOverlay type not supported.")
        };
    }
}

public class FileTileProvider : Java.Lang.Object, ITileProvider
{
    private readonly Func<Point, int, ImageSource?> _getTileFunc;
    private readonly int _tileSize;
    private readonly IMauiContext _context;

    public FileTileProvider(Func<Point, int, ImageSource?> getTileFunc, int tileSize, IMauiContext context)
    {
        _getTileFunc = getTileFunc;
        _tileSize = tileSize;
        _context = context;
    }

    public Tile? GetTile(int x, int y, int zoom)
    {
        var fileSource = _getTileFunc?.Invoke(new(x, y), zoom) as FileImageSource;
        var file = fileSource!.File;

        if (fileSource.IsEmpty) return TileProvider.NoTile;

        try
        {
            var bitmap = file.GetBitmap(_context.Context!);

            return new Tile(_tileSize, _tileSize, bitmap.ToArray());
        }
        catch { Console.WriteLine($"Cannot find file or resource with name {file}"); }

        return TileProvider.NoTile;
    }
}

public class CustomUriTileProvider : UrlTileProvider
{
    private readonly Func<Point, int, ImageSource?> _getTileFunc;

    public CustomUriTileProvider(Func<Point, int, ImageSource?> getTileFunc, int tileSize) : base(tileSize, tileSize)
    {
        _getTileFunc = getTileFunc;
    }

    public override URL? GetTileUrl(int x, int y, int zoom)
    {
        var uri = _getTileFunc?.Invoke(new(x, y), zoom);

        return uri is UriImageSource uriImageSource && !uriImageSource.IsEmpty ? new URL(uriImageSource.Uri.AbsoluteUri) : null;
    }
}

public class ViewTileProvider : Java.Lang.Object, ITileProvider
{
    private readonly Func<Point, int, ImageSource?> _getTileFunc;
    private readonly int _tileSize;
    private readonly IMauiContext _mauiContext;

    public ViewTileProvider(Func<Point, int, ImageSource?> getTileFunc, int tileSize, IMauiContext mauiContext)
    {
        _getTileFunc = getTileFunc;
        _tileSize = tileSize;
        _mauiContext = mauiContext;
    }

    public Tile? GetTile(int x, int y, int zoom)
    {
        var view = _getTileFunc?.Invoke(new(x, y), zoom);

        if (view is ViewImageSource viewSource && !viewSource.IsEmpty)
        {
            using var bitmap = viewSource.View.ToBitmap(_mauiContext);
            return new Tile(_tileSize, _tileSize, bitmap.ToArray());
        }
        else return TileProvider.NoTile;
    }
}