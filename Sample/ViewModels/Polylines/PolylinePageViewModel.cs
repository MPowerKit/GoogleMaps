using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Controls.UserDialogs.Maui;
using Microsoft.Maui.Controls.Shapes;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Sample.ViewModels
{
    public partial class PolylinePageViewModel : ObservableObject
    {
        private readonly Polyline _polyline = new Polyline();


        [ObservableProperty]
        private ObservableCollection<Polyline> _polylines = [];
        [ObservableProperty]
        private double _lineWidth;
        [ObservableProperty]
        private bool _lineGradient;
        [ObservableProperty]
        private bool _lineStroke;
        [ObservableProperty]
        private bool _lineEnabled;
        [ObservableProperty]
        private bool _lineVisible;
        [ObservableProperty]
        private bool _capButtonsVisible;

        #region stroke
        [ObservableProperty]
        private float _fillAlpha;
        partial void OnFillAlphaChanged(float oldValue, float newValue)
        {
            var color = (_polyline.Stroke as SolidColorBrush).Color;
            _polyline.Stroke = Color.FromRgba(color.Red, color.Green, color.Blue, newValue);
        }

        [ObservableProperty]
        private float _fillRed;
        partial void OnFillRedChanged(float oldValue, float newValue)
        {
            var color = (_polyline.Stroke as SolidColorBrush).Color;
            _polyline.Stroke = Color.FromRgba(newValue, color.Green, color.Blue, color.Alpha);
        }

        [ObservableProperty]
        private float _fillGreen;
        partial void OnFillGreenChanged(float oldValue, float newValue)
        {
            var color = (_polyline.Stroke as SolidColorBrush).Color;
            _polyline.Stroke = Color.FromRgba(color.Red, newValue, color.Blue, color.Alpha);
        }

        [ObservableProperty]
        private float _fillBlue;
        partial void OnFillBlueChanged(float oldValue, float newValue)
        {
            if (_polyline.Stroke == null)
            {
                _polyline.Stroke = new SolidColorBrush();
            }
            var color = (_polyline.Stroke as SolidColorBrush).Color;
            _polyline.Stroke = Color.FromRgba(color.Red, color.Green, newValue, color.Alpha);
        }
        #endregion
        #region Gradient
        [ObservableProperty]
        private float _gradientAAlpha;
        partial void OnGradientAAlphaChanged(float oldValue, float newValue)
        {
            var stop = (_polyline.Stroke as LinearGradientBrush).GradientStops[0];
            var color = stop.Color;
            stop.Color = Color.FromRgba(color.Red, color.Green, color.Blue, newValue);
        }
        [ObservableProperty]
        private float _gradientARed;
        partial void OnGradientARedChanged(float oldValue, float newValue)
        {
            var stop = (_polyline.Stroke as LinearGradientBrush).GradientStops[0];
            var color = stop.Color;
            stop.Color = Color.FromRgba(newValue, color.Green, color.Blue, color.Alpha);
        }
        [ObservableProperty]
        private float _gradientAGreen;
        partial void OnGradientAGreenChanged(float oldValue, float newValue)
        {
            var stop = (_polyline.Stroke as LinearGradientBrush).GradientStops[0];
            var color = stop.Color;
            stop.Color = Color.FromRgba(color.Red, newValue, color.Blue, color.Alpha);
        }
        [ObservableProperty]
        private float _gradientABlue;
        partial void OnGradientABlueChanged(float oldValue, float newValue)
        {
            var stop = (_polyline.Stroke as LinearGradientBrush).GradientStops[0];
            var color = stop.Color;
            stop.Color = Color.FromRgba(color.Red, color.Green, newValue, color.Alpha);
        }

        [ObservableProperty]
        private float _gradientBAlpha;
        partial void OnGradientBAlphaChanged(float oldValue, float newValue)
        {
            var stop = (_polyline.Stroke as LinearGradientBrush).GradientStops[0];
            var color = stop.Color;
            stop.Color = Color.FromRgba(color.Red, color.Green, color.Blue, newValue);
        }
        [ObservableProperty]
        private float _gradientBRed;
        partial void OnGradientBRedChanged(float oldValue, float newValue)
        {
            var stop = (_polyline.Stroke as LinearGradientBrush).GradientStops[0];
            var color = stop.Color;
            stop.Color = Color.FromRgba(newValue, color.Green, color.Blue, color.Alpha);
        }
        [ObservableProperty]
        private float _gradientBGreen;
        partial void OnGradientBGreenChanged(float oldValue, float newValue)
        {
            var stop = (_polyline.Stroke as LinearGradientBrush).GradientStops[0];
            var color = stop.Color;
            stop.Color = Color.FromRgba(color.Red, newValue, color.Blue, color.Alpha);
        }
        [ObservableProperty]
        private float _gradientBBlue;
        partial void OnGradientBBlueChanged(float oldValue, float newValue)
        {
            var stop = (_polyline.Stroke as LinearGradientBrush).GradientStops[0];
            var color = stop.Color;
            stop.Color = Color.FromRgba(color.Red, color.Green, newValue, color.Alpha);
        }
        #endregion
        public PolylinePageViewModel()
        {
            SetupPolylines();
        }
        [RelayCommand]
        private void PolylineClick(Polyline pLine)
        {
            UserDialogs.Instance.Alert("PolylineClicked");
        }
        [RelayCommand]
        private async Task ChangeCap()
        {
            var res = await UserDialogs.Instance.ActionSheetAsync(null,
                "Choose line Cap",
                "Cancel",
                buttons:
                [
                    "Default",
                    "Flat",
                    "Round",
                    "Square"
                ]);

            switch (res)
            {
                case "Default":
                    _polyline.StrokeLineCap = PenLineCap.Flat;
                    break;
                case "Flat":
                    _polyline.StrokeLineCap = PenLineCap.Flat;
                    break;
                case "Round":
                    _polyline.StrokeLineCap = PenLineCap.Round;
                    break;
                case "Square":
                    _polyline.StrokeLineCap = PenLineCap.Square;
                    break;
            }
        }
        [RelayCommand]
        private async Task ChangeJoint()
        {
            var res = await UserDialogs.Instance.ActionSheetAsync(null,
                "Choose line joint",
                "Cancel",
                buttons:
                [
                    "Default",
                    "Round",
                    "Bevel",
                    "Miter"
                ]);

            switch (res)
            {
                case "Default":
                    _polyline.StrokeLineJoin = PenLineJoin.Bevel;
                    break;
                case "Round":
                    _polyline.StrokeLineJoin = PenLineJoin.Round;
                    break;
                case "Bevel":
                    _polyline.StrokeLineJoin = PenLineJoin.Bevel;
                    break;
                case "Miter":
                    _polyline.StrokeLineJoin = PenLineJoin.Miter;
                    break;
            }
        }
        [RelayCommand]
        private void ChangeDashedState()
        {
            if (_polyline.StrokeDashArray.Count == 0)
            {
                var coll = new DoubleCollection([10, 20]);
                _polyline.StrokeDashArray = coll;
            }
            else
            {
                _polyline.StrokeDashArray = new DoubleCollection();
            }
        }
        [RelayCommand]
        private void RandomizeLine()
        {
            SetPolyline();
        }
        private void SetupPolylines()
        {

            LineGradient = false;
            LineStroke = true;
#if IOS
            CapButtonsVisible = false;
#else
            CapButtonsVisible = true;
#endif
            _polyline.Stroke = new SolidColorBrush(Colors.Black);
            FillAlpha = (_polyline.Stroke as SolidColorBrush).Color.Alpha;
            FillRed = (_polyline.Stroke as SolidColorBrush).Color.Red;
            FillGreen = (_polyline.Stroke as SolidColorBrush).Color.Green;
            FillBlue = (_polyline.Stroke as SolidColorBrush).Color.Blue;

            LineWidth = _polyline.StrokeThickness;
            _polyline.SetBinding(Polyline.StrokeThicknessProperty, new Binding(nameof(LineWidth), source: this, mode: BindingMode.TwoWay));
            LineEnabled = _polyline.IsEnabled;
            _polyline.SetBinding(Polyline.IsEnabledProperty, new Binding(nameof(LineEnabled), source: this, mode: BindingMode.TwoWay));
            LineVisible = _polyline.IsVisible;
            _polyline.SetBinding(Polyline.IsVisibleProperty, new Binding(nameof(LineVisible), source: this, mode: BindingMode.TwoWay));
            LineWidth = 6;
            SetPolyline();
        }

        private void SetPolyline()
        {
            _polyline.Points.Clear();
            Polylines.Clear();
            int pointCount = GetRandomPointsCount();

            for (int i = 0; i < pointCount; i++)
            {
                _polyline.Points.Add(GetRandomPoint());
            }
            Polylines.Add(_polyline);
        }
        private int GetRandomPointsCount()
        {
            Random rnd = new Random();
            return rnd.Next(4,10);
        }
        private Point GetRandomPoint()
        {
            Random rnd = new Random();
            double lat = rnd.Next(-90,90);
            double lon = rnd.Next(-180,180);
            return new Point(lat, lon);
        }
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.PropertyName == nameof(LineGradient))
            {
                if (LineGradient)
                {
                    var gradientStops = new GradientStopCollection();
                    gradientStops.Add(new GradientStop(Colors.Black, 0));
                    gradientStops.Add(new GradientStop(Colors.Black, 1));

                    _polyline.Stroke = new LinearGradientBrush(gradientStops);
                    GradientAAlpha = (_polyline.Stroke as LinearGradientBrush).GradientStops[0].Color.Alpha;
                    GradientARed = (_polyline.Stroke as LinearGradientBrush).GradientStops[0].Color.Red;
                    GradientAGreen = (_polyline.Stroke as LinearGradientBrush).GradientStops[0].Color.Green;
                    GradientABlue = (_polyline.Stroke as LinearGradientBrush).GradientStops[0].Color.Blue;

                    GradientBAlpha = (_polyline.Stroke as LinearGradientBrush).GradientStops[1].Color.Alpha;
                    GradientBRed = (_polyline.Stroke as LinearGradientBrush).GradientStops[1].Color.Red;
                    GradientBGreen = (_polyline.Stroke as LinearGradientBrush).GradientStops[1].Color.Green;
                    GradientBBlue = (_polyline.Stroke as LinearGradientBrush).GradientStops[1].Color.Blue;
                }
                else
                {
                    _polyline.Stroke = new SolidColorBrush(Colors.Black);
                    FillAlpha = (_polyline.Stroke as SolidColorBrush).Color.Alpha;
                    FillRed = (_polyline.Stroke as SolidColorBrush).Color.Red;
                    FillGreen = (_polyline.Stroke as SolidColorBrush).Color.Green;
                    FillBlue = (_polyline.Stroke as SolidColorBrush).Color.Blue;
                }
                
                LineStroke = !LineGradient;
            }

        }
    }
}
