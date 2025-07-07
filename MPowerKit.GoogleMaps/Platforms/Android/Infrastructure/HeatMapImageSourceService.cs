using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;

namespace MPowerKit.GoogleMaps;

public class HeatMapImageSourceService : ImageSourceService, IImageSourceService<IHeatMapImageSource>
{
    public override Task<IImageSourceServiceResult<Drawable>?> GetDrawableAsync(IImageSource imageSource, Context context, CancellationToken cancellationToken = default)
    {
        if (imageSource is not IHeatMapImageSource heatMapSource || heatMapSource.IsEmpty)
            return Task.FromResult<IImageSourceServiceResult<Drawable>?>(null);

        var bitmap = GetBitmapFromPixels(heatMapSource.Pixels!, heatMapSource.Size);

        return Task.FromResult<IImageSourceServiceResult<Drawable>?>(new ImageSourceServiceResult(new BitmapDrawable(context.Resources, bitmap)));
    }

    public static Bitmap GetBitmapFromPixels(IEnumerable<int> pixels, int size)
    {
        var array = (pixels as int[]) ?? [.. pixels];

        Bitmap bitmap = Bitmap.CreateBitmap(size, size, Bitmap.Config.Argb8888!);
        bitmap.SetPixels(array, 0, size, 0, 0, size, size);

        return bitmap;
    }
}