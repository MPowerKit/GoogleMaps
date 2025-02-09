namespace MPowerKit.GoogleMaps.Data;

public class GeoJsonMultiPolygon : MultiGeometry
{
    public GeoJsonMultiPolygon(IEnumerable<IGeometry> geometries)
        : base(geometries)
    {
    }

    public List<GeoJsonPolygon> Polygons => Geometries.OfType<GeoJsonPolygon>().ToList();
}