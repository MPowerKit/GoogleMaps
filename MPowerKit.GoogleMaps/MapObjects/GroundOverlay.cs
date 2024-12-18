using System.Runtime.CompilerServices;

namespace MPowerKit.GoogleMaps;

public class GroundOverlay : VisualElement
{
    public GroundOverlay()
    {
        AnchorX = 0.5;
        AnchorY = 0.5;
    }

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

    #region Position
    public Point Position
    {
        get { return (Point)GetValue(PositionProperty); }
        set { SetValue(PositionProperty, value); }
    }

    public static readonly BindableProperty PositionProperty =
        BindableProperty.Create(
            nameof(Position),
            typeof(Point),
            typeof(GroundOverlay)
            );
    #endregion

    #region OverlayBounds
    public LatLngBounds? OverlayBounds
    {
        get { return (LatLngBounds?)GetValue(OverlayBoundsProperty); }
        set { SetValue(OverlayBoundsProperty, value); }
    }

    public static readonly BindableProperty OverlayBoundsProperty =
        BindableProperty.Create(
            nameof(OverlayBounds),
            typeof(LatLngBounds?),
            typeof(GroundOverlay)
            );
    #endregion
}