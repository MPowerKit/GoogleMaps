using System.Text;

namespace MPowerKit.GoogleMaps.Data;

/// <summary>
/// An abstraction that shares the common properties of multi-geometry types.
/// </summary>
public class MultiGeometry : IGeometry
{
    /// <summary>
    /// Gets a list of Geometry objects.
    /// </summary>
    public List<IGeometry> Geometries { get; }

    /// <summary>
    /// Creates a new MultiGeometry object.
    /// </summary>
    /// <param name="geometries">A list of geometries (e.g., Polygons, LineStrings, Points).</param>
    /// <exception cref="ArgumentNullException">Thrown if the geometries parameter is null.</exception>
    public MultiGeometry(IEnumerable<IGeometry> geometries)
    {
        Geometries = geometries?.ToList()
            ?? throw new ArgumentNullException(nameof(geometries), "Geometries cannot be null.");
    }

#if DEBUG
    /// <summary>
    /// Returns a string representation of the MultiGeometry object.
    /// </summary>
    /// <returns>A formatted string describing the geometry.</returns>
    public override string ToString()
    {
        var sb = new StringBuilder(nameof(MultiGeometry)).Append('{').AppendLine();
        sb.Append("  ").AppendJoin(", ", Geometries).AppendLine();
        sb.Append('}').AppendLine();
        return sb.ToString();
    }
#endif
}