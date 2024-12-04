using Sample.ViewModels;

namespace Sample.Views;

public partial class PolylinePage : ContentPage
{
	public PolylinePage()
	{
		BindingContext = new PolylinePageViewModel();
		InitializeComponent();
	}
}