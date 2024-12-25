namespace MPowerKit.GoogleMaps;

public static class PolygonAttached
{
    #region Holes
    public static readonly BindableProperty HolesProperty =
        BindableProperty.CreateAttached(
            "Holes",
            typeof(IEnumerable<IEnumerable<Point>>),
            typeof(PolylineAttached),
            null);

    public static IEnumerable<IEnumerable<Point>> GetHoles(BindableObject view) => (IEnumerable<IEnumerable<Point>>)view.GetValue(HolesProperty);

    public static void SetHoles(BindableObject view, IEnumerable<IEnumerable<Point>> value) => view.SetValue(HolesProperty, value);
    #endregion
}