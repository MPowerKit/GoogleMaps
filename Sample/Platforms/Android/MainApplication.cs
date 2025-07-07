using Android.App;
using Android.Runtime;

namespace Sample;

[Application]
[MetaData("com.google.android.geo.API_KEY", Value = "AIzaSyBCsqPLN1qKZSO6Q6qlWKdfXiFsSpWmZzQ")]
public class MainApplication : MauiApplication
{
    public MainApplication(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}