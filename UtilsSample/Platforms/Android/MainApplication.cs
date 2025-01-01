using Android.App;
using Android.Runtime;

namespace UtilsSample;

[Application]
[MetaData("com.google.android.geo.API_KEY", Value = "")]
public class MainApplication : MauiApplication
{
	public MainApplication(IntPtr handle, JniHandleOwnership ownership)
		: base(handle, ownership)
	{
	}

	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}