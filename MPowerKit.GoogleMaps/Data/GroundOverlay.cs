namespace MPowerKit.GoogleMaps.Data;

public record class GroundOverlay
{
    public required string Image { get; set; }
    public LatLngBounds Bounds { get; set; }
    public double Rotation { get; set; }
    public int ZIndex { get; set; }
    public bool IsVisible { get; set; } = true;
}