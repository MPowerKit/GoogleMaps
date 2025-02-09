using System.Text;

namespace MPowerKit.GoogleMaps.Data;

/// <summary>
/// Represents a KML Polygon. Contains a single array of outer boundary coordinates and an array of
/// arrays for the inner boundary coordinates.
/// </summary>
public class KmlPolygon : IDataPolygon
{
    /// <summary>
    /// Creates a new KmlPolygon object.
    /// </summary>
    /// <param name="outerBoundaryCoordinates">Single array of outer boundary coordinates.</param>
    /// <param name="innerBoundaryCoordinates">Multiple arrays of inner boundary coordinates.</param>
    /// <exception cref="ArgumentNullException">Thrown if outerBoundaryCoordinates is null.</exception>
    public KmlPolygon(IEnumerable<Point> outerBoundaryCoordinates, IEnumerable<IEnumerable<Point>> innerBoundaryCoordinates)
    {
        OuterBoundaryCoordinates = outerBoundaryCoordinates
            ?? throw new ArgumentNullException(nameof(outerBoundaryCoordinates), "Outer boundary coordinates cannot be null");
        InnerBoundaryCoordinates = innerBoundaryCoordinates;
    }

    /// <summary>
    /// Gets an array of outer boundary coordinates.
    /// </summary>
    public IEnumerable<Point> OuterBoundaryCoordinates { get; }

    /// <summary>
    /// Gets an array of arrays of inner boundary coordinates.
    /// </summary>
    public IEnumerable<IEnumerable<Point>>? InnerBoundaryCoordinates { get; }

#if DEBUG
    /// <summary>
    /// Returns a string representation of the KmlPolygon.
    /// </summary>
    public override string ToString()
    {
        var sb = new StringBuilder(Constants.Polygon).Append('{').AppendLine();
        sb.Append("  outer coordinates=").Append(OuterBoundaryCoordinates).Append(',').AppendLine();
        sb.Append("  inner coordinates=").Append(InnerBoundaryCoordinates).AppendLine();
        sb.Append('}').AppendLine();
        return sb.ToString();
    }
#endif
}