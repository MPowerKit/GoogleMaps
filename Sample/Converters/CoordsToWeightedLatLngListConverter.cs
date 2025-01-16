using System.Globalization;

using MPowerKit.GoogleMaps;

namespace Sample.Converters;

public class CoordsToWeightedLatLngListConverter : IValueConverter
{

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is IEnumerable<Point> coords)
        {
            return coords.Select(c => new WeightedLatLng(c));
        }

        return value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}