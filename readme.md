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

## API definition

**!!! Important: If you cannot find documentation about some methods, event or properties here, then you can search some at offical Google Maps SDK web site.**

API of this library is devided into 9 logical groups: Map features, Camera, UI Settings, Pins, Circles, Polylines, Polygons, Tiles, Ground overlays. Each logic group is represented as separate manager and responsible only for that part of logic, and preregistered in the public static dictionary inside the handler:

```csharp
public static Dictionary<string, Func<IMapFeatureManager<GoogleMap, NativeMapView, GoogleMapHandler>>> ManagerMapper = new()
{
    { nameof(MapManager), () => new MapManager() },
    { nameof(CameraManager), () => new CameraManager() },
    { nameof(UiSettingsManager), () => new UiSettingsManager() },
    { nameof(PolylineManager), () => new PolylineManager() },
    { nameof(PolygonManager), () => new PolygonManager() },
    { nameof(CircleManager), () => new CircleManager() },
    { nameof(TileOverlayManager), () => new TileOverlayManager() },
    { nameof(GroundOverlayManager), () => new GroundOverlayManager() },
    { nameof(PinManager), () => new PinManager() },
};
```

**Note: `NativeMapView` is different for Android and iOS**

This approach is very flexible and gives you possibility to operate with different logics in a very simple way, and it is very easy understand, add, change or remove already registered implementations by your's.
For example: if you want to remove some logic, you just need to remove registration from `ManagerMapper` dictionary, as next:

```csharp
GoogleMapHandler.ManagerMapper.Remove(nameof(GroundOverlayManager));
```

If you want to change implementation, you can do next:

```csharp
GoogleMapHandler.ManagerMapper[nameof(Pins)] = () => new YourPinManager();
```

If you want to add absolutely new logic to your map, you can do next:

```csharp
GoogleMapHandler.ManagerMapper["YourManagerUniqueName"] = () => new YourNewLogicManager();
```
and your `YourNewLogicManager` should be `typeof(IMapFeatureManager<GoogleMap, NativeMapView, GoogleMapHandler>)`

**Note: You can do it in your `MauiProgram.cs` or in any other place in your code. But you should understand, that this changes can be applied to the map only before navigating to the page with map.**

### Events and commands

|Event|Command|Argument type|Comment|
|-|-|-|-|
|NativeMapReady|NativeMapReadyCommand| |Raised, when native map is initialized and rendered and ready to go. Raises only once. All operations with map should be done only after this event. `IsNativeMapReady` property is set after this event to `true` value|
|MapCapabilitiesChanged|MapCapabilitiesChangedCommand|MapCapabilities|Raised, when map capabilities were changed, the `MapCapabilities` property is set to the new value.|
|IndoorBuildingFocused|IndoorBuildingFocusedCommand|IndoorBuilding?|Raised, when indoor building was focused, the `IndoorBuilding` property is set to the new value. Can be null if no building is focused|
|IndoorLevelActivated|IndoorLevelActivatedCommand|IndoorLevel?|Raised, when indoor building was focused or active level was changed, the `ActiveLevel` property is set to the new value. Can have value only if `IndoorBuilding` is not null, otherwise null|
|MapClick|MapClickedCommand|Point|Raised, when user clicks on the map.|
|MapLongClick|MapLongClickedCommand|Point|Raised, when user does long press on the map.|
|PoiClick|PoiClickedCommand|PointOfInterest|Raised, when user on the 'Point of Interest'.|
|CameraMoveStart|CameraMoveStartedCommand|CameraMoveReason|Raised, when camera starting change of it's position. `CameraMoveReason` is enum and representing the reason of camera move|
|CameraMove|CameraMoveCommand| |Raised, when camera is moving. Can be risen only when camera is animated programmatically|
|CameraPositionChanged|CameraPositionChangedCommand|CameraPosition|Raised, when camera is changing it's position, the `CameraPosition` property is set to the new value|
|CameraMoveCanceled|CameraMoveCanceledCommand| |Raised, when camera move is canceled. Can be risen only when camera is animated programmatically and animation was canceled. Applies only to Android|
|CameraIdle|CameraIdleCommand|VisibleRegion|Raised, when camera is idle (stopped). the `CameraPosition` property is set to the new value, the `VisibleRegion` property is set to the new value|
|PolygonClick|PolygonClickedCommand|Polygon/object|Raised, when user clicks on the polygon. The argument type of the command depends on how polygon was added to the map.|
|PolylineClick|PolylineClickedCommand|Polyline/object|Raised, when user clicks on the polyline. The argument type of the command depends on how polyline was added to the map.|
|CircleClick|CircleClickedCommand|Circle/object|Raised, when user clicks on the circle. The argument type of the command depends on how circle was added to the map.|
|GroundOverlayClick|GroundOverlayClickedCommand|GroundOverlay/object|Raised, when user clicks on the ground overlay. The argument type of the command depends on how ground overlay was added to the map.|
|PinClick|PinClickedCommand|Pin/object|Raised, when user clicks on the pin. The argument type of the command depends on how pin was added to the map. the `SelectedPin` property is set to the new value, but only if this pin `CanBeSelected`|
|PinDragStart|PinDragStartedCommand|Pin/object|Raised, when user starting drag the pin. The argument type of the command depends on how pin was added to the map.|
|PinDragging|PinDraggingCommand|Pin/object|Raised, when user is dragging the pin. The argument type of the command depends on how pin was added to the map.|
|PinDragEnd|PinDragEndedCommand|Pin/object|Raised, when pin dragging is ended. The argument type of the command depends on how pin was added to the map.|
|InfoWindowClick|InfoWindowClickedCommand|Pin/object|Raised, when user clicks on pin's info window. The argument type of the command depends on how pin was added to the map.|
|InfoWindowLongClick|InfoWindowLongClickedCommand|Pin/object|Raised, when user does long press on pin's info window. The argument type of the command depends on how pin was added to the map.|
|InfoWindowClosed|InfoWindowClosedCommand|Pin/object|Raised, when pin's info window was closed. The argument type of the command depends on how pin was added to the map.|

