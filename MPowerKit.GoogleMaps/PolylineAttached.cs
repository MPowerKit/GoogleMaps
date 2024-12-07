namespace MPowerKit.GoogleMaps;

public static class PolylineAttached
{
    #region iOSPixelDependentDashedPattern
    public static readonly BindableProperty iOSPixelDependentDashedPatternProperty =
        BindableProperty.CreateAttached(
            "iOSPixelDependentDashedPattern",
            typeof(bool),
            typeof(PolylineAttached),
            true);

    public static bool GetiOSPixelDependentDashedPattern(BindableObject view) => (bool)view.GetValue(iOSPixelDependentDashedPatternProperty);

    public static void SetiOSPixelDependentDashedPattern(BindableObject view, bool value) => view.SetValue(iOSPixelDependentDashedPatternProperty, value);
    #endregion

    #region TextureStamp
    public static readonly BindableProperty TextureStampProperty =
        BindableProperty.CreateAttached(
            "TextureStamp",
            typeof(string),
            typeof(PolylineAttached),
            null);

    public static string GetTextureStamp(BindableObject view) => (string)view.GetValue(TextureStampProperty);

    public static void SetTextureStamp(BindableObject view, string value) => view.SetValue(TextureStampProperty, value);
    #endregion
}