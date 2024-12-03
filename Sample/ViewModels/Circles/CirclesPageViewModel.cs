using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

using MPowerKit.GoogleMaps;

namespace Sample.ViewModels;

public partial class CirclesPageViewModel : ObservableObject
{
    private Circle _circle = new()
    {
        Center = new Point(0, 0),
        Fill = Colors.Purple.WithAlpha(0.6f),
        Stroke = Colors.Orange,
        StrokeThickness = 5,
        Radius = Distance.FromKMeters(1000)
    };

    public CirclesPageViewModel()
    {
        SetupCircles();
    }

    [ObservableProperty]
    private bool _clickable;
    [ObservableProperty]
    private bool _isVisible;
    [ObservableProperty]
    private double _radius;
    [ObservableProperty]
    private double _strokeThickness;

    [ObservableProperty]
    private double _centerX;
    partial void OnCenterXChanged(double oldValue, double newValue)
    {
        _circle.Center = new Point(newValue, _circle.Center.Y);
    }

    [ObservableProperty]
    private double _centerY;
    partial void OnCenterYChanged(double oldValue, double newValue)
    {
        _circle.Center = new Point(_circle.Center.X, newValue);
    }

    [ObservableProperty]
    private float _fillAlpha;
    partial void OnFillAlphaChanged(float oldValue, float newValue)
    {
        var color = (_circle.Fill as SolidColorBrush).Color;
        _circle.Fill = Color.FromRgba(color.Red, color.Green, color.Blue, newValue);
    }

    [ObservableProperty]
    private float _fillRed;
    partial void OnFillRedChanged(float oldValue, float newValue)
    {
        var color = (_circle.Fill as SolidColorBrush).Color;
        _circle.Fill = Color.FromRgba(newValue, color.Green, color.Blue, color.Alpha);
    }

    [ObservableProperty]
    private float _fillGreen;
    partial void OnFillGreenChanged(float oldValue, float newValue)
    {
        var color = (_circle.Fill as SolidColorBrush).Color;
        _circle.Fill = Color.FromRgba(color.Red, newValue, color.Blue, color.Alpha);
    }

    [ObservableProperty]
    private float _fillBlue;
    partial void OnFillBlueChanged(float oldValue, float newValue)
    {
        var color = (_circle.Fill as SolidColorBrush).Color;
        _circle.Fill = Color.FromRgba(color.Red, color.Green, newValue, color.Alpha);
    }

    [ObservableProperty]
    private float _strokeAlpha;
    partial void OnStrokeAlphaChanged(float oldValue, float newValue)
    {
        var color = (_circle.Stroke as SolidColorBrush).Color;
        _circle.Stroke = Color.FromRgba(color.Red, color.Green, color.Blue, newValue);
    }

    [ObservableProperty]
    private float _strokeRed;
    partial void OnStrokeRedChanged(float oldValue, float newValue)
    {
        var color = (_circle.Stroke as SolidColorBrush).Color;
        _circle.Stroke = Color.FromRgba(newValue, color.Green, color.Blue, color.Alpha);
    }

    [ObservableProperty]
    private float _strokeGreen;
    partial void OnStrokeGreenChanged(float oldValue, float newValue)
    {
        var color = (_circle.Stroke as SolidColorBrush).Color;
        _circle.Stroke = Color.FromRgba(color.Red, newValue, color.Blue, color.Alpha);
    }

    [ObservableProperty]
    private float _strokeBlue;
    partial void OnStrokeBlueChanged(float oldValue, float newValue)
    {
        var color = (_circle.Stroke as SolidColorBrush).Color;
        _circle.Stroke = Color.FromRgba(color.Red, color.Green, newValue, color.Alpha);
    }

    [ObservableProperty]
    private ObservableCollection<Circle> _circles = [];

    private void SetupCircles()
    {
        Clickable = _circle.IsEnabled;
        _circle.SetBinding(Circle.IsEnabledProperty, new Binding(nameof(Clickable), source: this, mode: BindingMode.TwoWay));
        IsVisible = _circle.IsVisible;
        _circle.SetBinding(Circle.IsVisibleProperty, new Binding(nameof(IsVisible), source: this, mode: BindingMode.TwoWay));
        Radius = _circle.Radius;
        _circle.SetBinding(Circle.RadiusProperty, new Binding(nameof(Radius), source: this, mode: BindingMode.TwoWay));
        StrokeThickness = _circle.StrokeThickness;
        _circle.SetBinding(Circle.StrokeThicknessProperty, new Binding(nameof(StrokeThickness), source: this, mode: BindingMode.TwoWay));
        CenterX = _circle.Center.X;
        CenterY = _circle.Center.Y;

        FillAlpha = (_circle.Fill as SolidColorBrush).Color.Alpha;
        FillRed = (_circle.Fill as SolidColorBrush).Color.Red;
        FillGreen = (_circle.Fill as SolidColorBrush).Color.Green;
        FillBlue = (_circle.Fill as SolidColorBrush).Color.Blue;

        StrokeAlpha = (_circle.Stroke as SolidColorBrush).Color.Alpha;
        StrokeRed = (_circle.Stroke as SolidColorBrush).Color.Red;
        StrokeGreen = (_circle.Stroke as SolidColorBrush).Color.Green;
        StrokeBlue = (_circle.Stroke as SolidColorBrush).Color.Blue;

        ObservableCollection<Circle> circles = [];
        circles.Add(_circle);
        Circles = circles;
    }
}