using Android.Gms.Maps.Model;
using Android.Graphics.Drawables;

using GMap = Android.Gms.Maps.GoogleMap;
using NTileOverlay = Android.Gms.Maps.Model.TileOverlay;
using Point = Microsoft.Maui.Graphics.Point;
using VTileOverlay = MPowerKit.GoogleMaps.TileOverlay;

namespace MPowerKit.GoogleMaps;

public class TileOverlayManager : ItemsMapFeatureManager<VTileOverlay, NTileOverlay, GoogleMap, GMap, GoogleMapHandler>
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
        nItem?.Remove();
    }

    protected override NTileOverlay AddItemToPlatformView(VTileOverlay vItem)
    {
        return PlatformView!.AddTileOverlay(vItem.ToNative(Handler!.MauiContext!));
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
        return new CommonTileProvider(tileOverlay.GetTileFunc, tileOverlay.TileSize, context);
    }
}

public class CommonTileProvider : Java.Lang.Object, ITileProvider
{
    private readonly Func<Point, int, int, ImageSource?> _getTileFunc;
    private readonly int _tileSize;
    private readonly IMauiContext _mauiContext;

    public CommonTileProvider(Func<Point, int, int, ImageSource?> getTileFunc, int tileSize, IMauiContext mauiContext)
    {
        _getTileFunc = getTileFunc;
        _tileSize = tileSize;
        _mauiContext = mauiContext;
    }

    public Tile? GetTile(int x, int y, int zoom)
    {
        var source = _getTileFunc?.Invoke(new(x, y), zoom, _tileSize);

        if (source is null) return null;
        if (source is NoTileImageSource) return TileProvider.NoTile;

        try
        {
            var imageResult = Task.Run(() => source.GetPlatformImageAsync(_mauiContext)).Result;
            if (imageResult?.Value is null) throw new Exception();

            using var bitmap = (imageResult.Value as BitmapDrawable)!.Bitmap!;

            return new Tile(_tileSize, _tileSize, bitmap.ToArray());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Cannot find or load resource");
        }

        return null;
    }
}