using System.Text;

namespace MPowerKit.GoogleMaps.Data;

/// <summary>
/// Represents a KML Ground Overlay.
/// </summary>
public class KmlGroundOverlay : IHasProperty
{
    private readonly Dictionary<string, string> _properties;

    /// <summary>
    /// Creates a new KmlGroundOverlay.
    /// </summary>
    /// <param name="imageUrl">URL of the ground overlay image.</param>
    /// <param name="latLngBox">Bounds of the image.</param>
    /// <param name="drawOrder">Z-index of the image.</param>
    /// <param name="visibility">True if visible, false otherwise.</param>
    /// <param name="properties">Properties dictionary.</param>
    /// <param name="rotation">Rotation of the image.</param>
    /// <exception cref="ArgumentNullException">Thrown if latLngBox is null.</exception>
    public KmlGroundOverlay(string imageUrl, LatLngBounds latLngBox, float drawOrder, int visibility, Dictionary<string, string> properties, float rotation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(imageUrl);

        ImageUrl = imageUrl;
        _properties = properties ?? [];
        LatLngBox = latLngBox;
        IsVisible = visibility != 0;

        GroundOverlay = new GroundOverlay
        {
            Bounds = latLngBox,
            Rotation = rotation,
            ZIndex = (int)drawOrder,
            IsVisible = IsVisible,
            Image = imageUrl
        };
    }

    public bool IsVisible { get; set; }

    /// <summary>
    /// Gets the image URL.
    /// </summary>
    public string ImageUrl { get; }

    /// <summary>
    /// Gets the boundaries of the ground overlay.
    /// </summary>
    public LatLngBounds LatLngBox { get; }

    /// <summary>
    /// Gets a property value.
    /// </summary>
    /// <param name="key">Key of the property.</param>
    /// <returns>Value of the property.</returns>
    public string? GetProperty(string key)
    {
        return _properties.TryGetValue(key, out var value) ? value : null;
    }

    /// <summary>
    /// Checks if the ground overlay has a property.
    /// </summary>
    /// <param name="key">Key of the property.</param>
    /// <returns>True if the property exists, false otherwise.</returns>
    public bool HasProperty(string key)
    {
        return _properties.ContainsKey(key);
    }

    /// <summary>
    /// Gets the GroundOverlayOptions for the ground overlay.
    /// </summary>
    public GroundOverlay GroundOverlay { get; }

#if DEBUG
    /// <summary>
    /// Returns a string representation of the KmlGroundOverlay.
    /// </summary>
    public override string ToString()
    {
        var sb = new StringBuilder(Constants.GroundOverlay).Append('{').AppendLine();
        sb.Append("  properties=").AppendJoin(", ", _properties).Append(',').AppendLine();
        sb.Append("  image url=").Append(ImageUrl).Append(',').AppendLine();
        sb.Append("  LatLngBox=").Append(LatLngBox).AppendLine();
        sb.Append('}').AppendLine();
        return sb.ToString();
    }
#endif
}