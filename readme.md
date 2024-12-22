# MPowerKit.GoogleMaps

#### Google Maps SDK library for .NET MAUI. Very easy to use. Allows to interact with map in MVVM manner through bindings or with map control directly.

[![Nuget](https://img.shields.io/nuget/v/MPowerKit.GoogleMaps)](https://www.nuget.org/packages/MPowerKit.GoogleMaps)

[!["Buy Me A Coffee"](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://www.buymeacoffee.com/alexdobrynin)

This library is designed for the .NET MAUI. The main control of this library `GoogleMap` implements every single feature that Google Maps SDK for Android and iOS provides. Every feature of the SDK is represented as `BindableProperty`, so you can build very flexible solutions that require usage of Google Maps.

## Setup

First of all you need to make sure you have set up [Google Cloud Console](https://developers.google.com/maps/get-started#create-project) and obtained Google Maps API key.

Then you need to set up your project to be able to use Google Maps SDK.
In your MainProgram.cs add next:

```csharp
builder
    .UseMauiApp<App>()
    .UseMPowerKitGoogleMaps(
#if IOS
        "Your iOS API key here"
#endif
    );
```

and in Android's MainApplication.cs:

```csharp
[Application]
[MetaData("com.google.android.maps.v2.API_KEY", Value = "Your Android API key here")]
public class MainApplication : MauiApplication
{
    public MainApplication(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
    { }
    
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
```

or in you Android's AndroidManifest.xml:

```xml
<application>
    <!-- you other settings -->
    <meta-data android:name="com.google.android.geo.API_KEY" android:value="Your Android API key here" />
</application>
```

Also, be sure you have added permissions to your Android's AndroidManifest.xml if you want to show 'My location' on the map:
```xml
<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
<uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />
<uses-feature android:name="android.hardware.location" android:required="false" />
<uses-feature android:name="android.hardware.location.gps" android:required="false" />
<uses-feature android:name="android.hardware.location.network" android:required="false" />
```

## Usage

To use this library in you MAUI projects just add `GoogleMap` control you your xaml file as next:

```xaml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="Sample.Views.MainPage"
             xmlns:gm="clr-namespace:MPowerKit.GoogleMaps;assembly=MPowerKit.GoogleMaps">
    <gm:GoogleMap />
</ContentPage>
```

The full list of all properties and features you can find [here](https://github.com/MPowerKit/GoogleMaps/blob/f976b19aab0175c590910ad3b68fc9bcccce00d0/MPowerKit.GoogleMaps/GoogleMap.cs#L14). Also every single feature is shown in Sample project, so it is better to download it and there you can find how to use feature you need.

## Map objects

There 6 types of objects that can be added to the map: `Pin`, `Circle`, `Polyline`, `Polygon`, `TileOverlay`, `GroundOverlay`. To simplify implementation of the library for polylines and polygons were used classes from `Microsoft.Maui.Controls.Shapes` namespace. `Circle` is derived from `Shape` class. `Pin`, `TileOverlay`, `GroundOverlay` are derived from `VisualElement` class.
