using System.Text;

namespace MPowerKit.GoogleMaps.Data;

/// <summary>
/// An abstraction that shares the common properties of Point geometries.
/// </summary>
public class PointGeometry : IGeometry
{
    public Point Position { get; }

    /// <summary>
    /// Creates a new Point object.
    /// </summary>
    /// <param name="coordinates">The coordinates of the Point.</param>
    public PointGeometry(Point coordinates)
    {
        Position = coordinates;
    }

#if DEBUG
    /// <summary>
    /// Returns a string representation of the Point object.
    /// </summary>
    /// <returns>A formatted string describing the Point.</returns>
    public override string ToString()
    {
        var sb = new StringBuilder(nameof(PointGeometry)).Append('{').AppendLine();
        sb.Append("  coordinates=").Append(Position).AppendLine();
        sb.Append('}').AppendLine();
        return sb.ToString();
    }
#endif
}