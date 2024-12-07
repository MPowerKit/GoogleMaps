using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Controls.UserDialogs.Maui;

using Microsoft.Maui.Controls.Shapes;
using MPowerKit.GoogleMaps;

namespace Sample.ViewModels;

public partial class PolylinesPageViewModel : ObservableObject
{
    private Polyline _polyline = new()
    {
        Points = [new Point(0, 0), new Point(10, 10), new Point(-10, 10)],
        Stroke = Colors.Orange,
        StrokeThickness = 20,
    };

    public PolylinesPageViewModel()
    {
        SetupPolylines();
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
    private bool _useGradient;
    [ObservableProperty]
    private string _dashedPattern;
    [ObservableProperty]
    private CameraPosition _cameraPosition;

    [ObservableProperty]
    private bool _useTexture;
    partial void OnUseTextureChanged(bool oldValue, bool newValue)
    {
        PolylineAttached.SetTextureStamp(_polyline, newValue ? "map_pin.png" : null);
    }

    [ObservableProperty]
    private bool _pixelDependentDashedPattern;
    partial void OnPixelDependentDashedPatternChanged(bool oldValue, bool newValue)
    {
        OnPixelDependentDashedPatternChanged();
    }

    private async void OnPixelDependentDashedPatternChanged()
    {
        if (PixelDependentDashedPattern)
        {
            await Task.Delay(1);
        }

        var metersPerPixel = Distance.MetersPerDevicePixel(0, CameraPosition?.Zoom ?? 3);
        var coeficient = PixelDependentDashedPattern ? 1d / metersPerPixel : metersPerPixel;

        var splitted = DashedPattern?
            .Trim()
            .Split(" ", StringSplitOptions.RemoveEmptyEntries)
            .Select(s => float.Parse(s) * (float)coeficient) ?? [];

        DashedPattern = string.Join(" ", splitted.Select(s => PixelDependentDashedPattern ? $"{s:F0}" : $"{s}"));
        ApplyDashPattern(DashedPattern);
    }

    [ObservableProperty]
    private float _strokeStartAlpha;
    partial void OnStrokeStartAlphaChanged(float oldValue, float newValue)
    {
        var colors = GetColors();
        var newStartColor = Color.FromRgba(colors[0].Red, colors[0].Green, colors[0].Blue, newValue);
        if (!UseGradient)
        {
            _polyline.Stroke = newStartColor;
        }
        else
        {
            _polyline.Stroke = new LinearGradientBrush()
            {
                GradientStops = [new GradientStop() { Color = newStartColor }, new GradientStop() { Color = colors[1] }]
            };
        }
    }

    [ObservableProperty]
    private float _strokeStartRed;
    partial void OnStrokeStartRedChanged(float oldValue, float newValue)
    {
        var colors = GetColors();
        var newStartColor = Color.FromRgba(newValue, colors[0].Green, colors[0].Blue, colors[0].Alpha);
        if (!UseGradient)
        {
            _polyline.Stroke = newStartColor;
        }
        else
        {
            _polyline.Stroke = new LinearGradientBrush()
            {
                GradientStops = [new GradientStop() { Color = newStartColor }, new GradientStop() { Color = colors[1] }]
            };
        }
    }

    [ObservableProperty]
    private float _strokeStartGreen;
    partial void OnStrokeStartGreenChanged(float oldValue, float newValue)
    {
        var colors = GetColors();
        var newStartColor = Color.FromRgba(colors[0].Red, newValue, colors[0].Blue, colors[0].Alpha);
        if (!UseGradient)
        {
            _polyline.Stroke = newStartColor;
        }
        else
        {
            _polyline.Stroke = new LinearGradientBrush()
            {
                GradientStops = [new GradientStop() { Color = newStartColor }, new GradientStop() { Color = colors[1] }]
            };
        }
    }

    [ObservableProperty]
    private float _strokeStartBlue;
    partial void OnStrokeStartBlueChanged(float oldValue, float newValue)
    {
        var colors = GetColors();
        var newStartColor = Color.FromRgba(colors[0].Red, colors[0].Green, newValue, colors[0].Alpha);
        if (!UseGradient)
        {
            _polyline.Stroke = newStartColor;
        }
        else
        {
            _polyline.Stroke = new LinearGradientBrush()
            {
                GradientStops = [new GradientStop() { Color = newStartColor }, new GradientStop() { Color = colors[1] }]
            };
        }
    }

    [ObservableProperty]
    private float _strokeEndAlpha;
    partial void OnStrokeEndAlphaChanged(float oldValue, float newValue)
    {
        if (!UseGradient) return;

        var colors = GetColors();

        var newEndColor = Color.FromRgba(colors[1].Red, colors[1].Green, colors[1].Blue, newValue);

        _polyline.Stroke = new LinearGradientBrush()
        {
            GradientStops = [new GradientStop() { Color = colors[0] }, new GradientStop() { Color = newEndColor }]
        };
    }

    [ObservableProperty]
    private float _strokeEndRed;
    partial void OnStrokeEndRedChanged(float oldValue, float newValue)
    {
        if (!UseGradient) return;

        var colors = GetColors();

        var newEndColor = Color.FromRgba(newValue, colors[1].Green, colors[1].Blue, colors[1].Alpha);

        _polyline.Stroke = new LinearGradientBrush()
        {
            GradientStops = [new GradientStop() { Color = colors[0] }, new GradientStop() { Color = newEndColor }]
        };
    }

    [ObservableProperty]
    private float _strokeEndGreen;
    partial void OnStrokeEndGreenChanged(float oldValue, float newValue)
    {
        if (!UseGradient) return;

        var colors = GetColors();

        var newEndColor = Color.FromRgba(colors[1].Red, newValue, colors[1].Blue, colors[1].Alpha);

        _polyline.Stroke = new LinearGradientBrush()
        {
            GradientStops = [new GradientStop() { Color = colors[0] }, new GradientStop() { Color = newEndColor }]
        };
    }

    [ObservableProperty]
    private float _strokeEndBlue;
    partial void OnStrokeEndBlueChanged(float oldValue, float newValue)
    {
        if (!UseGradient) return;

        var colors = GetColors();

        var newEndColor = Color.FromRgba(colors[1].Red, colors[1].Green, newValue, colors[1].Alpha);

        _polyline.Stroke = new LinearGradientBrush()
        {
            GradientStops = [new GradientStop() { Color = colors[0] }, new GradientStop() { Color = newEndColor }]
        };
    }

    private Color[] GetColors()
    {
        Color[] colors = [Colors.Black, Colors.Black];
        if (_polyline.Stroke is SolidColorBrush solidBrush)
            colors = [solidBrush.Color, solidBrush.Color];
        else if (_polyline.Stroke is GradientBrush gradientBrush)
            colors = [gradientBrush.GradientStops[0].Color, gradientBrush.GradientStops[1].Color];
        return colors;
    }

    [ObservableProperty]
    private ObservableCollection<Polyline> _polylines = [];

    private void SetupPolylines()
    {
        Clickable = _polyline.IsEnabled;
        _polyline.SetBinding(Polyline.IsEnabledProperty, new Binding(nameof(Clickable), source: this));
        IsVisible = _polyline.IsVisible;
        _polyline.SetBinding(Polyline.IsVisibleProperty, new Binding(nameof(IsVisible), source: this));
        Points = _polyline.Points;
        _polyline.SetBinding(Polyline.PointsProperty, new Binding(nameof(Points), source: this));
        StrokeThickness = _polyline.StrokeThickness;
        _polyline.SetBinding(Polyline.StrokeThicknessProperty, new Binding(nameof(StrokeThickness), source: this));
        JointType = _polyline.StrokeLineJoin;
        _polyline.SetBinding(Polyline.StrokeLineJoinProperty, new Binding(nameof(JointType), source: this));
        PixelDependentDashedPattern = true;
        _polyline.SetBinding(PolylineAttached.iOSPixelDependentDashedPatternProperty, new Binding(nameof(PixelDependentDashedPattern), source: this));

        StrokeStartAlpha = (_polyline.Stroke as SolidColorBrush).Color.Alpha;
        StrokeStartRed = (_polyline.Stroke as SolidColorBrush).Color.Red;
        StrokeStartGreen = (_polyline.Stroke as SolidColorBrush).Color.Green;
        StrokeStartBlue = (_polyline.Stroke as SolidColorBrush).Color.Blue;

        StrokeEndAlpha = (_polyline.Stroke as SolidColorBrush).Color.Alpha;
        StrokeEndRed = (_polyline.Stroke as SolidColorBrush).Color.Red;
        StrokeEndGreen = (_polyline.Stroke as SolidColorBrush).Color.Green;
        StrokeEndBlue = (_polyline.Stroke as SolidColorBrush).Color.Blue;

        ObservableCollection<Polyline> polylines = [];
        polylines.Add(_polyline);
        Polylines = polylines;
    }

    private Random _rndLatitude = new();
    private Random _rndLongitude = new();

    [RelayCommand]
    private void ApplyDashPattern(string pattern)
    {
        try
        {
            var splitted = pattern?
                .Trim()
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .Select(float.Parse) ?? [];

            _polyline.StrokeDashArray = [..splitted];
        }
        catch (Exception ex)
        {
            UserDialogs.Instance.AlertAsync("Cannot parse dash pattern. You can use only numbers and whitespaces");
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
        _polyline.Points = [new Point(lat1, lat2), new Point(lat2, lon2), new Point(lat3, lon3)];
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
                _polyline.StrokeLineJoin = PenLineJoin.Miter;
                break;
            case "Bevel":
                _polyline.StrokeLineJoin = PenLineJoin.Bevel;
                break;
            case "Round":
                _polyline.StrokeLineJoin = PenLineJoin.Round;
                break;
        }
    }

    [RelayCommand]
    private async Task ChangeCapType()
    {
        var res = await UserDialogs.Instance.ActionSheetAsync(null,
            "Choose line cap type",
            "Cancel",
            buttons:
            [
                "Flat",
                "Square",
                "Round"
            ]);

        switch (res)
        {
            case "Flat":
                _polyline.StrokeLineCap = PenLineCap.Flat;
                break;
            case "Square":
                _polyline.StrokeLineCap = PenLineCap.Square;
                break;
            case "Round":
                _polyline.StrokeLineCap = PenLineCap.Round;
                break;
        }
    }

    [RelayCommand]
    private async Task PolylineClicked(Polyline polyline)
    {
        await UserDialogs.Instance.AlertAsync("Polyline was clicked");
    }
}