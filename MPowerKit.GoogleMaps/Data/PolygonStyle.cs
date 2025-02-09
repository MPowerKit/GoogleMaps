namespace MPowerKit.GoogleMaps.Data;

public record class PolygonStyle : PolylineStyle
{
    public string? Fill { get; set; }
}