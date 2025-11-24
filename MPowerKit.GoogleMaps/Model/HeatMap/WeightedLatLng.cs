namespace MPowerKit.GoogleMaps;

public readonly struct WeightedLatLng : IPointItem
{
    private static readonly SphericalMercatorProjection _projection = new(HeatmapTileProvider.WorldWidth);

    /// <summary>
    /// Constructor for WeightedLatLng.
    /// </summary>
    /// <param name="latLng">LatLng to add to the wrapper.</param>
    /// <param name="intensity">Intensity to use: should be greater than or equal to 0. Default value is 1.</param>
    public WeightedLatLng(Point latLng, float intensity = 1f)
    {
        Point = _projection.ToPoint(latLng);
        Intensity = intensity >= 0f ? intensity : 1f;
    }

    /// <summary>
    /// Gets the point associated with this WeightedLatLng.
    /// </summary>
    public Point Point { get; }

    /// <summary>
    /// Gets the intensity of this WeightedLatLng.
    /// </summary>
    public float Intensity { get; }
}