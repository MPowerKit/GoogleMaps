using CoreGraphics;

using UIKit;

namespace MPowerKit.GoogleMaps;

public class HeatMapImageSourceService : ImageSourceService, IImageSourceService<IHeatMapImageSource>
{
    public override Task<IImageSourceServiceResult<UIImage>?> GetImageAsync(IImageSource imageSource, float scale = 1, CancellationToken cancellationToken = default)
    {
        if (imageSource is not IHeatMapImageSource heatMapSource || heatMapSource.IsEmpty)
            return Task.FromResult<IImageSourceServiceResult<UIImage>?>(null);

        var size = heatMapSource.Size;

        var image = ImageFromPixels(heatMapSource.Pixels!, size);

        return Task.FromResult<IImageSourceServiceResult<UIImage>?>(new ImageSourceServiceResult(image, true));
    }

    public static UIImage? ImageFromPixels(IEnumerable<int> pixels, int size)
    {
        var array = (pixels as int[]) ?? [.. pixels];

        var length = size * size;
        // Ensure the pixel array has the correct size
        if (array.Length != length)
        {
            return null;
        }

        // Create a byte array from the ARGB int array
        var pixelBytes = new byte[length << 2];

        Parallel.For(0, array.Length, new ParallelOptions() { MaxDegreeOfParallelism = 4 }, i =>
        {
            var index = i << 2;
            var argb = array[i];
            pixelBytes[index] = (byte)((argb >> 24) & 0xFF); // Alpha
            pixelBytes[index + 1] = (byte)((argb >> 16) & 0xFF); // Red
            pixelBytes[index + 2] = (byte)((argb >> 8) & 0xFF); // Green
            pixelBytes[index + 3] = (byte)(argb & 0xFF); // Blue
        });

        // Define the color space (RGB)
        var colorSpace = CGColorSpace.CreateDeviceRGB();

        // Create a CGDataProvider from the byte array
        using var provider = new CGDataProvider(pixelBytes, 0, pixelBytes.Length);

        // Create a CGImage from the pixel data
        using var cgImage = new CGImage(
            size,
            size,
            8, // bits per component
            32, // bits per pixel
            size << 2, // bytes per row (4 bytes per pixel)
            colorSpace,
            CGBitmapFlags.ByteOrder32Big | CGBitmapFlags.PremultipliedFirst, // ARGB
            provider,
            null, // decode array
            false, // should interpolate
            CGColorRenderingIntent.Default
        );

        if (cgImage is null) return null;

        return UIImage.FromImage(cgImage);
    }
}