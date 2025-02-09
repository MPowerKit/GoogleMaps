namespace MPowerKit.GoogleMaps.Data;

public class GeoJsonMultiLineString : MultiGeometry
{
    public GeoJsonMultiLineString(IEnumerable<IGeometry> geometries)
        : base(geometries)
    {
    }

    public List<LineString> LineStrings => Geometries.OfType<LineString>().ToList();
}