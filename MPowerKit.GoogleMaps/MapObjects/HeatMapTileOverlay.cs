using System.Runtime.CompilerServices;

namespace MPowerKit.GoogleMaps;

public class HeatMapTileOverlay : TileOverlay
{
    protected virtual HeatmapTileProvider Provider { get; set; }

    public HeatMapTileOverlay()
    {
        Opacity = 0.7;
        TileProvider = GetHeatMapTile;
    }

    protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName == DataProperty.PropertyName && Data is not null)
        {
            Provider?.SetWeightedData(Data);
            ClearTileCache();
        }
        else if (propertyName == GradientProperty.PropertyName)
        {
            Provider?.SetGradient(Gradient);
            ClearTileCache();
        }
        else if (propertyName == RadiusProperty.PropertyName)
        {
            Provider?.SetRadius(Radius);
            ClearTileCache();
        }
        else if (propertyName == MaxIntensityProperty.PropertyName)
        {
            Provider?.SetMaxIntensity(MaxIntensity);
            ClearTileCache();
        }
    }

    #region Data
    public IEnumerable<WeightedLatLng> Data
    {
        get { return (IEnumerable<WeightedLatLng>)GetValue(DataProperty); }
        set { SetValue(DataProperty, value); }
    }

    public static readonly BindableProperty DataProperty =
        BindableProperty.Create(
            nameof(Data),
            typeof(IEnumerable<WeightedLatLng>),
            typeof(HeatMapTileOverlay));
    #endregion

    #region Radius
    public int Radius
    {
        get { return (int)GetValue(RadiusProperty); }
        set { SetValue(RadiusProperty, value); }
    }

    public static readonly BindableProperty RadiusProperty =
        BindableProperty.Create(
            nameof(Radius),
            typeof(int),
            typeof(HeatMapTileOverlay),
#if IOS
            50
#else
            20
#endif
            );
    #endregion

    #region Gradient
    public Gradient Gradient
    {
        get { return (Gradient)GetValue(GradientProperty); }
        set { SetValue(GradientProperty, value); }
    }

    public static readonly BindableProperty GradientProperty =
        BindableProperty.Create(
            nameof(FadeIn),
            typeof(Gradient),
            typeof(HeatMapTileOverlay));
    #endregion

    #region MaxIntensity
    public float MaxIntensity
    {
        get { return (float)GetValue(MaxIntensityProperty); }
        set { SetValue(MaxIntensityProperty, value); }
    }

    public static readonly BindableProperty MaxIntensityProperty =
        BindableProperty.Create(
            nameof(FadeIn),
            typeof(float),
            typeof(HeatMapTileOverlay));
    #endregion

    #region Heat map algorithm

    private ImageSource? GetHeatMapTile(Point point, int zoom, int tileSize)
    {
        return (Provider ??= new HeatmapTileProvider(Data, Gradient, Radius, MaxIntensity)).GetTile((int)point.X, (int)point.Y, zoom);
    }

    #endregion
}

public class HeatmapTileProvider
{
    public const int DefaultRadius = 20;
    public const float DefaultOpacity = 0.7f;
    public const int TileDimension =
#if ANDROID
        512;
#else
        256;
#endif
    public const int ScreenSize = 1280;
    public static int DefaultMinZoom { get; set; } = 2;
    public static int DefaultMaxZoom { get; set; } = 22;
    public const int MaxZoomLevel = 22;
    public const int MinRadius = 10;
    public const int MaxRadius =
#if ANDROID
        100;
#else
        200;
#endif
    public const float WorldWidth = 1f;

    public static Color[] DefaultGradientColors { get; set; } =
    [
        Color.FromRgba(102, 225, 0, 255),
        Color.FromRgba(255, 0, 0, 255)
    ];

    public static float[] DefaultGradientStartPoints { get; set; } = [0.2f, 1f];

    public static readonly Gradient DefaultGradient = new(DefaultGradientColors, DefaultGradientStartPoints);

    private PointQuadTree<WeightedLatLng>? _tree;
    private WeightedLatLng[] _data;
    private Bounds _bounds;
    private int _radius;
    private Gradient _gradient;
    private int[]? _colorMap;
    private float[] _kernel;
    private float[]? _maxIntensity;
    private float _customMaxIntensity;

    public HeatmapTileProvider(IEnumerable<WeightedLatLng> data, Gradient? gradient, int radius, float maxIntensity = 0f)
    {
        _data = [.. data];
        _radius = radius;
        _gradient = gradient ?? DefaultGradient;
        _customMaxIntensity = maxIntensity;

        _kernel = GenerateKernel(_radius, _radius / 3f);
        SetGradient(_gradient);
        SetWeightedData(_data);
    }

