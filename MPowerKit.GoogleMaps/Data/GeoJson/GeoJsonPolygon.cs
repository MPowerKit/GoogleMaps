namespace MPowerKit.GoogleMaps.Data;

/// <summary>
/// Represents a GeoJSON polygon geometry with outer and inner boundaries.
/// </summary>
public class GeoJsonPolygon : IDataPolygon
{
    /// <summary>
    /// Initializes a new GeoJsonPolygon with specified coordinates.
    /// </summary>
    /// <param name="coordinates">
    /// List of coordinate sequences where the first is the outer boundary,
    /// and subsequent are inner boundaries (holes).
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when coordinates is null</exception>
    public GeoJsonPolygon(IEnumerable<IEnumerable<Point>> coordinates)
    {
        var outer = coordinates?.FirstOrDefault();
        if (outer?.Count() is null or 0) throw new ArgumentNullException(nameof(coordinates));

        OuterBoundaryCoordinates = outer;
        InnerBoundaryCoordinates = coordinates?.Skip(1);
    }

    /// <summary>
    /// Gets an array of outer boundary coordinates.
    /// </summary>
    public IEnumerable<Point> OuterBoundaryCoordinates { get; }

    /// <summary>
    /// Gets an array of arrays of inner boundary coordinates.
    /// </summary>
    public IEnumerable<IEnumerable<Point>>? InnerBoundaryCoordinates { get; }
}