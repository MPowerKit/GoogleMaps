using System.Text.RegularExpressions;

namespace MPowerKit.GoogleMaps.Data;

public partial class KmlUtil
{
    public static bool ParseBoolean(string text)
    {
        return "1".Equals(text) || "true".Equals(text, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Substitute property values in BalloonStyle text template.
    /// </summary>
    /// <param name="template">The text template.</param>
    /// <param name="placemark">The placemark to get property values from.</param>
    /// <returns>A string with property values substituted.</returns>
    public static string SubstituteProperties(string template, KmlPlacemark placemark)
    {
        return SubstituteRegex().Replace(template, match =>
        {
            var property = match.Groups[1].Value;
            var value = placemark.GetProperty(property);
            return value ?? "";
        });
    }

    [GeneratedRegex(@"\$\[(.+?)\]")]
    public static partial Regex SubstituteRegex();
}