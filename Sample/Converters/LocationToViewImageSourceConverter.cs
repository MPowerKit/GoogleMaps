using System.Globalization;

using MPowerKit.GoogleMaps;

namespace Sample.Converters;

public class LocationToViewImageSourceConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Sample.Models.Location location)
        {
            var stack = new VerticalStackLayout();
            stack.Children.Add(new Label()
            {
                Text = "Drag me",
                TextColor = Colors.White,
                FontAttributes = FontAttributes.Bold
            });
            stack.Children.Add(new Label()
            {
                Text = $"{location.Latitude:F2},{location.Longitude:F2}",
                TextColor = Colors.White,
                FontAttributes = FontAttributes.Bold
            });

            return (ViewImageSource)new ContentView()
            {
                Padding = 15,
                BackgroundColor = Colors.Red,
                Content = stack
            };
        }

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}