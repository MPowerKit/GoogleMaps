using System.Globalization;

using MPowerKit.GoogleMaps;

namespace Sample.Converters;

public class LocationToViewImageSourceConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Sample.Models.Location location
            && parameter is View view)
        {
            view.BindingContext = $"{location.Latitude:F2},{location.Longitude:F2}";
            return (ViewImageSource)view;
        }

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}