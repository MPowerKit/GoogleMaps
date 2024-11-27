namespace MPowerKit.GoogleMaps;

public static class Distance
{
    public const double DegreesToRadiansConst = Math.PI / 180.0;
    public const double RadiansToDegreesConst = 180.0 / Math.PI;
    public const double MetersPerMile = 1_609.344;
    public const double MetersPerKiloMeter = 1_000d;
    public const double EarthRadiusMeters = 6_371_000d;
    public const double EarthCircumferenceMeters = EarthRadiusMeters * 2.0 * Math.PI;

    public static double FromMiles(double miles) => miles * MetersPerMile;
    public static double FromMeters(double meters) => meters;
    public static double FromKMeters(double kiloMeters) => kiloMeters * MetersPerKiloMeter;
    public static double BetweenPoints(Point a, Point b) => BetweenPoints(a.X, a.Y, b.X, b.Y);
    public static double BetweenPoints(double lat1, double lon1, double lat2, double lon2)
    {
        if (lat1 == lat2 && lon1 == lon2) return 0d;

        var dLat = DegreesToRadians(lat2 - lat1);
        var dLon = DegreesToRadians(lon2 - lon1);

        lat1 = DegreesToRadians(lat1);
        lat2 = DegreesToRadians(lat2);

        var sinLat = Math.Sin(dLat / 2d);
        var sinLon = Math.Sin(dLon / 2d);

        var a = sinLat * sinLat + sinLon * sinLon * Math.Cos(lat1) * Math.Cos(lat2);
        var c = 2d * Math.Asin(Math.Sqrt(a));

        return EarthRadiusMeters * c;
    }

    public static double ToLattitudeDegrees(double meters) => RadiansToDegrees(meters) / EarthRadiusMeters;
    public static double ToLongitudeDegrees(double meters, double latitude) => RadiansToDegrees(meters) / (EarthRadiusMeters * Math.Cos(DegreesToRadians(latitude)));

    public static double DegreesToRadians(double degrees)
    {
        return degrees * DegreesToRadiansConst;
    }

    public static double RadiansToDegrees(double radians)
    {
        return radians * RadiansToDegreesConst;
    }
}