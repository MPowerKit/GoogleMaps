using CoreGraphics;

using Google.Maps;

using UIKit;

using NTileOverlay = Google.Maps.TileLayer;
using VTileOverlay = MPowerKit.GoogleMaps.TileOverlay;

namespace MPowerKit.GoogleMaps;

public class HeatMapTileManager : TileOverlayManager
{
    protected override NTileOverlay ToTileProvider(VTileOverlay tileOverlay, IMauiContext context)
    {
        if (tileOverlay is not HeatMapTileOverlay heatMapTile)
            return base.ToTileProvider(tileOverlay, context);

        return new HeatMapTileProvider(heatMapTile.TileProvider, heatMapTile.TileSize);
    }
}

public class HeatMapTileProvider : SyncTileLayer
{
    private readonly Func<Point, int, int, ImageSource?> _provider;
    private readonly int _tileSize;
    private readonly UIImage _emptyImage;

    public HeatMapTileProvider(Func<Point, int, int, ImageSource?> provider, int tileSize)
    {
        _provider = provider;
        _tileSize = tileSize;
        _emptyImage = EmptyImage(tileSize);
    }

    public override UIImage? Tile(nuint x, nuint y, nuint zoom)
    {
        ImageSource? source;
        try
        {
            source = _provider?.Invoke(new(x, y), (int)zoom, _tileSize);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return null;
        }

        if (source is NoTileImageSource)
        {
            return _emptyImage;
        }

        if (source is HeatMapImageSource heatMapSource)
        {
            return HeatMapImageSourceService.ImageFromPixels(heatMapSource.Pixels, heatMapSource.Size);
        }
        return null;
    }

    private UIImage EmptyImage(int size)
    {
        var renderer = new UIGraphicsImageRenderer(new(size, size));
        var image = renderer.CreateImage(ctx =>
        {
            UIColor.Clear.SetFill();
            ctx.FillRect(new CGRect(0, 0, size, size));
        });
        return image;
    }
}