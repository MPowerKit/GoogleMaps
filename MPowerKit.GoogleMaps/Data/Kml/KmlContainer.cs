namespace MPowerKit.GoogleMaps.Data;

/// <summary>
/// Represents a KML Document or Folder.
/// </summary>
public class KmlContainer : IHasProperty
{
    private readonly Dictionary<string, string> _properties;

    /// <summary>
    /// Initializes a new instance of the <see cref="KmlContainer"/> class.
    /// </summary>
    /// <param name="properties">The properties of the container.</param>
    /// <param name="styles">The styles of the container.</param>
    /// <param name="placemarks">The placemarks of the container.</param>
    /// <param name="styleMaps">The style maps of the container.</param>
    /// <param name="containers">The nested containers of the container.</param>
    /// <param name="groundOverlays">The ground overlays of the container.</param>
    /// <param name="id">The ID of the container.</param>
    public KmlContainer(
        Dictionary<string, string> properties,
        Dictionary<string, KmlStyle> styles,
        HashSet<KmlPlacemark> placemarks,
        Dictionary<string, string> styleMaps,
        IEnumerable<KmlContainer> containers,
        HashSet<KmlGroundOverlay> groundOverlays,
        string? id)
    {
        _properties = properties ?? [];
        Styles = styles ?? [];
        Placemarks = placemarks ?? [];
        StyleMap = styleMaps ?? [];
        Containers = containers?.ToList() ?? [];
        GroundOverlays = groundOverlays ?? [];
        ContainerId = id;
    }

    /// <summary>
    /// Gets the map of KML styles, with key values representing style name (e.g., color) and value representing style value (e.g., #FFFFFF).
    /// </summary>
    public Dictionary<string, KmlStyle> Styles { get; }

    /// <summary>
    /// Gets the style map, which is a map of strings representing a style map.
    /// </summary>
    public Dictionary<string, string> StyleMap { get; }

    /// <summary>
    /// Gets all of the ground overlays which were set in the container.
    /// </summary>
    public HashSet<KmlGroundOverlay> GroundOverlays { get; }

    /// <summary>
    /// Gets the container ID if it is specified.
    /// </summary>
    public string? ContainerId { get; }

    public bool IsVisible { get; set; }

    /// <summary>
    /// Gets a style based on an ID.
    /// </summary>
    /// <param name="styleId">The ID of the style.</param>
    /// <returns>The style corresponding to the ID, or null if not found.</returns>
    public KmlStyle? GetStyle(string styleId) => Styles.GetValueOrDefault(styleId);

    /// <summary>
    /// Gets a style ID from the style map based on an ID.
    /// </summary>
    /// <param name="styleId">The ID of the style.</param>
    /// <returns>The style ID from the style map, or null if not found.</returns>
    public string? GetStyleIdFromMap(string styleId) => StyleMap.GetValueOrDefault(styleId);

    /// <summary>
    /// Gets the placemarks in the container.
    /// </summary>
    public HashSet<KmlPlacemark> Placemarks { get; }

    /// <summary>
    /// Gets the value of a property based on the given key.
    /// </summary>
    /// <param name="propertyName">The property key to find.</param>
    /// <returns>The value of the property, or null if the key doesn't exist.</returns>
    public string? GetProperty(string propertyName) => _properties.GetValueOrDefault(propertyName);

    /// <summary>
    /// Gets whether the given key exists in the properties.
    /// </summary>
    /// <param name="keyValue">The property key to find.</param>
    /// <returns>True if the key was found, otherwise false.</returns>
    public bool HasProperty(string keyValue) => _properties.ContainsKey(keyValue);

    /// <summary>
    /// Gets an iterable of nested KmlContainers.
    /// </summary>
    public IEnumerable<KmlContainer> Containers { get; }

#if DEBUG
    /// <summary>
    /// Returns a string representation of the container.
    /// </summary>
    public override string ToString()
    {
        var sb = new System.Text.StringBuilder(Constants.Container).Append('{').AppendLine();
        sb.Append("  properties=").AppendJoin(", ", _properties).Append(',').AppendLine();
        sb.Append("  placemarks=").AppendJoin(", ", Placemarks).Append(',').AppendLine();
        sb.Append("  containers=").AppendJoin(", ", Containers).Append(',').AppendLine();
        sb.Append("  ground overlays=").AppendJoin(", ", GroundOverlays).Append(',').AppendLine();
        sb.Append("  style maps=").AppendJoin(", ", StyleMap).Append(',').AppendLine();
        sb.Append("  styles=").AppendJoin(", ", Styles).AppendLine();
        sb.Append('}').AppendLine();
        return sb.ToString();
    }
#endif
}