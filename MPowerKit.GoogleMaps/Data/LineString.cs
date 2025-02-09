using System.Text;

namespace MPowerKit.GoogleMaps.Data;

/// <summary>
/// An abstraction that shares the common properties of LineString geometries.
/// </summary>
public class LineString : IGeometry
{
    /// <summary>
    /// Gets the list of coordinates associated with the LineString.
    /// </summary>
    public List<Point> Points { get; }

    /// <summary>
    /// Creates a new LineString object.
    /// </summary>
    /// <param name="coordinates">The list of coordinates representing the LineString.</param>
    /// <exception cref="ArgumentNullException">Thrown if the coordinates parameter is null.</exception>
    public LineString(IEnumerable<Point> coordinates)
    {
        Points = coordinates?.ToList()
            ?? throw new ArgumentNullException(nameof(coordinates), "Coordinates cannot be null.");
    }

#if DEBUG
    /// <summary>
    /// Returns a string representation of the LineString object.
    /// </summary>
    /// <returns>A formatted string describing the LineString.</returns>
    public override string ToString()
    {
        var sb = new StringBuilder(nameof(LineString)).Append('{').AppendLine();
        sb.Append("  coordinates=").AppendJoin(", ", Points).AppendLine();
        sb.Append('}').AppendLine();
        return sb.ToString();
    }
#endif
}