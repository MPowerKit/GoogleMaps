namespace MPowerKit.GoogleMaps.Data;

public record class MarkerStyle
{
    public double? Rotation { get; set; }
    public Point? Anchor { get; set; }
    public double? Scale { get; set; }
    public string? Icon { get; set; }
    public string? IconColor { get; set; }
    public string? InfoWindowText { get; set; }
}