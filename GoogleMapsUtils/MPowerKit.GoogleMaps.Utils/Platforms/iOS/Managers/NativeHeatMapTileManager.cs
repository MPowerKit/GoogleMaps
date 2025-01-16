using Foundation;

using Google.Maps.Utils;

using Microsoft.Maui.Platform;

using NGradient = Google.Maps.Utils.Gradient;
using NTileOverlay = Google.Maps.TileLayer;
using NWeightedLatLng = Google.Maps.Utils.WeightedLatLng;
using VTileOverlay = MPowerKit.GoogleMaps.TileOverlay;

namespace MPowerKit.GoogleMaps.Utils;

public class NativeHeatMapTileManager : TileOverlayManager
{
    protected override void ItemPropertyChanged(VTileOverlay vItem, NTileOverlay nItem, string? propertyName)
    {
        base.ItemPropertyChanged(vItem, nItem, propertyName);

        if (vItem is not HeatMapTileOverlay heatMapTile) return;

        var native = nItem as HeatmapTileLayer;

        if (propertyName == HeatMapTileOverlay.DataProperty.PropertyName)
        {
            OnDataChanged(heatMapTile, native);
        }
        else if (propertyName == HeatMapTileOverlay.RadiusProperty.PropertyName)
        {
            OnRadiusChanged(heatMapTile, native);
        }
        else if (propertyName == HeatMapTileOverlay.GradientProperty.PropertyName)
        {
            OnGradientChanged(heatMapTile, native);
        }
    }

    protected virtual void OnDataChanged(HeatMapTileOverlay vItem, HeatmapTileLayer nItem)
    {
        nItem.WeightedData = vItem.Data.ToNative();
        nItem.ClearTileCache();
    }

    protected virtual void OnRadiusChanged(HeatMapTileOverlay vItem, HeatmapTileLayer nItem)
    {
        nItem.Radius = (nuint)vItem.Radius;
        nItem.ClearTileCache();
    }

    protected virtual void OnGradientChanged(HeatMapTileOverlay vItem, HeatmapTileLayer nItem)
    {
        nItem.Gradient = vItem.Gradient.ToNative();
        nItem.ClearTileCache();
    }

    protected override NTileOverlay ToTileProvider(VTileOverlay tileOverlay, IMauiContext context)
    {
        if (tileOverlay is not HeatMapTileOverlay heatMapTile) return base.ToTileProvider(tileOverlay, context);

        var native = new HeatmapTileLayer();

        if (heatMapTile.Gradient is not null)
        {
            native.Gradient = heatMapTile.Gradient.ToNative();
        }

        native.WeightedData = heatMapTile.Data.ToNative();
        native.Radius = (nuint)heatMapTile.Radius;
        native.MinimumZoomIntensity = 22;
        native.MaximumZoomIntensity = 22;

        return native;
    }
}

public static class HeatMapExtensions
{
    public static NGradient ToNative(this Gradient gradient)
    {
        return new NGradient(
            gradient.Colors.Select(c => Color.FromInt(c).ToPlatform()).ToArray(),
            gradient.StartPoints.Select(p => new NSNumber(p)).ToArray(),
            (nuint)gradient.ColorMapSize);
    }

    public static NWeightedLatLng[] ToNative(this IEnumerable<WeightedLatLng> data)
    {
        return data.Select(p => p.ToNative()).ToArray();
    }

    public static NWeightedLatLng ToNative(this WeightedLatLng latLng)
    {
        return new NWeightedLatLng(latLng.Point.ToCoord(), latLng.Intensity);
    }
}