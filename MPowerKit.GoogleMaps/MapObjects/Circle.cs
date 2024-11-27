using Microsoft.Maui.Controls.Shapes;

namespace MPowerKit.GoogleMaps;

public sealed class Circle : Shape
{
    public override PathF GetPath()
    {
        throw new NotImplementedException();
    }

    #region Center
    public Point Center
    {
        get { return (Point)GetValue(CenterProperty); }
        set { SetValue(CenterProperty, value); }
    }

    public static readonly BindableProperty CenterProperty =
        BindableProperty.Create(
            nameof(Center),
            typeof(Point),
            typeof(Circle)
            );
    #endregion

    #region Radius
    public double Radius
    {
        get { return (double)GetValue(RadiusProperty); }
        set { SetValue(RadiusProperty, value); }
    }

    public static readonly BindableProperty RadiusProperty =
        BindableProperty.Create(
            nameof(Radius),
            typeof(double),
            typeof(Circle)
            );
    #endregion
}