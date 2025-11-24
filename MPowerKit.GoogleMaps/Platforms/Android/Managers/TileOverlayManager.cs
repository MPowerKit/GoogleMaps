using Android.Gms.Maps.Model;
using Android.Graphics.Drawables;

using GMap = Android.Gms.Maps.GoogleMap;
using NTileOverlay = Android.Gms.Maps.Model.TileOverlay;
using VTileOverlay = MPowerKit.GoogleMaps.TileOverlay;

namespace MPowerKit.GoogleMaps;

public class TileOverlayManager : ItemsMapFeatureManager<VTileOverlay, NTileOverlay, GMap>
{
    protected override IEnumerable<VTileOverlay>? VirtualViewItems => VirtualView!.TileOverlays;
    protected override string VirtualViewItemsPropertyName => GoogleMap.TileOverlaysProperty.PropertyName;

    protected override void RemoveItemFromPlatformView(NTileOverlay? nItem)
    {
        nItem?.Remove();
    }

    protected override NTileOverlay? AddItemToPlatformView(VTileOverlay vItem)
    {
        return PlatformView!.AddTileOverlay(ToOptions(vItem, Handler!.MauiContext!));
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
        nTileOverlay.Visible = vTileOverlay.IsVisible;
    }

    protected virtual void OnZIndexChanged(VTileOverlay vTileOverlay, NTileOverlay nTileOverlay)
    {
        nTileOverlay.ZIndex = vTileOverlay.ZIndex;
    }

    protected virtual void OnOpacityChanged(VTileOverlay vTileOverlay, NTileOverlay nTileOverlay)
    {
        nTileOverlay.Transparency = 1f - (float)vTileOverlay.Opacity;
    }

    protected virtual void OnFadeInChanged(VTileOverlay vTileOverlay, NTileOverlay nTileOverlay)
    {
        nTileOverlay.FadeIn = vTileOverlay.FadeIn;
    }

    protected virtual TileOverlayOptions ToOptions(VTileOverlay tileOverlay, IMauiContext context)
    {
        var options = new TileOverlayOptions();

        options.InvokeZIndex(tileOverlay.ZIndex);
        options.Visible(tileOverlay.IsVisible);
        options.InvokeTransparency(1f - (float)tileOverlay.Opacity);
        options.InvokeFadeIn(tileOverlay.FadeIn);

        options.InvokeTileProvider(ToTileProvider(tileOverlay, context));

        return options;
    }

    protected virtual ITileProvider ToTileProvider(VTileOverlay tileOverlay, IMauiContext context)
    {
        return new CommonTileProvider(tileOverlay, tileOverlay.TileSize, VirtualView!, context);
    }
}

public class CommonTileProvider : Java.Lang.Object, ITileProvider
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

    public Tile? GetTile(int x, int y, int zoom)
    {
        TileData data = new()
        {
            Point = new(x, y),
            Zoom = zoom,
            TileSize = _tileSize
        };

        var template = _tileOverlay.TileTemplate;

        while (template is DataTemplateSelector selector)
        {
            template = selector.SelectTemplate(data, _map);
        }

        var source = template?.CreateContent() as ImageSource
            ?? _tileOverlay.TileProvider?.Invoke(new(x, y), zoom, _tileSize);

        if (source is null) return null;

        source.BindingContext = data;

        if (source is NoTileImageSource) return TileProvider.NoTile;

        try
        {
            var imageResult = Task.Run(() => source.GetPlatformImageAsync(_mauiContext)).Result;
            if (imageResult?.Value is null) throw new Exception();

            using var bitmap = (imageResult.Value as BitmapDrawable)!.Bitmap!;

            return new(_tileSize, _tileSize, bitmap.ToArray());
        }
        catch
        {
            Console.WriteLine($"Cannot find or load resource");
        }

        return null;
    }
}