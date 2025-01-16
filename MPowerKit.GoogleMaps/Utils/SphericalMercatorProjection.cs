namespace MPowerKit.GoogleMaps;

public class SphericalMercatorProjection
{
    private readonly double _worldWidth;

    public SphericalMercatorProjection(double worldWidth)
    {
        _worldWidth = worldWidth;
    }

    /// <summary>
    /// Converts a LatLng to a Point using a spherical Mercator projection.
    /// </summary>
    /// <param name="latLng">The geographic coordinate (latitude and longitude).</param>
    /// <returns>The projected point in (x, y) coordinates.</returns>
    public Point ToPoint(Point latLng)
    {
        var x = latLng.Y / 360d + 0.5;
        var siny = Math.Sin(Distance.DegreesToRadians(latLng.X));
        var y = 0.5 * Math.Log((1d + siny) / (1d - siny)) / -(2d * Math.PI) + 0.5;

        return new(x * _worldWidth, y * _worldWidth);
    }

    /// <summary>
    /// Converts a Point to a LatLng using a spherical Mercator projection.
    /// </summary>
    /// <param name="point">The point in (x, y) coordinates.</param>
    /// <returns>The geographic coordinate (latitude and longitude).</returns>
    public Point ToLatLng(Point point)
    {
        var x = point.X / _worldWidth - 0.5;
        var lng = x * 360d;

        var y = 0.5 - (point.Y / _worldWidth);
        var lat = 90d - Distance.RadiansToDegrees(Math.Atan(Math.Exp(-y * 2d * Math.PI)) * 2d);

        return new(lat, lng);
    }
}