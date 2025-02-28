﻿using Sample.Views;

namespace Sample;

public partial class MainPage
{
    public MainPage()
    {
        InitializeComponent();
    }

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

    private void Button_Clicked_9(System.Object sender, System.EventArgs e)
    {
        this.Navigation.PushAsync(new NavigationPage(new GroundOverlaysTabbedPage()));
    }

    private void Button_Clicked_10(System.Object sender, System.EventArgs e)
    {
        this.Navigation.PushAsync(new NavigationPage(new HeatMapTabbedPage()));
    }

    private void Button_Clicked_11(System.Object sender, System.EventArgs e)
    {
        this.Navigation.PushAsync(new NavigationPage(new ClustersTabbedPage()));
    }

    private void Button_Clicked_12(System.Object sender, System.EventArgs e)
    {
        this.Navigation.PushAsync(new NavigationPage(new KmlPage()));
    }

    private void Button_Clicked_13(System.Object sender, System.EventArgs e)
    {
        this.Navigation.PushAsync(new NavigationPage(new GeoJsonPage()));
    }
}