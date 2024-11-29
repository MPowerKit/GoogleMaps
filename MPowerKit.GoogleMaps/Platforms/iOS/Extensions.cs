using CoreLocation;

using Google.Maps;

using Microsoft.Maui.Platform;

using UIKit;

namespace MPowerKit.GoogleMaps;

public static class Extensions
{
    public static CLLocationCoordinate2D ToCoord(this Point point)
    {
        return new(point.X, point.Y);
    }

    public static Point ToCrossPlatformPoint(this CLLocationCoordinate2D coord)
    {
        return new(coord.Latitude, coord.Longitude);
    }

    public static MutablePath ToPath(this IEnumerable<Point> points)
    {
        MutablePath path = new();

        foreach (var point in points)
        {
            path.AddLatLon(point.X, point.Y);
        }

        return path;
    }

    public static UIView ToNative(this View virtualView, IMauiContext context)
    {
        var platfromView = virtualView.ToPlatform(context);
        var size = (virtualView as IView).Measure(double.PositiveInfinity, double.PositiveInfinity);
        virtualView.Arrange(new Rect(0, 0, size.Width, size.Height));

        platfromView.Bounds = new(0, 0, size.Width, size.Height);
        return platfromView;
    }

    public static UIImage ToImage(this View virtualView, IMauiContext context)
    {
        var platfromView = virtualView.ToNative(context);
        return platfromView.ToImage();
    }

    public static UIImage ToImage(this UIView v)
    {
        var renderer = new UIGraphicsImageRenderer(new(v.Bounds.Width, v.Bounds.Height));
        return renderer.CreateImage(ctx =>
        {
            v.Layer.RenderInContext(ctx.CGContext);
        });
    }
}