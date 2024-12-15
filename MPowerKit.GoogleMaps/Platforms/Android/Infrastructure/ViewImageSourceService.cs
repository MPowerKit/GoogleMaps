using Android.Content;
using Android.Graphics.Drawables;

namespace MPowerKit.GoogleMaps;

public class ViewImageSourceService : ImageSourceService, IImageSourceService<IViewImageSource>
{
    public override Task<IImageSourceServiceResult<Drawable>?> GetDrawableAsync(IImageSource imageSource, Context context, CancellationToken cancellationToken = default)
    {
        if (imageSource is not IViewImageSource viewImageSource)
            return Task.FromResult<IImageSourceServiceResult<Drawable>?>(null);

        var view = viewImageSource.View;

        if (view is null) return Task.FromResult<IImageSourceServiceResult<Drawable>?>(null);

        var bitmap = view.ToBitmap(Application.Current!.Handler.MauiContext!);

        return Task.FromResult<IImageSourceServiceResult<Drawable>?>(new ImageSourceServiceResult(new BitmapDrawable(context.Resources, bitmap)));
    }
}