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
        return ToNativeTile(vItem, Handler!.MauiContext!, PlatformView!);
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

    protected virtual NTileOverlay ToNativeTile(VTileOverlay tileOverlay, IMauiContext context, MapView map)
    {
        var native = ToTileProvider(tileOverlay, context);
        native.ZIndex = tileOverlay.ZIndex;
        native.FadeIn = tileOverlay.FadeIn;
        native.Opacity = (float)tileOverlay.Opacity;

        if (tileOverlay.IsVisible)
        {
            native.Map = map;
        }

        return native;
    }

    protected virtual NTileOverlay ToTileProvider(VTileOverlay tileOverlay, IMauiContext context)
    {
        return new CommonTileProvider(tileOverlay, tileOverlay.TileSize, VirtualView!, context);
    }
}

public class CommonTileProvider : NTileOverlay
{
    private readonly VTileOverlay _tileOverlay;
    private readonly int _tileSize;
    private readonly GoogleMap _map;
    private readonly IMauiContext _mauiContext;

    public CommonTileProvider(VTileOverlay tileOverlay, int tileSize, GoogleMap map, IMauiContext mauiContext)
    {
        _tileOverlay = tileOverlay;
        _tileSize = tileSize;
        _map = map;
        _mauiContext = mauiContext;
    }

    public override async void RequestTile(nuint x, nuint y, nuint zoom, ITileReceiver receiver)
    {
        var image = await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            TileData data = new()
            {
                Point = new(x, y),
                Zoom = (int)zoom,
                TileSize = _tileSize
            };
            var template = _tileOverlay.TileTemplate;

            while (template is DataTemplateSelector selector)
            {
                template = selector.SelectTemplate(data, _map);
            }

            var source = template?.CreateContent() as ImageSource
                ?? _tileOverlay.TileProvider?.Invoke(new(x, y), (int)zoom, _tileSize);

            if (source is null) return null;

            source.BindingContext = data;

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