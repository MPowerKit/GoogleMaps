using Android.Gms.Maps.Model;

using VTileOverlay = MPowerKit.GoogleMaps.TileOverlay;

namespace MPowerKit.GoogleMaps;

public class HeatMapTileManager : TileOverlayManager
{
    protected override ITileProvider ToTileProvider(VTileOverlay tileOverlay, IMauiContext context)
    {
        if (tileOverlay is not HeatMapTileOverlay heatMapTile) return base.ToTileProvider(tileOverlay, context);

        return new HeatMapTileProvider(heatMapTile.TileProvider, heatMapTile.TileSize);
    }
}

public class HeatMapTileProvider : Java.Lang.Object, ITileProvider
{
    private readonly Func<Point, int, int, ImageSource?> _provider;
    private readonly int _tileSize;

    public HeatMapTileProvider(Func<Point, int, int, ImageSource?> provider, int tileSize)
    {
        _provider = provider;
        _tileSize = tileSize;
    }

    public Tile? GetTile(int x, int y, int zoom)
    {
        ImageSource? source;
        try
        {
            source = _provider?.Invoke(new(x, y), zoom, _tileSize);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return null;
        }

        if (source is NoTileImageSource) return TileProvider.NoTile;

        if (source is HeatMapImageSource heatMapSource)
        {
            using var bitmap = HeatMapImageSourceService.GetBitmapFromPixels(heatMapSource.Pixels, heatMapSource.Size);

            return new Tile(_tileSize, _tileSize, bitmap.ToArray());
        }

        return null;
    }
}