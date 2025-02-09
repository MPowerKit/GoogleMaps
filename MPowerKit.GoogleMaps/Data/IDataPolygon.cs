namespace MPowerKit.GoogleMaps.Data;

/// <summary>
/// An interface containing the common properties of Polygon geometries.
/// </summary>
/// <typeparam name="T">The type of Polygon (e.g., GeoJsonPolygon or KmlPolygon).</typeparam>
public interface IDataPolygon : IGeometry
{
    /// <summary>
    /// Gets an array of outer boundary coordinates.
    /// </summary>
    IEnumerable<Point> OuterBoundaryCoordinates { get; }

    /// <summary>
    /// Gets an array of arrays of inner boundary coordinates.
    /// </summary>
    IEnumerable<IEnumerable<Point>>? InnerBoundaryCoordinates { get; }
}