    public ImageSource GetTile(int x, int y, int zoom)
    {
        var tileWidth = WorldWidth / (float)Math.Pow(2d, zoom);
        var padding = tileWidth * _radius / TileDimension;
        var tileWidthPadded = tileWidth + 2f * padding;
        var bucketWidth = tileWidthPadded / (TileDimension + (_radius << 1));

        var minX = x * tileWidth - padding;
        var maxX = (x + 1) * tileWidth + padding;
        var minY = y * tileWidth - padding;
        var maxY = (y + 1) * tileWidth + padding;

        var xOffset = 0f;
        List<WeightedLatLng> wrappedPoints = [];

        if (minX < 0)
        {
            var overlapBounds = new Bounds(minX + WorldWidth, WorldWidth, minY, maxY);
            xOffset = -WorldWidth;
            wrappedPoints = _tree!.Search(overlapBounds);
        }
        else if (maxX > WorldWidth)
        {
            var overlapBounds = new Bounds(0d, maxX - WorldWidth, minY, maxY);
            xOffset = WorldWidth;
            wrappedPoints = _tree!.Search(overlapBounds);
        }

        var tileBounds = new Bounds(minX, maxX, minY, maxY);
        var paddedBounds = new Bounds(_bounds.MinX - padding, _bounds.MaxX + padding, _bounds.MinY - padding, _bounds.MaxY + padding);

        if (!tileBounds.Intersects(paddedBounds))
        {
            return NoTileImageSource.Instance;
        }

        var points = _tree!.Search(tileBounds);

        if (points.Count == 0)
        {
            return NoTileImageSource.Instance;
        }

        var intensitySize = TileDimension + (_radius << 1);

        var intensity = new float[intensitySize, intensitySize];

        for (var i = 0; i < points.Count; i++)
        {
            var point = points[i];
            var p = point.Point;
            var bucketX = (int)((p.X - minX) / bucketWidth);
            var bucketY = (int)((p.Y - minY) / bucketWidth);
            intensity[bucketX, bucketY] += point.Intensity;
        }

        for (var i = 0; i < wrappedPoints.Count; i++)
        {
            var point = wrappedPoints[i];
            var p = point.Point;
            var bucketX = (int)((p.X + xOffset - minX) / bucketWidth);
            var bucketY = (int)((p.Y - minY) / bucketWidth);
            intensity[bucketX, bucketY] += point.Intensity;
        }

        var convolved = Convolve(intensity, _kernel);
        var (colors, size) = Colorize(convolved, _colorMap!, _maxIntensity![zoom]);

        return new HeatMapImageSource() { Pixels = colors, Size = size };
    }

    public void SetWeightedData(IEnumerable<WeightedLatLng> data)
    {
        if (data?.Count() is null or 0)
        {
            throw new ArgumentException("No input points.");
        }

        _data = data as WeightedLatLng[] ?? [.. data];
        _bounds = GetBounds(_data);
        _tree = new(_bounds);

        for (var i = 0; i < _data.Length; i++)
        {
            _tree.Add(_data[i]);
        }

        _maxIntensity = GetMaxIntensities(_radius);
    }

    public void SetData(IEnumerable<Point> data)
    {
        SetWeightedData(WrapData(data));
    }

    private static IEnumerable<WeightedLatLng> WrapData(IEnumerable<Point> data)
    {
        return data.Select(l => new WeightedLatLng(l));
    }

    public void SetGradient(Gradient gradient)
    {
        _gradient = gradient ?? DefaultGradient;
        _colorMap = _gradient.GenerateColorMap();
    }

    public void SetRadius(int radius)
    {
        if (radius < MinRadius || radius > MaxRadius)
            throw new ArgumentException("Radius not within bounds.");
        _radius = radius;
        _kernel = GenerateKernel(_radius, _radius / 3f);
        _maxIntensity = GetMaxIntensities(_radius);
    }

    public void SetMaxIntensity(float intensity)
    {
        _customMaxIntensity = intensity;
        _maxIntensity = GetMaxIntensities(_radius);
    }

    private float[] GetMaxIntensities(int radius)
    {
        Span<float> maxIntensityArray = stackalloc float[MaxZoomLevel];

        if (_customMaxIntensity != 0f)
        {
            maxIntensityArray.Fill(_customMaxIntensity);
            return maxIntensityArray.ToArray();
        }

        for (var i = DefaultMinZoom; i < DefaultMaxZoom; i++)
        {
            maxIntensityArray[i] = (float)GetMaxValue(_data, _bounds, radius, (int)(ScreenSize * Math.Pow(2d, i - 3)));
            if (i == DefaultMinZoom)
            {
                var slice = maxIntensityArray[..i];
                slice.Fill(maxIntensityArray[i]);
            }
        }

        var lastSlice = maxIntensityArray[DefaultMaxZoom..MaxZoomLevel];
        lastSlice.Fill(maxIntensityArray[DefaultMaxZoom - 1]);

        return maxIntensityArray.ToArray();
    }

