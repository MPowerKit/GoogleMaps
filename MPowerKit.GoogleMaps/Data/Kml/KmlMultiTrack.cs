namespace MPowerKit.GoogleMaps.Data;

/// <summary>
/// Represents a KML MultiTrack, which is a collection of KmlTrack objects.
/// </summary>
public class KmlMultiTrack : MultiGeometry
{
    /// <summary>
    /// Creates a new KmlMultiTrack object.
    /// </summary>
    /// <param name="tracks">List of KmlTrack objects contained in the MultiTrack.</param>
    /// <exception cref="ArgumentNullException">Thrown if tracks is null.</exception>
    public KmlMultiTrack(IEnumerable<KmlTrack> tracks)
        : base(tracks)
    {
    }

    public List<KmlTrack> Tracks => Geometries.OfType<KmlTrack>().ToList();
}