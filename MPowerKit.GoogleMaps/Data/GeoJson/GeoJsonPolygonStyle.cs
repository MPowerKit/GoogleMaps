namespace MPowerKit.GoogleMaps.Data;

public record GeoJsonPolygonStyle : GeoJsonPolylineStyle
{
    public string? Fill { get; set; }
    public double? FillOpacity { get; set; }
}