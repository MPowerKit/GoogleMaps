using Microsoft.Maui.Controls.Shapes;

namespace MPowerKit.GoogleMaps;

public class DefaultClusterIconTemplateSelector : DataTemplateSelector
{
    public static int[] Buckets { get; set; } = [10, 20, 50, 100, 200, 500, 1000];

    protected override DataTemplate? OnSelectTemplate(object item, BindableObject container)
    {
        if (item is not Cluster cluster || container is not GoogleMap gMap) return null;

        var useBuckets = gMap.UseBucketsForClusters;
        var size = cluster.Size;

        var (bucket, dimension, fontSize, text) = GetClusterValues(size);

        var color = GetClusterColor(useBuckets ? bucket : size);
        var textColor = color.IsBright() ? Colors.Black : Colors.White;

        return new DataTemplate(() =>
        {
            var border = new Border()
            {
                WidthRequest = dimension,
                HeightRequest = dimension,
                Stroke = Colors.White,
                StrokeThickness = 3d,
                StrokeShape = new RoundRectangle()
                {
                    CornerRadius = dimension / 2d
                },
                BackgroundColor = color,
                Content = new Label()
                {
                    TextColor = textColor,
                    FontSize = fontSize,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    Text = useBuckets ? text : size.ToString()
                }
            };

            return (ViewImageSource)border;
        });
    }

    public static (int size, double dimension, double fonSize, string text) GetClusterValues(int size)
    {
        var dim = 40d;
        var font = 12d;
        var bucket = size;
        if (size <= Buckets[0]) return (bucket, dim, font, bucket.ToString());

        var length = Buckets.Length - 1;
        for (var i = 0; i < length; i++)
        {
            dim += 5d;
            font += 1d;
            bucket = Buckets[i];
            if (size < Buckets[i + 1])
            {
                return (bucket, dim, font, $"{bucket}+");
            }
        }
        bucket = Buckets[length];
        dim += 5;
        font += 1;
        return (Buckets[length], dim, font, $"{bucket}+");
    }

    public static Color GetClusterColor(int clusterSize)
    {
        var hueRange = 220f;
        var sizeRange = 400f;
        var size = Math.Min(clusterSize, sizeRange);
        var newSize = sizeRange - size;
        var hue = newSize * newSize / (sizeRange * sizeRange) * hueRange / 360f;
        return Color.FromHsv(hue, 1f, .6f);
    }
}
