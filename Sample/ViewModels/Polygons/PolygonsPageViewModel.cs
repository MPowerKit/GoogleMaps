using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Controls.UserDialogs.Maui;

using Microsoft.Maui.Controls.Shapes;

namespace Sample.ViewModels;

public partial class PolygonsPageViewModel : ObservableObject
{
    private Polygon _polygon = new()
    {
        Points = [new Point(0, 0), new Point(10, 10), new Point(-10, 10)],
        Fill = Colors.Purple.WithAlpha(0.6f),
        Stroke = Colors.Orange,
        StrokeThickness = 5
    };

    public PolygonsPageViewModel()
    {
        SetupPolygons();
    }

    [ObservableProperty]
    private bool _clickable;
    [ObservableProperty]
    private bool _isVisible;
    [ObservableProperty]
    private PointCollection _points;
    [ObservableProperty]
    private double _strokeThickness;
    [ObservableProperty]
    private PenLineJoin _jointType;

    [ObservableProperty]
    private float _fillAlpha;
    partial void OnFillAlphaChanged(float oldValue, float newValue)
    {
        var color = (_polygon.Fill as SolidColorBrush).Color;
        _polygon.Fill = Color.FromRgba(color.Red, color.Green, color.Blue, newValue);
    }

    [ObservableProperty]
    private float _fillRed;
    partial void OnFillRedChanged(float oldValue, float newValue)
    {
        var color = (_polygon.Fill as SolidColorBrush).Color;
        _polygon.Fill = Color.FromRgba(newValue, color.Green, color.Blue, color.Alpha);
    }

    [ObservableProperty]
    private float _fillGreen;
    partial void OnFillGreenChanged(float oldValue, float newValue)
    {
        var color = (_polygon.Fill as SolidColorBrush).Color;
        _polygon.Fill = Color.FromRgba(color.Red, newValue, color.Blue, color.Alpha);
    }

    [ObservableProperty]
    private float _fillBlue;
    partial void OnFillBlueChanged(float oldValue, float newValue)
    {
        var color = (_polygon.Fill as SolidColorBrush).Color;
        _polygon.Fill = Color.FromRgba(color.Red, color.Green, newValue, color.Alpha);
    }

    [ObservableProperty]
    private float _strokeAlpha;
    partial void OnStrokeAlphaChanged(float oldValue, float newValue)
    {
        var color = (_polygon.Stroke as SolidColorBrush).Color;
        _polygon.Stroke = Color.FromRgba(color.Red, color.Green, color.Blue, newValue);
    }

    [ObservableProperty]
    private float _strokeRed;
    partial void OnStrokeRedChanged(float oldValue, float newValue)
    {
        var color = (_polygon.Stroke as SolidColorBrush).Color;
        _polygon.Stroke = Color.FromRgba(newValue, color.Green, color.Blue, color.Alpha);
    }

    [ObservableProperty]
    private float _strokeGreen;
    partial void OnStrokeGreenChanged(float oldValue, float newValue)
    {
        var color = (_polygon.Stroke as SolidColorBrush).Color;
        _polygon.Stroke = Color.FromRgba(color.Red, newValue, color.Blue, color.Alpha);
    }

    [ObservableProperty]
    private float _strokeBlue;
    partial void OnStrokeBlueChanged(float oldValue, float newValue)
    {
        var color = (_polygon.Stroke as SolidColorBrush).Color;
        _polygon.Stroke = Color.FromRgba(color.Red, color.Green, newValue, color.Alpha);
    }

    [ObservableProperty]
    private ObservableCollection<Polygon> _polygons = [];

    private void SetupPolygons()
    {
        Clickable = _polygon.IsEnabled;
        _polygon.SetBinding(Polygon.IsEnabledProperty, new Binding(nameof(Clickable), source: this));
        IsVisible = _polygon.IsVisible;
        _polygon.SetBinding(Polygon.IsVisibleProperty, new Binding(nameof(IsVisible), source: this));
        Points = _polygon.Points;
        _polygon.SetBinding(Polygon.PointsProperty, new Binding(nameof(Points), source: this));
        StrokeThickness = _polygon.StrokeThickness;
        _polygon.SetBinding(Polygon.StrokeThicknessProperty, new Binding(nameof(StrokeThickness), source: this));
        JointType = _polygon.StrokeLineJoin;
        _polygon.SetBinding(Polygon.StrokeLineJoinProperty, new Binding(nameof(JointType), source: this));

        FillAlpha = (_polygon.Fill as SolidColorBrush).Color.Alpha;
        FillRed = (_polygon.Fill as SolidColorBrush).Color.Red;
        FillGreen = (_polygon.Fill as SolidColorBrush).Color.Green;
        FillBlue = (_polygon.Fill as SolidColorBrush).Color.Blue;

        StrokeAlpha = (_polygon.Stroke as SolidColorBrush).Color.Alpha;
        StrokeRed = (_polygon.Stroke as SolidColorBrush).Color.Red;
        StrokeGreen = (_polygon.Stroke as SolidColorBrush).Color.Green;
        StrokeBlue = (_polygon.Stroke as SolidColorBrush).Color.Blue;

        ObservableCollection<Polygon> polygons = [];
        polygons.Add(_polygon);
        Polygons = polygons;
    }

    private Random _rndLatitude = new();
    private Random _rndLongitude = new();

    [RelayCommand]
    private async Task ApplyDashPattern(string pattern)
    {
        try
        {
            var splitted = pattern
                .Trim()
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .Select(float.Parse);

            _polygon.StrokeDashArray = [..splitted];
        }
        catch (Exception ex)
        {
            await UserDialogs.Instance.AlertAsync("Cannot parse dash pattern. You can use only numbers and whitespaces");
        }
    }

    [RelayCommand]
    private void RandomizePoints()
    {
        var lat1 = _rndLatitude.Next(-90, 90);
        var lat2 = _rndLatitude.Next(-90, 90);
        var lat3 = _rndLatitude.Next(-90, 90);
        var lon1 = _rndLatitude.Next(-180, 180);
        var lon2 = _rndLatitude.Next(-180, 180);
        var lon3 = _rndLatitude.Next(-180, 180);
        _polygon.Points = [new Point(lat1, lat2), new Point(lat2, lon2), new Point(lat3, lon3)];
    }

    [RelayCommand]
    private async Task ChangeJointType()
    {
        var res = await UserDialogs.Instance.ActionSheetAsync(null,
            "Choose line joint type",
            "Cancel",
            buttons:
            [
                "Default",
                "Bevel",
                "Round"
            ]);

        switch (res)
        {
            case "Default":
                _polygon.StrokeLineJoin = PenLineJoin.Miter;
                break;
            case "Bevel":
                _polygon.StrokeLineJoin = PenLineJoin.Bevel;
                break;
            case "Round":
                _polygon.StrokeLineJoin = PenLineJoin.Round;
                break;
        }
    }

    [RelayCommand]
    private async Task PolygonClicked(Polygon polygon)
    {
        await UserDialogs.Instance.AlertAsync("Polygon was clicked");
    }
}