using Android.App;
using Android.Gms.Common;

namespace MPowerKit.GoogleMaps;

public static class MapsInitializer
{
    public static bool IsInitialized { get; private set; }

    public static void Init(Activity activity)
    {
        if (IsInitialized) return;

        try
        {
            var resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(activity);
            if (resultCode == ConnectionResult.Success)
            {
                resultCode = Android.Gms.Maps.MapsInitializer.Initialize(activity);
            }

            if (resultCode != ConnectionResult.Success)
            {
                throw new Exception(GoogleApiAvailability.Instance.GetErrorString(resultCode));
            }
            else IsInitialized = true;
        }
        catch (Exception e)
        {
            Console.WriteLine("Google Play Services Not Found");
            Console.WriteLine("Exception: {0}", e.Message);
        }
    }
}