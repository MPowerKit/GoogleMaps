namespace MPowerKit.GoogleMaps.Data;

/// <summary>
/// Represents a KML track, which is a specialized type of KML line string with timestamps and properties.
/// </summary>
public class KmlTrack : LineString
{
    private readonly Dictionary<string, string> _properties;

    /// <summary>
    /// Creates a new KmlTrack object.
    /// </summary>
    /// <param name="coordinates">List of coordinates.</param>
    /// <param name="properties">Dictionary of properties.</param>
    public KmlTrack(IEnumerable<Point> coordinates, Dictionary<string, string> properties)
        : base(coordinates)
    {
        _properties = properties ?? [];
    }

    /// <summary>
    /// Gets the dictionary of properties associated with the track.
    /// </summary>
    public Dictionary<string, string> Properties => _properties;
}