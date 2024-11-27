using System.Runtime.CompilerServices;

namespace MPowerKit.GoogleMaps;

public class GroundOverlay : VisualElement
{
    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        if (Image is ViewImageSource viewSource && !viewSource.IsEmpty
            && viewSource.View.BindingContext is null)
        {
            viewSource.View.BindingContext = this.BindingContext;
        }
    }

    protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName == ImageProperty.PropertyName
            && Image is ViewImageSource viewSource && !viewSource.IsEmpty
            && viewSource.View.BindingContext is null)
        {
            viewSource.View.BindingContext = this.BindingContext;
        }
    }

    #region Bearing
    public float Bearing
    {
        get { return (float)GetValue(BearingProperty); }
        set { SetValue(BearingProperty, value); }
    }

    public static readonly BindableProperty BearingProperty =
        BindableProperty.Create(
            nameof(Bearing),
            typeof(float),
            typeof(GroundOverlay)
            );
    #endregion

    #region Image
    public ImageSource Image
    {
        get { return (ImageSource)GetValue(ImageProperty); }
        set { SetValue(ImageProperty, value); }
    }

    public static readonly BindableProperty ImageProperty =
        BindableProperty.Create(
            nameof(Image),
            typeof(ImageSource),
            typeof(GroundOverlay)
            );
    #endregion

    #region GroundOverlayPosition
    public GroundOverlayPosition GroundOverlayPosition
    {
        get { return (GroundOverlayPosition)GetValue(GroundOverlayPositionProperty); }
        set { SetValue(GroundOverlayPositionProperty, value); }
    }

    public static readonly BindableProperty GroundOverlayPositionProperty =
        BindableProperty.Create(
            nameof(GroundOverlayPosition),
            typeof(GroundOverlayPosition),
            typeof(GroundOverlay)
            );
    #endregion

    #region Anchor
    public Point Anchor
    {
        get { return (Point)GetValue(AnchorProperty); }
        set { SetValue(AnchorProperty, value); }
    }

    public static readonly BindableProperty AnchorProperty =
        BindableProperty.Create(
            nameof(Anchor),
            typeof(Point),
            typeof(GroundOverlay),
            new Point(0.5, 0.5));
    #endregion
}

public abstract class GroundOverlayPosition
{
    public static GroundOverlayPosition FromBounds(LatLngBounds bounds)
    {
        return new BoundsPosition(bounds);
    }

    public static GroundOverlayPosition FromCenterAndWidth(Point center, float width)
    {
        return new CenterAndWidthPosition(center, width);
    }

    public static GroundOverlayPosition FromCenterAndSize(Point center, Size size)
    {
        return new CenterAndSizePosition(center, size);
    }
}

public class CenterAndWidthPosition : GroundOverlayPosition
{
    public Point Center { get; set; }
    public float Width { get; set; }

    public CenterAndWidthPosition(Point center, float width)
    {
        Center = center;
        Width = width;
    }
}

public class CenterAndSizePosition : GroundOverlayPosition
{
    public Point Center { get; set; }
    public Size Size { get; set; }

    public CenterAndSizePosition(Point center, Size size)
    {
        Center = center;
        Size = size;
    }
}

public class BoundsPosition : GroundOverlayPosition
{
    public LatLngBounds Bounds { get; set; }

    public BoundsPosition(LatLngBounds bounds)
    {
        Bounds = bounds;
    }
}