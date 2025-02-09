namespace MPowerKit.GoogleMaps.Data;

public record GeoJsonPolylineStyle : PolylineStyle
{
    public double? StrokeOpacity { get; set; }
}