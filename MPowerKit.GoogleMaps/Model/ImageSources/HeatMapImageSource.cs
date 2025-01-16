namespace MPowerKit.GoogleMaps;

public interface IHeatMapImageSource : IImageSource
{
    IEnumerable<int>? Pixels { get; }
    int Size { get; }
}

public class HeatMapImageSource : ImageSource, IHeatMapImageSource
{
    public override bool IsEmpty => Pixels?.Count() is null or 0 || Size == 0;

    #region Pixels
    public IEnumerable<int> Pixels
    {
        get { return (IEnumerable<int>)GetValue(PixelsProperty); }
        set { SetValue(PixelsProperty, value); }
    }

    public static readonly BindableProperty PixelsProperty =
        BindableProperty.Create(
            nameof(Pixels),
            typeof(IEnumerable<int>),
            typeof(ViewImageSource)
            );
    #endregion

    #region Size
    public int Size
    {
        get { return (int)GetValue(SizeProperty); }
        set { SetValue(SizeProperty, value); }
    }

    public static readonly BindableProperty SizeProperty =
        BindableProperty.Create(
            nameof(Size),
            typeof(int),
            typeof(ViewImageSource)
            );
    #endregion
}