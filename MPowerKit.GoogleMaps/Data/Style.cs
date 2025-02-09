namespace MPowerKit.GoogleMaps.Data;

/// <summary>
/// An abstraction that shares the common properties of styles for points, lines, and polygons.
/// </summary>
public abstract class Style
{
    protected MarkerStyle MarkerStyle { get; } = new();
    protected PolylineStyle PolylineStyle { get; } = new();
    protected PolygonStyle PolygonStyle { get; } = new();

    /// <summary>
    /// Gets or sets the rotation of a marker in degrees clockwise about the marker's anchor.
    /// </summary>
    protected double? MarkerRotation
    {
        get => MarkerStyle.Rotation;
        set => MarkerStyle.Rotation = value;
    }

    /// <summary>
    /// Gets or sets the stroke thickness of the LineString in screen pixels.
    /// </summary>
    protected double? LineStringWidth
    {
        get => PolylineStyle.StrokeWidth;
        set => PolylineStyle.StrokeWidth = value;
    }

    /// <summary>
    /// Gets or sets the stroke thickness of the Polygon in screen pixels.
    /// </summary>
    protected double? PolygonStrokeWidth
    {
        get => PolygonStyle.StrokeWidth;
        set => PolygonStyle.StrokeWidth = value;
    }

    /// <summary>
    /// Gets or sets the fill color of the Polygon as a 32-bit ARGB color.
    /// </summary>
    protected string? PolygonFillColor
    {
        get => PolygonStyle.Fill;
        set => PolygonStyle.Fill = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Style"/> class.
    /// </summary>
    protected Style()
    {
    }

    /// <summary>
    /// Sets the hotspot / anchor point of a marker.
    /// </summary>
    /// <param name="x">The x point of the marker position.</param>
    /// <param name="y">The y point of the marker position.</param>
    /// <param name="xUnits">The units in which the x value is specified.</param>
    /// <param name="yUnits">The units in which the y value is specified.</param>
    protected void SetMarkerHotSpot(double x, double y, string xUnits, string yUnits)
    {
        var xAnchor = 0.5;
        var yAnchor = 1d;

        // Set x coordinate
        if (xUnits.Equals(Constants.Fraction, StringComparison.OrdinalIgnoreCase))
        {
            xAnchor = x;
        }
        else
        {
            Console.WriteLine("Hotspot xUnits other than \"fraction\" are not supported.");
        }

        // Set y coordinate
        if (yUnits.Equals(Constants.Fraction, StringComparison.OrdinalIgnoreCase))
        {
            yAnchor = y;
        }
        else
        {
            Console.WriteLine("Hotspot yUnits other than \"fraction\" are not supported.");
        }

        MarkerStyle.Anchor = new(xAnchor, yAnchor);
    }
}