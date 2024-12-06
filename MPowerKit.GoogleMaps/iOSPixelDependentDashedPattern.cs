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
}