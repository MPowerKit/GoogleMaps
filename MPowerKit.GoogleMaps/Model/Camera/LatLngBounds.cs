namespace MPowerKit.GoogleMaps;

public record struct LatLngBounds(Point SouthWest, Point NorthEast)
{
    public readonly bool Contains(Point point)
    {
        // Check if the point's latitude is between the bounds' latitudes
        bool isLatInBounds = point.X >= SouthWest.X && point.X <= NorthEast.X;

        // Check if the point's longitude is between the bounds' longitudes
        bool isLngInBounds = point.Y >= SouthWest.Y && point.Y <= NorthEast.Y;

        // Return true if both latitude and longitude are within bounds
        return isLatInBounds && isLngInBounds;
    }

    public override string ToString()
    {
        return $"SouthWest={{Lat={SouthWest.X:F2}, Lon={SouthWest.Y:F2}}}, NorthEast={{Lat={NorthEast.X:F2}, Lon={NorthEast.Y:F2}}}";
    }
}