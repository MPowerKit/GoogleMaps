using CoreLocation;

using Google.Maps;

namespace MPowerKit.GoogleMaps;

public static class Extensions
{
    public static CLLocationCoordinate2D ToCoord(this Point point)
    {
        return new CLLocationCoordinate2D(point.X, point.Y);
    }

    public static MutablePath ToPath(this IEnumerable<Point> points)
    {
        MutablePath path = new();

        foreach (var point in points)
        {
            path.AddLatLon(point.X, point.Y);
        }

        return path;
    }
}