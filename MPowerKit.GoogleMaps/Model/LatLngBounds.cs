namespace MPowerKit.GoogleMaps;

public record struct LatLngBounds(Point SouthWest, Point NorthEast)
{
    public override string ToString()
    {
        return $"SouthWest={{Lat={SouthWest.X:F2}, Lon={SouthWest.Y:F2}}}, NorthEast={{Lat={NorthEast.X:F2}, Lon={NorthEast.Y:F2}}}";
    }
}