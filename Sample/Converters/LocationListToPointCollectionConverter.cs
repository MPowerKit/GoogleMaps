using System.Globalization;

namespace Sample.Converters;

public class LocationListToPointCollectionConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is IEnumerable<Sample.Models.Location> list)
        {
            PointCollection collection = [];
            foreach (var item in list)
            {
                collection.Add(new Point(item.Latitude, item.Longitude));
            }
            return collection;
        }

        return new PointCollection();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}