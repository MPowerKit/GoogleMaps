using Google.Maps;

using NTileOverlay = Google.Maps.TileLayer;
using VTileOverlay = MPowerKit.GoogleMaps.TileOverlay;

namespace MPowerKit.GoogleMaps;

public class TileOverlayManager : ItemsMapFeatureManager<VTileOverlay, NTileOverlay, GoogleMap, MapView, GoogleMapHandler>
{
    protected override string GetVirtualViewItemsPropertyName()
    {
        return GoogleMap.TileOverlaysProperty.PropertyName;
    }

    protected override IEnumerable<VTileOverlay> GetVirtualViewItems()
    {
        return VirtualView!.TileOverlays;
    }

    protected override void RemoveItemFromPlatformView(NTileOverlay? nItem)
    {
        if (nItem is not null)
        {
            nItem.Map = null;
        }
    }

    protected override NTileOverlay AddItemToPlatformView(VTileOverlay vItem)
    {
        return vItem.ToNative(Handler!.MauiContext!, PlatformView!);
    }

    protected override void ItemPropertyChanged(VTileOverlay vItem, NTileOverlay nItem, string? propertyName)
    {
        base.ItemPropertyChanged(vItem, nItem, propertyName);

        if (propertyName == VisualElement.IsVisibleProperty.PropertyName)
        {
            OnIsVisibleChanged(vItem, nItem);
        }
        else if (propertyName == VisualElement.ZIndexProperty.PropertyName)
        {
            OnZIndexChanged(vItem, nItem);
        }
        else if (propertyName == VisualElement.OpacityProperty.PropertyName)
        {
            OnOpacityChanged(vItem, nItem);
        }
        else if (propertyName == VTileOverlay.FadeInProperty.PropertyName)
        {
            OnFadeInChanged(vItem, nItem);
        }
    }

    protected virtual void OnIsVisibleChanged(VTileOverlay vTileOverlay, NTileOverlay nTileOverlay)
    {
        nTileOverlay.Map = vTileOverlay.IsVisible ? PlatformView! : null;
    }

    protected virtual void OnZIndexChanged(VTileOverlay vTileOverlay, NTileOverlay nTileOverlay)
    {
        nTileOverlay.ZIndex = vTileOverlay.ZIndex;
    }

    protected virtual void OnOpacityChanged(VTileOverlay vTileOverlay, NTileOverlay nTileOverlay)
    {
        nTileOverlay.Opacity = (float)vTileOverlay.Opacity;
    }

    protected virtual void OnFadeInChanged(VTileOverlay vTileOverlay, NTileOverlay nTileOverlay)
    {
        nTileOverlay.FadeIn = vTileOverlay.FadeIn;
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
        return new CommonTileProvider(tileOverlay.TileProvider, tileOverlay.TileSize, context);
    }
}

public class CommonTileProvider : NTileOverlay
{
    private readonly Func<Point, int, int, ImageSource?> _provider;
    private readonly int _tileSize;
    private readonly IMauiContext _mauiContext;

    public CommonTileProvider(Func<Point, int, int, ImageSource?> provider, int tileSize, IMauiContext mauiContext)
    {
        _provider = provider;
        _tileSize = tileSize;
        _mauiContext = mauiContext;
    }

    public override async void RequestTile(nuint x, nuint y, nuint zoom, ITileReceiver receiver)
    {
        var image = await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            var source = _provider?.Invoke(new(x, y), (int)zoom, _tileSize);

            if (source is null) return null;
            if (source is NoTileImageSource) return Constants.TileLayerNoTile;

            try
            {
                var imageResult = await source.GetPlatformImageAsync(_mauiContext);
                if (imageResult?.Value is null) throw new Exception();

                return imageResult.Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cannot find or load resource");
            }

            return null;
        });

        receiver.ReceiveTile(x, y, zoom, image);
    }
}