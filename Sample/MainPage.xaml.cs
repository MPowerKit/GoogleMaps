using Sample.Views;

namespace Sample;

public partial class MainPage
{
    public MainPage()
    {
        InitializeComponent();

        //Setup();
    }

    //private async void Setup()
    //{
    //    var polylines = new ObservableCollection<Polyline>
    //    {
    //        new()
    //        {
    //            StrokeThickness = 5,
    //            Stroke = new SolidColorBrush(Colors.Red),
    //            Points = new PointCollection() { new Point(0, 0), new Point(0, 20), new Point(20, 0) }
    //        }
    //    };
    //    gmap.Polylines = polylines;

    //    var polygons = new ObservableCollection<Polygon>
    //    {
    //        new()
    //        {
    //            StrokeThickness = 10,
    //            Fill = new SolidColorBrush(Colors.Purple.WithAlpha(0.3f)),
    //            Stroke = new SolidColorBrush(Colors.Orange),
    //            Points = new PointCollection() { new Point(0, 0), new Point(0, 30), new Point(30, 0), new Point(0, 0) }
    //        }
    //    };
    //    gmap.Polygons = polygons;

    //    var pins = new ObservableCollection<Pin>
    //    {
    //        new() { Position = new(50,50), Icon = "map_pin.png", Title = "sdhsdhsdh", Snippet = "sdhsdgf", Draggable = true }
    //    };
    //    gmap.MapStyleJson = "map_style.json";
    //    gmap.Pins = pins;

    //    var circles = new ObservableCollection<Circle>
    //    {
    //        new()
    //        {
    //            Center = new(50,40),
    //            StrokeThickness = 5,
    //            Stroke = Colors.Blue,
    //            Fill = Colors.Green.WithAlpha(0.3f),
    //            Radius = Distance.FromKMeters(500),
    //        }
    //    };

    //    gmap.Circles = circles;

    //    //gmap.NativeMapReady += Gmap_NativeMapReady;

    //    //await Task.Delay(5000);

    //    //var line = circles[0];
    //    //line.StrokeThickness = 10;
    //    //line.Stroke = Colors.Blue;

    //    //await Task.Delay(5000);

    //    //gmap.Circles = new ObservableCollection<Circle>
    //    //{
    //    //    new()
    //    //    {
    //    //        StrokeThickness = 5,
    //    //        Stroke = Colors.Purple.WithAlpha(0.5f),
    //    //        Fill = Colors.Purple,
    //    //        Radius = Distance.FromKMeters(1000),
    //    //    }
    //    //};
    //}

    //private void Gmap_NativeMapReady()
    //{
    //    gmap.MoveCamera(CameraUpdateFactory.FromCenterAndRadius(new(50, 50), Distance.FromKMeters(500)));
    //}

    private void Button_Clicked(System.Object sender, System.EventArgs e)
    {
        this.Navigation.PushAsync(new NavigationPage(new PinsTabbedPage()));
    }

    private void Button_Clicked_1(System.Object sender, System.EventArgs e)
    {
        this.Navigation.PushAsync(new NavigationPage(new CirclesTabbedPage()));
    }
}