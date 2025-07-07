namespace MPowerKit.GoogleMaps.Data;

public class GeoJsonMultiPoint : MultiGeometry
{
    public GeoJsonMultiPoint(IEnumerable<IGeometry> geometries)
        : base(geometries)
    {
    }

    public List<PointGeometry> Points => [.. Geometries.OfType<PointGeometry>()];
}