using UIKit;

namespace MPowerKit.GoogleMaps;

public class ViewImageSourceService : ImageSourceService, IImageSourceService<IViewImageSource>
{
    public override Task<IImageSourceServiceResult<UIImage>?> GetImageAsync(IImageSource imageSource, float scale = 1, CancellationToken cancellationToken = default)
    {
        if (imageSource is not IViewImageSource viewImageSource)
            return Task.FromResult<IImageSourceServiceResult<UIImage>?>(null);

        var view = viewImageSource.View;

        if (view is null)
            return Task.FromResult<IImageSourceServiceResult<UIImage>?>(null);

        var image = view.ToImage(Application.Current!.Handler.MauiContext!);

        return Task.FromResult<IImageSourceServiceResult<UIImage>?>(new ImageSourceServiceResult(image, true));
    }
}