    private static Bounds GetBounds(IEnumerable<WeightedLatLng> points)
    {
        var first = points.First();
        double minX = first.Point.X, maxX = first.Point.X, minY = first.Point.Y, maxY = first.Point.Y;

        foreach (var point in points.Skip(1))
        {
            var p = point.Point;
            minX = Math.Min(minX, p.X);
            maxX = Math.Max(maxX, p.X);
            minY = Math.Min(minY, p.Y);
            maxY = Math.Max(maxY, p.Y);
        }

        return new(minX, maxX, minY, maxY);
    }

    private static float[] GenerateKernel(int radius, float sd)
    {
        var scale = -1f / (2f * sd * sd);
        Span<float> kernel = stackalloc float[(radius << 1) + 1];
        for (var i = -radius; i <= radius; i++)
        {
            kernel[i + radius] = (float)Math.Exp(i * i * scale);
        }
        return kernel.ToArray();
    }

    private static float[,] Convolve(float[,] grid, float[] kernel)
    {
        var radius = kernel.Length / 2;
        var dimOld = grid.GetLength(0);
        var dim = dimOld - (radius << 1);
        var lowerLimit = radius;
        var upperLimit = radius + dim - 1;

        var intermediate = new float[dimOld, dimOld];

        for (int x = 0; x < dimOld; x++)
        {
            var xpRadius = x + radius;
            var xmRadius = x - radius;

            for (var y = 0; y < dimOld; y++)
            {
                var val = grid[x, y];
                if (val == 0f) continue;

                var xUpperLimit = Math.Min(upperLimit, xpRadius) + 1;
                var initial = Math.Max(lowerLimit, xmRadius);
                for (var x2 = initial; x2 < xUpperLimit; x2++)
                {
                    intermediate[x2, y] += val * kernel[x2 - xmRadius];
                }
            }
        }

        var outputGrid = new float[dim, dim];

        for (int x = lowerLimit; x <= upperLimit; x++)
        {
            var xmRadius = x - radius;

            for (var y = 0; y < dimOld; y++)
            {
                var val = intermediate[x, y];
                if (val == 0f) continue;

                var ymRadius = y - radius;

                var yUpperLimit = Math.Min(upperLimit, y + radius) + 1;
                var initial = Math.Max(lowerLimit, ymRadius);
                for (var y2 = initial; y2 < yUpperLimit; y2++)
                {
                    outputGrid[xmRadius, y2 - radius] += val * kernel[y2 - ymRadius];
                }
            }
        }

        return outputGrid;
    }

    private static (int[] colors, int size) Colorize(float[,] grid, int[] colorMap, float max)
    {
        var maxColor = colorMap[^1];
        var colorMapScaling = (colorMap.Length - 1) / max;
        var dim = grid.GetLength(0);
        var colors = new int[dim * dim];

        var transparent = Colors.Transparent.ToInt();

        for (int i = 0; i < dim; i++)
        {
            var iDim = i * dim;

            for (var j = 0; j < dim; j++)
            {
                var val = grid[j, i];
                var index = iDim + j;
                var col = (int)(val * colorMapScaling);

                colors[index] = val != 0f
                    ? (col < colorMap.Length ? colorMap[col] : maxColor)
                    : transparent;
            }
        }

        return (colors, dim);
    }

    private static float GetMaxValue(WeightedLatLng[] points, Bounds bounds, int radius, int screenDim)
    {
        var boundsDim = (float)Math.Max(bounds.MaxX - bounds.MinX, bounds.MaxY - bounds.MinY);
        var nBuckets = (int)(screenDim / (radius << 1) + 0.5f);
        var scale = nBuckets / boundsDim;

        var buckets = new Dictionary<long, Dictionary<long, float>>();
        var max = 0f;

        for (int i = 0; i < points.Length; i++)
        {
            var point = points[i];
            var xBucket = (long)((point.Point.X - bounds.MinX) * scale);
            var yBucket = (long)((point.Point.Y - bounds.MinY) * scale);

            if (!buckets.TryGetValue(xBucket, out Dictionary<long, float>? value))
            {
                value = [];
                buckets[xBucket] = value;
            }

            if (!value.ContainsKey(yBucket))
            {
                value[yBucket] = 0f;
            }

            value[yBucket] += point.Intensity;
            max = Math.Max(max, value[yBucket]);
        }

        return max;
    }
}