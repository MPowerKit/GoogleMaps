namespace MPowerKit.GoogleMaps.Data;

/// <summary>
/// An abstraction that shares the common properties of KmlPlacemark and GeoJsonFeature.
/// </summary>
public class Feature : IHasProperty
{
    /// <summary>
    /// Gets or sets the ID of the feature.
    /// </summary>
    public string? StyleUrl { get; }

    /// <summary>
    /// Gets the geometry object associated with the feature.
    /// </summary>
    public IGeometry Geometry { get; }

    /// <summary>
    /// Gets the property entry set.
    /// </summary>
    /// <returns>An enumerable of key-value pairs representing the properties.</returns>
    protected Dictionary<string, string> Properties;

    /// <summary>
    /// Creates a new Feature object.
    /// </summary>
    /// <param name="geometry">The geometry to assign to the feature.</param>
    /// <param name="styleUrl">The common identifier of the feature.</param>
    /// <param name="properties">A dictionary containing properties related to the feature.</param>
    public Feature(IGeometry geometry, string? styleUrl, Dictionary<string, string> properties)
    {
        Geometry = geometry ?? throw new ArgumentNullException(nameof(geometry));
        StyleUrl = styleUrl;
        Properties = properties ?? [];
    }

    /// <summary>
    /// Gets the value for a stored property.
    /// </summary>
    /// <param name="property">The key of the property.</param>
    /// <returns>The value of the property if its key exists; otherwise, null.</returns>
    public string? GetProperty(string property) => Properties.GetValueOrDefault(property);

    /// <summary>
    /// Checks whether the given property key exists.
    /// </summary>
    /// <param name="property">The key of the property to check.</param>
    /// <returns>True if the property key exists; otherwise, false.</returns>
    public bool HasProperty(string property) => Properties.ContainsKey(property);

    public bool IsVisible { get; set; }

    /// <summary>
    /// Stores a new property key and value.
    /// </summary>
    /// <param name="property">The key of the property to store.</param>
    /// <param name="propertyValue">The value of the property to store.</param>
    /// <returns>The previous value with the same key; otherwise, null if the key didn't exist.</returns>
    protected string? SetProperty(string property, string propertyValue)
    {
        ArgumentNullException.ThrowIfNull(property, nameof(property));

        Properties.TryGetValue(property, out var previousValue);
        Properties[property] = propertyValue;
        return previousValue;
    }

    /// <summary>
    /// Removes a given property.
    /// </summary>
    /// <param name="property">The key of the property to remove.</param>
    /// <returns>The value of the removed property or null if there was no corresponding key.</returns>
    protected string? RemoveProperty(string property)
    {
        ArgumentNullException.ThrowIfNull(property, nameof(property));

        Properties.TryGetValue(property, out var value);
        Properties.Remove(property);
        return value;
    }
}