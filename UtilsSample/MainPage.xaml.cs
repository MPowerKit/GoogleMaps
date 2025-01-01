using UtilsSample.Views;

namespace UtilsSample;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private void Button_Clicked(System.Object sender, System.EventArgs e)
    {
        this.Navigation.PushAsync(new NavigationPage(new HeatMapPage()));
    }
}