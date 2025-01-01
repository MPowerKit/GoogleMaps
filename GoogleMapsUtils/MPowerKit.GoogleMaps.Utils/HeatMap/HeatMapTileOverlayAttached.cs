namespace MPowerKit.GoogleMaps.Utils;

public static class HeatMapTileOverlayAttached
{
    #region NativeObject
    public static readonly BindableProperty NativeTileProviderProperty =
        BindableProperty.CreateAttached(
            "NativeTileProvider",
            typeof(object),
            typeof(HeatMapTileOverlayAttached),
            null);

    public static object GetNativeTileProvider(BindableObject view) => (object)view.GetValue(NativeTileProviderProperty);

    public static void SetNativeTileProvider(BindableObject view, object value) => view.SetValue(NativeTileProviderProperty, value);
    #endregion
}