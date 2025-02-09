using System.Text;

namespace MPowerKit.GoogleMaps.Data;

/// <summary>
/// Represents the defined styles in the KML document.
/// </summary>
public class KmlStyle : Style
{

    /// <summary>
    /// Gets a MarkerOptions object.
    /// </summary>
    public MarkerStyle GetMarkerStyle() => CreateMarkerStyle(MarkerStyle, IsIconRandomColorMode);

    /// <summary>
    /// Gets a PolylineOptions object.
    /// </summary>
    public PolylineStyle GetPolylineStyle() => CreatePolylineStyle(PolylineStyle, IsLineRandomColorMode);

    /// <summary>
    /// Gets a PolygonOptions object.
    /// </summary>
    public PolygonStyle GetPolygonStyle() => CreatePolygonStyle(PolygonStyle, Fill, HasOutline, IsPolyRandomColorMode);

    /// <summary>
    /// Creates a new KmlStyle object.
    /// </summary>
    public KmlStyle()
    {
        IsIconRandomColorMode = false;
        IsLineRandomColorMode = false;
        IsPolyRandomColorMode = false;
    }

    /// <summary>
    /// Sets text found for an info window.
    /// </summary>
    public string? InfoWindowText
    {
        get => MarkerStyle.InfoWindowText;
        set => MarkerStyle.InfoWindowText = value;
    }

    /// <summary>
    /// Gets or sets the ID for the style.
    /// </summary>
    public string? StyleId { get; set; }

    /// <summary>
    /// Gets or sets whether the Polygon fill is set.
    /// </summary>
    public bool Fill { get; set; } = true;

    /// <summary>
    /// Gets or sets the scale for a marker icon.
    /// </summary>
    public double? IconScale
    {
        get => MarkerStyle.Scale;
        set => MarkerStyle.Scale = value;
    }

    /// <summary>
    /// Gets whether the Polygon outline is set.
    /// </summary>
    public bool HasOutline { get; set; } = true;

    /// <summary>
    /// Gets the URL for the marker icon.
    /// </summary>
    public string? IconUrl
    {
        get => MarkerStyle.Icon;
        set => MarkerStyle.Icon = value;
    }

    /// <summary>
    /// Gets or sets the fill color for a KML Polygon using a string.
    /// </summary>
    public string? FillColor
    {
        get => PolygonFillColor;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);

