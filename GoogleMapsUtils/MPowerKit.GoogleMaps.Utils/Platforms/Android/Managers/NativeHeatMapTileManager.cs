using Android.Gms.Maps.Model;

using NGradient = global::Android.Gms.Maps.Utils.HeatMaps.Gradient;
using NHeatmapTileProvider = global::Android.Gms.Maps.Utils.HeatMaps.HeatmapTileProvider;
using NTileOverlay = global::Android.Gms.Maps.Model.TileOverlay;
using NWeightedLatLng = global::Android.Gms.Maps.Utils.HeatMaps.WeightedLatLng;
using VTileOverlay = MPowerKit.GoogleMaps.TileOverlay;

namespace MPowerKit.GoogleMaps.Utils;

public class NativeHeatMapTileManager : TileOverlayManager
{
    protected override NTileOverlay AddItemToPlatformView(VTileOverlay vItem)
    {
        var options = ToOptions(vItem, Handler!.MauiContext!);
        var native = PlatformView!.AddTileOverlay(options);
        HeatMapTileOverlayAttached.SetNativeTileProvider(vItem, options.TileProvider!);
        return native;
    }

    protected override void ItemPropertyChanged(VTileOverlay vItem, NTileOverlay nItem, string? propertyName)
    {
        base.ItemPropertyChanged(vItem, nItem, propertyName);

        if (vItem is not HeatMapTileOverlay heatMapTile) return;

        if (propertyName == HeatMapTileOverlay.DataProperty.PropertyName)
        {
            OnDataChanged(heatMapTile, nItem);
        }
        else if (propertyName == HeatMapTileOverlay.RadiusProperty.PropertyName)
        {
            OnRadiusChanged(heatMapTile, nItem);
        }
        else if (propertyName == HeatMapTileOverlay.GradientProperty.PropertyName)
        {
            OnGradientChanged(heatMapTile, nItem);
        }
    }

    protected virtual void OnDataChanged(HeatMapTileOverlay vItem, NTileOverlay nItem)
    {
        var tileProvider = HeatMapTileOverlayAttached.GetNativeTileProvider(vItem) as NHeatmapTileProvider;
        tileProvider!.SetWeightedData(vItem.Data.ToNative());
    }

    protected virtual void OnRadiusChanged(HeatMapTileOverlay vItem, NTileOverlay nItem)
    {
        var tileProvider = HeatMapTileOverlayAttached.GetNativeTileProvider(vItem) as NHeatmapTileProvider;
        tileProvider!.SetRadius(vItem.Radius);
    }

    protected virtual void OnGradientChanged(HeatMapTileOverlay vItem, NTileOverlay nItem)
    {
        var tileProvider = HeatMapTileOverlayAttached.GetNativeTileProvider(vItem) as NHeatmapTileProvider;
        tileProvider!.SetGradient(vItem.Gradient.ToNative());
    }

    protected override ITileProvider ToTileProvider(VTileOverlay tileOverlay, IMauiContext context)
    {
        if (tileOverlay is not HeatMapTileOverlay heatMapTile) return base.ToTileProvider(tileOverlay, context);

        var builder = new NHeatmapTileProvider.Builder();

        if (heatMapTile.Gradient is not null)
        {
            builder.Gradient(heatMapTile.Gradient.ToNative());
        }

        builder.WeightedData(heatMapTile.Data.ToNative());
        builder.Radius(heatMapTile.Radius);

        return builder.Build()!;
    }
}

public static class HeatMapExtensions
{
    public static NGradient ToNative(this Gradient gradient)
    {
        return new NGradient(gradient.Colors, gradient.StartPoints, gradient.ColorMapSize);
    }

    public static NWeightedLatLng[] ToNative(this IEnumerable<WeightedLatLng> data)
    {
        return data.Select(p => p.ToNative()).ToArray();
    }

    public static NWeightedLatLng ToNative(this WeightedLatLng latLng)
    {
        return new NWeightedLatLng(latLng.Point.ToLatLng(), latLng.Intensity);
    }
}