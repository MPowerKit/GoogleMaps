﻿using Android.Content;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Bumptech.Glide.Load.Resource.Gif;
using Microsoft.Maui.Platform;

using Point = Microsoft.Maui.Graphics.Point;
using Rect = Microsoft.Maui.Graphics.Rect;
using View = Microsoft.Maui.Controls.View;

namespace MPowerKit.GoogleMaps;

public static class Extensions
{
    public static LatLng ToLatLng(this Point point)
    {
        return new(point.X, point.Y);
    }

    public static Point ToCrossPlatformPoint(this LatLng latlng)
    {
        return new(latlng.Latitude, latlng.Longitude);
    }

    public static Android.Graphics.Point ToNativePoint(this Point point, Context context)
    {
        return new((int)context.ToPixels(point.X), (int)context.ToPixels(point.Y));
    }

    public static Point ToCrossPlatformPoint(this Android.Graphics.Point point, Context context)
    {
        return new(context.FromPixels(point.X), context.FromPixels(point.Y));
    }

    public static LatLngBounds ToCrossPlatform(this Android.Gms.Maps.Model.LatLngBounds bounds)
    {
        return new(bounds.Southwest.ToCrossPlatformPoint(), bounds.Northeast.ToCrossPlatformPoint());
    }

    public static Android.Gms.Maps.Model.LatLngBounds ToNative(this LatLngBounds bounds)
    {
        return new(
            bounds.SouthWest.ToLatLng(),
            bounds.NorthEast.ToLatLng()
        );
    }

    public static List<PatternItem> ToPatternItems(this float[] strokeDashPattern)
    {
        if (strokeDashPattern?.Length is null or 0) return [];

        List<PatternItem> patterns = [];

        for (int i = 0; i < strokeDashPattern.Length; i++)
        {
            var value = strokeDashPattern[i];

            patterns.Add(i % 2 == 0
                ? new Dash(value)
                : new Gap(value));
        }

        return patterns;
    }

    public static Android.Views.View ToNative(this View virtualView, IMauiContext context)
    {
        var platfromView = virtualView.ToPlatform(context);
        var size = (virtualView as IView).Measure(double.PositiveInfinity, double.PositiveInfinity);
        virtualView.Arrange(new Rect(0, 0, size.Width, size.Height));

        var width = (int)platfromView.Context.ToPixels(size.Width);
        var height = (int)platfromView.Context.ToPixels(size.Height);

        platfromView.Measure(
            Android.Views.View.MeasureSpec.MakeMeasureSpec(width, MeasureSpecMode.Exactly),
            Android.Views.View.MeasureSpec.MakeMeasureSpec(height, MeasureSpecMode.Exactly)
        );
        platfromView.Layout(0, 0, width, height);

        return platfromView;
    }

    public static Bitmap ToBitmap(this View virtualView, IMauiContext context)
    {
        var platfromView = virtualView.ToNative(context);
        return platfromView.ToBitmap();
    }

    public static Bitmap ToBitmap(this Android.Views.View v)
    {
        var b = Bitmap.CreateBitmap(v.Width, v.Height, Bitmap.Config.Argb8888!);
        Canvas c = new(b);
        v.Draw(c);
        return b;
    }

    public static byte[] ToArray(this Bitmap bitmap)
    {
        using MemoryStream byteStream = new();
        bitmap.Compress(Bitmap.CompressFormat.Png!, 100, byteStream);
        var byteArray = byteStream.ToArray();

        return byteArray;
    }

    public static async Task<BitmapDescriptor> ToBitmapDescriptor(this ImageSource imageSource, IMauiContext mauiContext)
    {
        var drawable = await imageSource.GetPlatformImageAsync(mauiContext);
        Bitmap? bitmap = null;
        if (drawable?.Value is BitmapDrawable bitmapDrawable)
        {
            bitmap = bitmapDrawable.Bitmap;
        }
        else if (drawable?.Value is GifDrawable gifDrawable)
        {
            bitmap = gifDrawable.FirstFrame;
        }

        var desc = BitmapDescriptorFactory.FromBitmap(bitmap);

        return desc;
    }

    public static Bitmap GetBitmap(this string file, Context context)
    {
        Bitmap? bitmap = null;

        if (!System.IO.Path.IsPathRooted(file) || !File.Exists(file))
        {
            var id = context.GetDrawableId(file);
            if (id <= 0) throw new Exception();

            var drawable = context.GetDrawable(id);
            bitmap = (drawable as BitmapDrawable)!.Bitmap!;
        }
        else if (File.Exists(file))
        {
            bitmap = BitmapFactory.DecodeFile(file);
        }

        if (bitmap is null) throw new Exception();

        return bitmap;
    }
}