### Public methods, actions, funcs

|Method|Bindable property|Arguments types|Return type|Comment|
|-|-|-|-|-|
|TakeSnapshot|TakeSnapshotFunc| |Task&lt;Stream&gt;|Takes snapshot of the map in current state. Returns stream of the taken snapshot.|
|ResetMinMaxZoom|ResetMinMaxZoomAction| | |Resets min and max zoom properties. Applies only to Android.|
|MoveCamera|MoveCameraAction|CameraPosition| |Instantly moves camera to the new position.|
|AnimateCamera|AnimateCameraFunc|CameraPosition, int|Task|Moves camera to the new position with animation. By default animation duration is 300 ms, but can be changed.|
|ProjectMapCoordsToScreenLocation|ProjectMapCoordsToScreenLocationFunc|Point|Point?|Projects coordinates on map to the coordinates on the screen.|
|ProjectScreenLocationToMapCoords|ProjectScreenLocationToMapCoordsFunc|Point|Point?|Projects coordinates on screen to the coordinates on the map.|

### Bindable properties

There are some readonly properties, which are not visible in XAML editor, but you can bind them. One condition, they can be bound only with `Mode=OneWayToSource`, otherwise it will not work. This means that value can be passed only from map control to the viewmodel.
Example of usage:
```xaml
<gm:GoogleMap MapCapabilities="{Binding MapCapabilities, Mode=OneWayToSource}" />
```

|Readonly property|Property type|Comment|
|-|-|-|
|MapCapabilities|MapCapabilities?|Allows to track the availability of each map capability.|
|FocusedBuilding|IndoorBuilding|Represents a currently focused building by camera position. Can be null if there is no building focused at the moment.|
|ActiveLevel|IndoorLevel|Represents an active level of a currently focused building by camera position. Can be null if there is no building focused at the moment.|
|IsNativeMapReady|bool|Indicates whether native map is initialized, rendered and ready to use.|
|CameraPosition|CameraPosition|Represents current position of the camera.|
|ResetMinMaxZoomAction|Action|Can be used as action to reset min and max zoom properties.|

## Map objects

There 6 types of objects that can be added to the map: `Pin`, `Circle`, `Polyline`, `Polygon`, `TileOverlay`, `GroundOverlay`. To simplify implementation of the library for polylines and polygons were used classes from `Microsoft.Maui.Controls.Shapes` namespace. `Circle` is derived from `Shape` class. `Pin`, `TileOverlay`, `GroundOverlay` are derived from `VisualElement` class.
