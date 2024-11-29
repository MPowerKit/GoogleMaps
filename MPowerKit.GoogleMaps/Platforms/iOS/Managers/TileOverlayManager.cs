using System.Collections.Specialized;

using Foundation;

using Google.Maps;

using UIKit;

using NTileOverlay = Google.Maps.TileLayer;
using VTileOverlay = MPowerKit.GoogleMaps.TileOverlay;

namespace MPowerKit.GoogleMaps;

public class TileOverlayManager : IMapFeatureManager<GoogleMap, MapView, GoogleMapHandler>
{
    protected GoogleMap? VirtualView { get; set; }
    protected MapView? NativeView { get; set; }
    protected GoogleMapHandler? Handler { get; set; }

    protected List<VTileOverlay> TileOverlays { get; set; } = [];

    public virtual void Connect(GoogleMap virtualView, MapView platformView, GoogleMapHandler handler)
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

    public virtual void Disconnect(GoogleMap virtualView, MapView platformView, GoogleMapHandler handler)
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
            native.Map = tileOverlay.IsVisible ? NativeView! : null;
        }
        else if (e.PropertyName == VisualElement.ZIndexProperty.PropertyName)
        {
            native.ZIndex = tileOverlay.ZIndex;
        }
        else if (e.PropertyName == VisualElement.OpacityProperty.PropertyName)
        {
            native.Opacity = (float)tileOverlay.Opacity;
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
            NativeObjectAttachedProperty.SetNativeObject(tileOverlay, tileOverlay.ToNative(Handler!.MauiContext!, NativeView!));
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

            if (native is not null)
            {
                native.Map = null;
            }

            TileOverlays.Remove(tileOverlay);
        }
    }
}

public static class TileOverlayExtensions
{
    public static NTileOverlay ToNative(this VTileOverlay tileOverlay, IMauiContext context, MapView map)
    {
        var native = tileOverlay.ToTileLayer(context);
        native.TileSize = tileOverlay.TileSize;

        if (tileOverlay.IsVisible)
        {
            native.Map = map;
        }

        return native;
    }

    public static NTileOverlay ToTileLayer(this VTileOverlay tileOverlay, IMauiContext context)
    {
        return tileOverlay switch
        {
            UrlTileOverlay urlTileOverlay => UrlTileLayer.FromUrlConstructor((x, y, z) => tileOverlay.GetTileFunc.ToUrl((int)x, (int)y, (int)z)),
            FileTileOverlay fileTileOverlay => new FileTileLayer(fileTileOverlay.GetTileFunc, context),
            ViewTileOverlay viewTileOverlay => new ViewTileProvider(viewTileOverlay.GetTileFunc, viewTileOverlay.TileSize, context),
            _ => throw new NotSupportedException("VTileOverlay type not supported.")
        };
    }

    public static NSUrl? ToUrl(this Func<Point, int, ImageSource?> getTileFunc, int x, int y, int zoom)
    {
        var uri = getTileFunc?.Invoke(new(x, y), zoom);

        return uri is UriImageSource uriImageSource && !uriImageSource.IsEmpty ? new NSUrl(uriImageSource.Uri.AbsoluteUri) : null;
    }
}

public class FileTileLayer : NTileOverlay
{
    private readonly Func<Point, int, ImageSource?> _getTileFunc;
    private readonly IMauiContext _context;

    public FileTileLayer(Func<Point, int, ImageSource?> getTileFunc, IMauiContext context)
    {
        _getTileFunc = getTileFunc;
        _context = context;
    }

    public override async void RequestTile(nuint x, nuint y, nuint zoom, ITileReceiver receiver)
    {
        var fileSource = _getTileFunc?.Invoke(new(x, y), (int)zoom) as IFileImageSource;
        var file = fileSource!.File;

        if (fileSource.IsEmpty)
        {
            receiver.ReceiveTile(x, y, zoom, null);
            return;
        }

        try
        {
            var image = await fileSource.GetPlatformImageAsync(_context);

            if (image?.Value is null)
            {
                receiver.ReceiveTile(x, y, zoom, null);
                return;
            }

            receiver.ReceiveTile(x, y, zoom, image.Value);
        }
        catch { Console.WriteLine($"Cannot find file or resource with name {file}"); }
    }
}

public class ViewTileProvider : SyncTileLayer
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

    public override UIImage? Tile(nuint x, nuint y, nuint zoom)
    {
        var view = _getTileFunc?.Invoke(new(x, y), (int)zoom);

        if (view is ViewImageSource viewSource && !viewSource.IsEmpty)
        {
            return viewSource.View!.ToImage(_mauiContext);
        }

        return null;
    }
}