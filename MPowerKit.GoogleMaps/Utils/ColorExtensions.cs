namespace MPowerKit.GoogleMaps;

public static class ColorExtensions
{
    public static int ColorWithNewAlpha(this int color, float alpha)
    {
        alpha = Math.Clamp(alpha, 0f, 1f);

        var newAlpha = (int)(alpha * 255);
        return (newAlpha << 24) | (color & 0x00FFFFFF);
    }

    public static int ColorWithMultipliedAlphas(this int color, float alpha)
    {
        alpha = Math.Clamp(alpha, 0f, 1f);

        var oldAlpha = color.GetColorAlpha() / 255f;
        var newAlpha = (int)(alpha * oldAlpha * 255);
        return (newAlpha << 24) | (color & 0x00FFFFFF);
    }

    public static int ColorFromArgb(float a, float r, float g, float b)
    {
        a = Math.Clamp(a, 0f, 1f);
        r = Math.Clamp(r, 0f, 1f);
        g = Math.Clamp(g, 0f, 1f);
        b = Math.Clamp(b, 0f, 1f);

        return ((int)(a * 255f) << 24) | ((int)(r * 255f) << 16) | ((int)(g * 255f) << 8) | (int)(b * 255f);
    }

    public static int GetColorAlpha(this int color)
    {
        return (color >> 24) & 0x000000FF;
    }

    public static int GetColorRed(this int color)
    {
        return (color >> 16) & 0x000000FF;
    }

    public static int GetColorGreen(this int color)
    {
        return (color >> 8) & 0x000000FF;
    }

    public static int GetColorBlue(this int color)
    {
        return color & 0x000000FF;
    }

    public static float InterpolateChannel(float channel1, float channel2, float ratio)
    {
        return channel1 + (channel2 - channel1) * ratio;
    }

    public static float InterpolateHue(float h1, float h2, float ratio)
    {
        // Handle circular interpolation for Hue
        if (Math.Abs(h2 - h1) > 0.5f)
        {
            if (h2 > h1)
                h1 += 1f;
            else
                h2 += 1f;
        }

        float h = InterpolateChannel(h1, h2, ratio);
        return h % 1f; // Wrap around if necessary
    }

    public static int HsvColorsInterpolation(int color1, int color2, float ratio)
    {
        var alpha1 = color1.GetColorAlpha() / 255f;
        var alpha2 = color2.GetColorAlpha() / 255f;

        var alpha = InterpolateChannel(alpha1, alpha2, ratio);

        var hsv1 = color1.ColorToHsv();
        var hsv2 = color2.ColorToHsv();

        // Interpolate HSV components
        var h = InterpolateHue(hsv1.h, hsv2.h, ratio); // Handle circular interpolation for Hue
        var s = InterpolateChannel(hsv1.s, hsv2.s, ratio);
        var v = InterpolateChannel(hsv1.v, hsv2.v, ratio);

        // Convert HSV back to RGB
        return HsvToRgb(h, s, v, alpha);
    }

    public static (float h, float s, float v) ColorToHsv(this int color)
    {
        var r = color.GetColorRed() / 255f;
        var g = color.GetColorGreen() / 255f;
        var b = color.GetColorBlue() / 255f;

        var max = Math.Max(r, Math.Max(g, b));
        var min = Math.Min(r, Math.Min(g, b));
        var delta = max - min;

        float h = 0f, s = 0f, v = max;

        if (delta != 0f)
        {
            s = delta / max;

            if (max == r)
                h = (g - b) / delta + (g < b ? 6f : 0f);
            else if (max == g)
                h = (b - r) / delta + 2f;
            else if (max == b)
                h = (r - g) / delta + 4f;

            h /= 6f;
        }

        return (h, s, v);
    }

