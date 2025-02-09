namespace MPowerKit.GoogleMaps.Data;

/// <summary>
/// Represents a placemark which is either a KmlPoint, KmlLineString, KmlPolygon, or KmlMultiGeometry.
/// Stores the properties and styles of the place.
/// </summary>
public class KmlPlacemark : Feature
{
    /// <summary>
    /// Creates a new KmlPlacemark object.
    /// </summary>
    /// <param name="geometry">The geometry object to store.</param>
    /// <param name="styleUrl">The style ID to store.</param>
    /// <param name="inlineStyle">The inline style to store.</param>
    /// <param name="properties">The properties dictionary to store.</param>
    public KmlPlacemark(IGeometry geometry, string? styleUrl, KmlStyle? inlineStyle, Dictionary<string, string> properties)
        : base(geometry, styleUrl, properties)
    {
        ArgumentNullException.ThrowIfNull(geometry);

        InlineStyle = inlineStyle;
    }

    /// <summary>
    /// Gets the inline style that was found.
    /// </summary>
    public KmlStyle? InlineStyle { get; }

    public KmlStyle? Style { get; set; }

    public int ZIndex { get; set; }

    public string? Name { get; set; }
    public string? Description { get; set; }

#if DEBUG
    /// <summary>
    /// Returns a string representation of the KmlPlacemark.
    /// </summary>
    public override string ToString()
    {
        var sb = new System.Text.StringBuilder(Constants.Placemark).Append('{').AppendLine();
        sb.Append("  style id=(").Append(StyleUrl).Append("),").AppendLine();
        sb.Append("  inline style=(").Append(InlineStyle).Append(')').AppendLine();
        sb.Append('}').AppendLine();
        return sb.ToString();
    }
#endif
}