            PolygonFillColor = $"#{ColorExtensions.AbgrToArgbColor(value)}";
        }
    }

    /// <summary>
    /// Sets the color for a marker.
    /// </summary>
    public string? MarkerColor
    {
        get => MarkerStyle.IconColor;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);

            var color = $"#{ColorExtensions.AbgrToArgbColor(value)}";
            MarkerStyle.IconColor = color;
        }
    }

    /// <summary>
    /// Sets the rotation / heading for a marker.
    /// </summary>
    public double? Heading
    {
        get => MarkerRotation;
        set => MarkerRotation = value;
    }

    /// <summary>
    /// Sets the hotspot / anchor point of a marker.
    /// </summary>
    /// <param name="x">X point of a marker position.</param>
    /// <param name="y">Y point of a marker position.</param>
    /// <param name="xUnits">Units in which the x value is specified.</param>
    /// <param name="yUnits">Units in which the y value is specified.</param>
    public void SetHotSpot(float x, float y, string xUnits, string yUnits)
    {
        SetMarkerHotSpot(x, y, xUnits, yUnits);
    }

    /// <summary>
    /// Sets the color mode for a marker.
    /// </summary>
    /// <param name="colorMode">A "random" or "normal" color mode.</param>
    public void SetIconColorMode(string colorMode)
    {
        IsIconRandomColorMode = colorMode.Equals(Constants.Random, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks whether the color mode for a marker is random.
    /// </summary>
    public bool IsIconRandomColorMode { get; private set; }

    /// <summary>
    /// Sets the color mode for a polyline.
    /// </summary>
    /// <param name="colorMode">A "random" or "normal" color mode.</param>
    public void SetLineColorMode(string colorMode)
    {
        IsLineRandomColorMode = colorMode.Equals(Constants.Random, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks whether the color mode for a polyline is random.
    /// </summary>
    public bool IsLineRandomColorMode { get; private set; }

    /// <summary>
    /// Sets the color mode for a polygon.
    /// </summary>
    /// <param name="colorMode">A "random" or "normal" color mode.</param>
    public void SetPolyColorMode(string colorMode)
    {
        IsPolyRandomColorMode = colorMode.Equals(Constants.Random, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks whether the color mode for a polygon is random.
    /// </summary>
    public bool IsPolyRandomColorMode { get; private set; }

    /// <summary>
    /// Sets the outline color for a Polyline and a Polygon using a string.
    /// </summary>
    public string? OutlineColor
    {
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);

            var newColor = $"#{ColorExtensions.AbgrToArgbColor(value)}";
            PolylineStyle.Stroke = newColor;
            PolygonStyle.Stroke = newColor;
        }
    }

    /// <summary>
    /// Sets the line width for a Polyline and a Polygon.
    /// </summary>
    public double Width
    {
        set
        {
            LineStringWidth = value;
            PolygonStrokeWidth = value;
        }
    }

    /// <summary>
    /// Creates a new MarkerStyle from given properties of an existing MarkerStyle.
    /// </summary>
    private static MarkerStyle CreateMarkerStyle(MarkerStyle originalMarkerOption, bool iconRandomColorMode)
    {
        var newMarkerOption = new MarkerStyle
        {
            Rotation = originalMarkerOption.Rotation,
            Anchor = originalMarkerOption.Anchor,
            Scale = originalMarkerOption.Scale,
            InfoWindowText = originalMarkerOption.InfoWindowText,
            Icon = originalMarkerOption.Icon,
            IconColor = originalMarkerOption.IconColor
        };

        if (iconRandomColorMode && originalMarkerOption.IconColor is not null)
        {
            newMarkerOption.IconColor = ColorExtensions.ComputeRandomColor(originalMarkerOption.IconColor);
        }

        return newMarkerOption;
    }

    /// <summary>
    /// Creates a new PolylineStyle from given properties of an existing PolylineStyle.
    /// </summary>
    private static PolylineStyle CreatePolylineStyle(PolylineStyle originalPolylineOption, bool lineRandomColorMode)
    {
        var option = new PolylineStyle()
        {
            Stroke = originalPolylineOption.Stroke,
            StrokeWidth = originalPolylineOption.StrokeWidth
        };

        if (lineRandomColorMode && originalPolylineOption.Stroke is not null)
        {
            option.Stroke = ColorExtensions.ComputeRandomColor(originalPolylineOption.Stroke);
        }

        return option;
    }

    /// <summary>
    /// Creates a new PolygonStyle from given properties of an existing PolygonStyle.
    /// </summary>
    private static PolygonStyle CreatePolygonStyle(PolygonStyle originalPolygonOption, bool isFill, bool isOutline, bool polyRandomColorMode)
    {
        var option = new PolygonStyle()
        {
            Fill = isFill ? originalPolygonOption.Fill : "#00FFFFFF",
            Stroke = isOutline ? originalPolygonOption.Stroke : "#00FFFFFF",
            StrokeWidth = isOutline ? originalPolygonOption.StrokeWidth : 0d,
        };

        if (polyRandomColorMode && originalPolygonOption.Fill is not null && isFill)
        {
            option.Fill = ColorExtensions.ComputeRandomColor(originalPolygonOption.Fill);
        }

        return option;
    }

#if DEBUG
    /// <summary>
    /// Returns a string representation of the KmlStyle.
    /// </summary>
    public override string ToString()
    {
        var sb = new StringBuilder(Constants.Style).Append('{').AppendLine();
        sb.Append("  balloon options=").AppendJoin(", ", InfoWindowText).Append(',').AppendLine();
        sb.Append("  fill=").Append(Fill).Append(',').AppendLine();
        sb.Append("  outline=").Append(HasOutline).Append(',').AppendLine();
        sb.Append("  icon url=").Append(MarkerStyle.Icon).Append(',').AppendLine();
        sb.Append("  scale=").Append(MarkerStyle.Scale).Append(',').AppendLine();
        sb.Append("  style id=").Append(StyleId).AppendLine();
        sb.Append('}').AppendLine();
        return sb.ToString();
    }
#endif
}