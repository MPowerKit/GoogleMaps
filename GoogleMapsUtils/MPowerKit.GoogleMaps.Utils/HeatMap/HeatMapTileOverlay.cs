namespace MPowerKit.GoogleMaps.Utils;

public class HeatMapTileOverlay : TileOverlay
{
    public HeatMapTileOverlay()
    {
        Opacity = 0.7;
    }

    #region Data
    public IEnumerable<WeightedLatLng> Data
    {
        get { return (IEnumerable<WeightedLatLng>)GetValue(DataProperty); }
        set { SetValue(DataProperty, value); }
    }

    public static readonly BindableProperty DataProperty =
        BindableProperty.Create(
            nameof(Data),
            typeof(IEnumerable<WeightedLatLng>),
            typeof(HeatMapTileOverlay));
    #endregion

    #region Radius
    public int Radius
    {
        get { return (int)GetValue(RadiusProperty); }
        set { SetValue(RadiusProperty, value); }
    }

    public static readonly BindableProperty RadiusProperty =
        BindableProperty.Create(
            nameof(Radius),
            typeof(int),
            typeof(HeatMapTileOverlay),
            20);
    #endregion

    #region Gradient
    public Gradient Gradient
    {
        get { return (Gradient)GetValue(GradientProperty); }
        set { SetValue(GradientProperty, value); }
    }

    public static readonly BindableProperty GradientProperty =
        BindableProperty.Create(
            nameof(FadeIn),
            typeof(Gradient),
            typeof(HeatMapTileOverlay));
    #endregion
}