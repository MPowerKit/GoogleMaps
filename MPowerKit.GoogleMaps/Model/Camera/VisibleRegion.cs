namespace MPowerKit.GoogleMaps;

public record struct VisibleRegion(LatLngBounds Bounds, Point FarLeft, Point FarRight, Point NearLeft, Point NearRight)
{
    public override string ToString()
    {
        return $"Bounds={{{Bounds}}}, NearLeft={{Lat={NearLeft.X:F2}, Lon={NearLeft.Y:F2}}}, NearRight={{Lat={NearRight.X:F2}, Lon={NearRight.Y:F2}}}, FarLeft={{Lat={FarLeft.X:F2}, Lon={FarLeft.Y:F2}}}, FarRight={{Lat={FarRight.X:F2}, Lon={FarRight.Y:F2}}}";
    }
}