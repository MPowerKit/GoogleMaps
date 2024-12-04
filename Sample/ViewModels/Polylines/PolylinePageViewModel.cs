using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Controls.UserDialogs.Maui;
using Microsoft.Maui.Controls.Shapes;
using System.Collections.ObjectModel;

namespace Sample.ViewModels
{
    public partial class PolylinePageViewModel : ObservableObject
    {
        private readonly Polyline _polyline = new Polyline();
        private readonly Polyline _polyline2 = new Polyline();
        private List<Point> _pointsForPolyline;
        private List<Point> _pointsForPolyline2;
        private Dictionary<int, Color> _randomColors = new Dictionary<int, Color>()
        {
            {1, Colors.Green },
            {2, Colors.DimGray },
            {3, Colors.Goldenrod },
            {4, Colors.LightSlateGray },
            {5, Colors.MintCream },
            {6, Colors.MediumBlue },
            {7, Colors.Yellow },
            {8, Colors.Violet },
            {9, Colors.Tomato },
            {10, Colors.Sienna }
        };


        [ObservableProperty]
        private ObservableCollection<Polyline> _polylines = [];
        [ObservableProperty]
        private double _lineWidth;
        [ObservableProperty]
        private bool _lineEnabled;
        [ObservableProperty]
        private bool _lineVisible;
        [ObservableProperty]
        private bool _capButtonsVisible;
        public PolylinePageViewModel()
        {
            _pointsForPolyline = new List<Point>()
            {
                new Point(2.538370, -19.699847),
                new Point(11.241577, -31.460428),
                new Point(21.004873, -20.016478),
                new Point(16.205252, -2.330373),
                new Point(5.651094, -5.406217),
                new Point(-9.672668, -0.611519)
            };
            _pointsForPolyline2 = new List<Point>()
            {
                new Point(11.596278, -34.219641),
                new Point(7.089574, -18.840419),
                new Point(-6.763039, -25.173040),
                new Point(-3.041441, -5.001384)
            };
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
        private void ChangeZIndex()
        {
            int tmpZIndex = _polyline.ZIndex;
            _polyline.ZIndex = _polyline2.ZIndex;
            _polyline2.ZIndex = tmpZIndex;
        }
        [RelayCommand]
        private void ChangeGradient()
        {
            Random rnd = new Random();
            int first = rnd.Next(1, 5);
            int second = rnd.Next(6, 10);
            var gradientStops = new GradientStopCollection();
            gradientStops.Add(new GradientStop(_randomColors[first], 0));
            gradientStops.Add(new GradientStop(_randomColors[second], 1));
            _polyline2.Stroke = new LinearGradientBrush(gradientStops);
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
        private void SetupPolylines()
        {
#if IOS
            CapButtonsVisible = false;
#else
            CapButtonsVisible = true;
#endif
            _polyline.ZIndex = 0;
            _polyline2.ZIndex = 1;
            LineWidth = _polyline.StrokeThickness;
            _polyline.SetBinding(Polyline.StrokeThicknessProperty, new Binding(nameof(LineWidth), source: this, mode: BindingMode.TwoWay));
            _polyline2.SetBinding(Polyline.StrokeThicknessProperty, new Binding(nameof(LineWidth), source: this, mode: BindingMode.TwoWay));
            LineEnabled = _polyline.IsEnabled;
            _polyline.SetBinding(Polyline.IsEnabledProperty, new Binding(nameof(LineEnabled), source: this, mode: BindingMode.TwoWay));
            LineVisible = _polyline.IsVisible;
            _polyline.SetBinding(Polyline.IsVisibleProperty, new Binding(nameof(LineVisible), source: this, mode: BindingMode.TwoWay));

            foreach (var item in _pointsForPolyline)
            {
                _polyline.Points.Add(item);
            }
            foreach (var item in _pointsForPolyline2)
            {
                _polyline2.Points.Add(item);
            }
            _polyline2.Stroke = new SolidColorBrush(Colors.Green);
            Polylines.Add(_polyline);
            Polylines.Add(_polyline2);
        }
    }
}
