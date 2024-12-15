using Sample.Views;

namespace Sample;

public partial class MainPage
{
    public MainPage()
    {
        InitializeComponent();
    }

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

    private void Button_Clicked_2(System.Object sender, System.EventArgs e)
    {
        this.Navigation.PushAsync(new NavigationPage(new PolygonsTabbedPage()));
    }

    private void Button_Clicked_3(object sender, EventArgs e)
    {
        this.Navigation.PushAsync(new NavigationPage(new PolylinesTabbedPage()));
    }

    private void Button_Clicked_4(object sender, EventArgs e)
    {
        this.Navigation.PushAsync(new NavigationPage(new UISettingsPage()));
    }

    private void Button_Clicked_5(object sender, EventArgs e)
    {
        this.Navigation.PushAsync(new NavigationPage(new MapFeaturesPage()));
    }

    private void Button_Clicked_6(System.Object sender, System.EventArgs e)
    {
        this.Navigation.PushAsync(new NavigationPage(new CameraPage(false)));
    }

    private void Button_Clicked_7(System.Object sender, System.EventArgs e)
    {
        this.Navigation.PushAsync(new NavigationPage(new CameraPage(true)));
    }

    private void Button_Clicked_8(System.Object sender, System.EventArgs e)
    {
        this.Navigation.PushAsync(new NavigationPage(new TileOverlaysTabbedPage()));
    }
}