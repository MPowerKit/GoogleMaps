using System.Text;

namespace MPowerKit.GoogleMaps.Data;

/// <summary>
/// Represents a GeoJSON feature with style information
/// </summary>
public class GeoJsonFeature : Feature
{
    public LatLngBounds? BoundingBox { get; }

    public GeoJsonFeature(IGeometry geometry, string? styleId,
        Dictionary<string, string> properties, LatLngBounds? boundingBox)
        : base(geometry, styleId, properties)
    {
        BoundingBox = boundingBox;
    }

    public GeoJsonPointStyle PointStyle { get; } = new();

    public GeoJsonPolylineStyle LineStringStyle { get; } = new();

    public GeoJsonPolygonStyle PolygonStyle { get; } = new();

    public void ParseStyles()
    {
        foreach (var kvp in Properties)
        {
            switch (kvp.Key)
            {
                case "title":
                    PointStyle.Title = kvp.Value;
                    break;
                case "snippet":
                    PointStyle.Snippet = kvp.Value;
                    break;
                case "icon":
                    PointStyle.Icon = kvp.Value;
                    break;
                case "marker-color":
                    PointStyle.IconColor = kvp.Value;
                    break;
                case "marker-rotation":
                    PointStyle.Rotation = double.TryParse(kvp.Value, out var rotation) ? rotation : null;
                    break;
                case "stroke":
                    LineStringStyle.Stroke = kvp.Value;
                    PolygonStyle.Stroke = kvp.Value;
                    break;
                case "stroke-opacity":
                    double? strokeOpacity = double.TryParse(kvp.Value, out var parsedStrokeOpacity) ? parsedStrokeOpacity : null;
                    LineStringStyle.StrokeOpacity = strokeOpacity;
                    PolygonStyle.StrokeOpacity = strokeOpacity;
                    break;
                case "stroke-width":
                    double? width = double.TryParse(kvp.Value, out var parsedWidth) ? parsedWidth : null;
                    LineStringStyle.StrokeWidth = width;
                    PolygonStyle.StrokeWidth = width;
                    break;
                case "fill":
                    PolygonStyle.Fill = kvp.Value;
                    break;
                case "fill-opacity":
                    double? fillOpacity = double.TryParse(kvp.Value, out var parsedFillOpacity) ? parsedFillOpacity : null;
                    PolygonStyle.FillOpacity = fillOpacity;
                    break;
            }
        }
    }

#if DEBUG
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine("Feature {");
        sb.AppendLine($"  BoundingBox: {BoundingBox}");
        sb.AppendLine($"  Geometry: {Geometry}");
        sb.AppendLine($"  PointStyle: {PointStyle}");
        sb.AppendLine($"  LineStringStyle: {LineStringStyle}");
        sb.AppendLine($"  PolygonStyle: {PolygonStyle}");
        sb.AppendLine($"  Id: {StyleUrl}");
        sb.AppendLine($"  Properties: {string.Join(", ", Properties ?? [])}");
        sb.AppendLine("}");
        return sb.ToString();
    }
#endif
}