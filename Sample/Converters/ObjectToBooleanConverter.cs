using System.Globalization;

namespace Sample.Converters;

public class ObjectToBooleanConverter : IValueConverter
{
    public bool IsInverted { get; set; }

    public object Convert(object? value, Type? targetType, object? parameter, CultureInfo culture)
    {
        bool result;
        if (value is string strValue)
        {
            result = !string.IsNullOrWhiteSpace(strValue);
        }
        else
        {
            result = value is not null;
        }
        return IsInverted ? !result : result;
    }

    public object ConvertBack(object? value, Type? targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}