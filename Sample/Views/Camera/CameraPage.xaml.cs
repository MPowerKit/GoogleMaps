using Sample.ViewModels;

namespace Sample.Views;

public partial class CameraPage
{
	public CameraPage(bool initialPosition)
	{
		this.BindingContext = new CameraPageViewModel(initialPosition);
		InitializeComponent();
	}
}