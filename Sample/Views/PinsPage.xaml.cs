using Sample.ViewModels;

namespace Sample.Views;

public partial class PinsPage
{
	public PinsPage()
	{
		BindingContext = new PinsPageViewModel();
		InitializeComponent();
	}
}