    public static int HsvToRgb(float h, float s, float v, float alpha)
    {
        if (s == 0f)
        {
            // Grayscale
            return ColorFromArgb(v, v, v, alpha);
        }

        h *= 6f; // Scale hue to 0-6
        var sector = (int)h;
        var fraction = h - sector;
        var p = v * (1f - s);
        var q = v * (1f - s * fraction);
        var t = v * (1f - s * (1f - fraction));

        float r, g, b;
        switch (sector)
        {
            case 0:
                r = v; g = t; b = p;
                break;
            case 1:
                r = q; g = v; b = p;
                break;
            case 2:
                r = p; g = v; b = t;
                break;
            case 3:
                r = p; g = q; b = v;
                break;
            case 4:
                r = t; g = p; b = v;
                break;
            default:
                r = v; g = p; b = q;
                break;
        }

        return ColorFromArgb(alpha, r, g, b);
    }

    // Function to interpolate between two colors using HSL color space
    public static int RgbColorsInterpolation(int color1, int color2, float ratio)
    {
        ratio = Math.Clamp(ratio, 0f, 1f);

        var c1 = Color.FromInt(color1);
        var c2 = Color.FromInt(color2);

        var alpha = InterpolateChannel(c1.Alpha, c2.Alpha, ratio);

        // Interpolate each component in the RGB color space
        var r = InterpolateChannel(c1.Red, c2.Red, ratio);
        var g = InterpolateChannel(c1.Green, c2.Green, ratio);
        var b = InterpolateChannel(c1.Blue, c2.Blue, ratio);

        return Color.FromRgba(r, g, b, alpha).ToInt();
    }

    // Function to interpolate between two colors using HSL color space
    public static int HslColorsInterpolation(int color1, int color2, float ratio)
    {
        ratio = Math.Clamp(ratio, 0f, 1f);

        Color.FromInt(color1).ToHsl(out var h1, out var s1, out var l1);
        Color.FromInt(color2).ToHsl(out var h2, out var s2, out var l2);

        var alpha1 = color1.GetColorAlpha() / 255f;
        var alpha2 = color2.GetColorAlpha() / 255f;

        var alpha = InterpolateChannel(alpha1, alpha2, ratio);

        // Interpolate each component in the HSL color space
        var h = InterpolateChannel(h1, h2, ratio);
        var s = InterpolateChannel(s1, s2, ratio);
        var l = InterpolateChannel(l1, l2, ratio);

        // Convert the interpolated HSL color back to RGB
        return Color.FromHsla(h, s, l, alpha).ToInt();
    }

    // Function to interpolate between two colors using Lab color space
    public static int LabColorsInterpolation(int color1, int color2, float ratio)
    {
        ratio = Math.Clamp(ratio, 0f, 1f);

        var lab1 = color1.ColorToLab();
        var lab2 = color2.ColorToLab();

        var alpha1 = color1.GetColorAlpha() / 255f;
        var alpha2 = color2.GetColorAlpha() / 255f;

        var alpha = InterpolateChannel(alpha1, alpha2, ratio);

        // Interpolate each component in the Lab color space
        var l = InterpolateChannel(lab1.l, lab2.l, ratio);
        var a = InterpolateChannel(lab1.a, lab2.a, ratio);
        var b = InterpolateChannel(lab1.b, lab2.b, ratio);

        // Convert the interpolated Lab color back to RGB
        return ColorFromLab(l, a, b, alpha);
    }

    private const float _epsilon = 0.008856f; // Constant used in the LAB conversion
    private const float _kappa = 903.3f;
    private const float _oneThird = 1f / 3f;
    private const float _16_116 = 16f / 116f;

    public static (float l, float a, float b) ColorToLab(this int color)
    {
        var (x, y, z) = color.ColorToToXyz();
        return XyzToLab(x, y, z);
    }

    public static int ColorFromLab(float l, float a, float b, float alpha)
    {
        var (x, y, z) = LabToXyz(l, a, b);
        return ColorFromXyz(x, y, z, alpha);
    }

