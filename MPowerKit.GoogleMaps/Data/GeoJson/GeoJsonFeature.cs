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
                    double? strkOpct = double.TryParse(kvp.Value, out var strokeOpacity) ? strokeOpacity : null;
                    LineStringStyle.StrokeOpacity = strkOpct;
                    PolygonStyle.StrokeOpacity = strkOpct;
                    break;
                case "stroke-width":
                    double? wdth = double.TryParse(kvp.Value, out var width) ? width : null;
                    LineStringStyle.StrokeWidth = wdth;
                    PolygonStyle.StrokeWidth = wdth;
                    break;
                case "fill":
                    PolygonStyle.Fill = kvp.Value;
                    break;
                case "fill-opacity":
                    double? fllOpct = double.TryParse(kvp.Value, out var fillOpacity) ? fillOpacity : null;
                    PolygonStyle.FillOpacity = fllOpct;
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