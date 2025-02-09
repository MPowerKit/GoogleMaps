namespace MPowerKit.GoogleMaps.Data;

public record GeoJsonPointStyle : MarkerStyle
{
    public string? Title { get; set; }
    public string? Snippet { get; set; }
}