    // Helper function to apply gamma correction
    public static float GammaCorrection(float channel)
    {
        return (channel > 0.04045f) ? (float)Math.Pow((channel + 0.055f) / 1.055f, 2.4f) : channel / 12.92f;
    }

    // Convert RGBA to XYZ
    public static (float x, float y, float z) ColorToToXyz(this int color)
    {
        var r = color.GetColorRed();
        var g = color.GetColorGreen();
        var b = color.GetColorBlue();

        var rLinear = GammaCorrection(r / 255f);
        var gLinear = GammaCorrection(g / 255f);
        var bLinear = GammaCorrection(b / 255f);

        // Convert to XYZ using the transformation matrix
        var X = rLinear * 0.4124f + gLinear * 0.3576f + bLinear * 0.1805f;
        var Y = rLinear * 0.2126f + gLinear * 0.7152f + bLinear * 0.0722f;
        var Z = rLinear * 0.0193f + gLinear * 0.1192f + bLinear * 0.9505f;

        return (X * 100f, Y * 100f, Z * 100f); // XYZ values are typically scaled by 100
    }

    // Normalize XYZ values based on reference white point (D65)
    private static (float x, float y, float z) NormalizeXyz(float x, float y, float z)
    {
        var refX = 95.047f;
        var refY = 100.0f;
        var refZ = 108.883f;

        return (x / refX, y / refY, z / refZ);
    }

    // XYZ to LAB conversion function
    public static (float l, float a, float b) XyzToLab(float x, float y, float z)
    {
        (x, y, z) = NormalizeXyz(x, y, z);

        x = x > _epsilon ? (float)Math.Pow(x, _oneThird) : (_kappa * x + 16f) / 116f;
        y = y > _epsilon ? (float)Math.Pow(y, _oneThird) : (_kappa * y + 16f) / 116f;
        z = z > _epsilon ? (float)Math.Pow(z, _oneThird) : (_kappa * z + 16f) / 116f;

        var l = 116f * y - 16f;
        var a = 500f * (x - y);
        var b = 200f * (y - z);

        return (l, a, b);
    }

    public static (float x, float y, float z) LabToXyz(float l, float a, float b)
    {
        var refX = 95.047f;
        var refY = 100.0f;
        var refZ = 108.883f;

        var y = (l + 16f) / 116f;
        var x = a / 500f + y;
        var z = y - b / 200f;

        var xPow3 = (float)Math.Pow(x, 3f);
        var yPow3 = (float)Math.Pow(y, 3f);
        var zPow3 = (float)Math.Pow(z, 3f);

        x = xPow3 > _epsilon ? xPow3 : (x - _16_116) / 7.787f;
        y = yPow3 > _epsilon ? yPow3 : (y - _16_116) / 7.787f;
        z = zPow3 > _epsilon ? zPow3 : (z - _16_116) / 7.787f;

        return (x * refX, y * refY, z * refZ);
    }

    private static float ReverseGammaCorrection(float channel)
    {
        return channel > 0.0031308f ? 1.055f * (float)Math.Pow(channel, 1f / 2.4f) - 0.055f : 12.92f * channel;
    }

    public static int ColorFromXyz(float x, float y, float z, float alpha)
    {
        x /= 100f;
        y /= 100f;
        z /= 100f;

        var rLinear = x * 3.2406f + y * -1.5372f + z * -0.4986f;
        var gLinear = x * -0.9689f + y * 1.8758f + z * 0.0415f;
        var bLinear = x * 0.0557f + y * -0.2040f + z * 1.0570f;

        return ColorFromArgb(alpha, rLinear, gLinear, bLinear);
    }

    public static float ToBrightness(this Color color)
    {
        // get brightness
        // accordingly to CCIR601
        return color.Red * 0.299f + color.Green * 0.587f + color.Blue * 0.114f;
    }

    public static bool IsBright(this Color color)
    {
        return color.ToBrightness() >= 0.5f;
    }
}