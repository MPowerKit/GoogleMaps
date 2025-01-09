using System.Globalization;

namespace Sample.Converters;

public class TileDataToStreamConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return new Func<CancellationToken, Task<Stream>>(ct => FileSystem.Current.OpenAppPackageFileAsync("bot.png"));
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}