using System.Globalization;

namespace Sample.Converters;

public class LocationToPointConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Sample.Models.Location location)
        {
            return new Point(location.Latitude, location.Longitude);
        }

        return value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Point point)
        {
            return new Sample.Models.Location() { Latitude = point.X, Longitude = point.Y };
        }

        return value;
    }
}