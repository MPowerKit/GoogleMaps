# MPowerKit.GoogleMaps

#### Google Maps SDK library for .NET MAUI. Very easy to use. Allows to interact with map in MVVM manner through bindings or with map control directly.

[![Nuget](https://img.shields.io/nuget/v/MPowerKit.GoogleMaps)](https://www.nuget.org/packages/MPowerKit.GoogleMaps)

[!["Buy Me A Coffee"](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://www.buymeacoffee.com/alexdobrynin)

This library is designed for the .NET MAUI. The main control of this library `GoogleMap` implements every single feature that Google Maps SDK for Android and iOS provides. Every feature of the SDK is represented as `BindableProperty`, so you can build very flexible solutions that require usage of Google Maps.

## Table Of Contents

- [Setup](#setup)
- [Usage](#usage)
- [API definition](#api-definition)
    - [Events and commands](#events-and-commands)
    - [Public methods, actions, funcs](#public-methods-actions-funcs)
    - [Bindable properties](#bindable-properties)
        - [Read only properties](#read-only-properties)
        - [Properties related to objects on the map](#properties-related-to-objects-on-the-map)
        - [Other properties](#other-properties)
    - [Models](#models)
        - [Point](#point)
        - [CameraMoveReason](#cameramovereason)
        - [CameraPosition](#cameraposition)

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
    <!-- your other settings -->
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

**!!! Important: If you cannot find documentation about some methods, events or properties here, then you can search some at offical Google Maps SDK web site.**

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

|Event|Command|Argument type|Description|
|-|-|-|-|
|NativeMapReady|NativeMapReadyCommand| |Raised, when native map is initialized and rendered and ready to go. Raises only once. All operations with map should be done only after this event. `IsNativeMapReady` property is set after this event to `true`.|
|MapCapabilitiesChanged|MapCapabilitiesChangedCommand|MapCapabilities|Raised, when map capabilities were changed, the `MapCapabilities` property is set to the new value.|
|IndoorBuildingFocused|IndoorBuildingFocusedCommand|IndoorBuilding?|Raised, when indoor building was focused, the `IndoorBuilding` property is set to the new value. Can be `null` if no building is focused.|
|IndoorLevelActivated|IndoorLevelActivatedCommand|IndoorLevel?|Raised, when indoor building was focused or active level was changed, the `ActiveLevel` property is set to the new value. Can have value only if `IndoorBuilding` is not `null`, otherwise `null`.|
|MapClick|MapClickedCommand|Point|Raised, when user clicks on the map.|
|MapLongClick|MapLongClickedCommand|Point|Raised, when user does long press on the map.|
|PoiClick|PoiClickedCommand|PointOfInterest|Raised, when user on the 'Point of Interest'.|
|CameraMoveStart|CameraMoveStartedCommand|CameraMoveReason|Raised, when camera starting change of it's position. `CameraMoveReason` is enum and representing the reason of camera move.|
|CameraMove|CameraMoveCommand| |Raised, when camera is moving. Can be risen only when camera is animated programmatically.|
|CameraPositionChanged|CameraPositionChangedCommand|CameraPosition|Raised, when camera is changing it's position, the `CameraPosition` property is set to the new value.|
|CameraMoveCanceled|CameraMoveCanceledCommand| |Raised, when camera move is canceled. Can be risen only when camera is animated programmatically and animation was canceled. Applies only to Android.|
|CameraIdle|CameraIdleCommand|VisibleRegion|Raised, when camera is idle (stopped). the `CameraPosition` property is set to the new value, the `VisibleRegion` property is set to the new value.|
|PolygonClick|PolygonClickedCommand|Polygon/object|Raised, when user clicks on the polygon. The argument type of the command depends on how polygon was added to the map.|
|PolylineClick|PolylineClickedCommand|Polyline/object|Raised, when user clicks on the polyline. The argument type of the command depends on how polyline was added to the map.|
|CircleClick|CircleClickedCommand|Circle/object|Raised, when user clicks on the circle. The argument type of the command depends on how circle was added to the map.|
|GroundOverlayClick|GroundOverlayClickedCommand|GroundOverlay/object|Raised, when user clicks on the ground overlay. The argument type of the command depends on how ground overlay was added to the map.|
|PinClick|PinClickedCommand|Pin/object|Raised, when user clicks on the pin. The argument type of the command depends on how pin was added to the map. the `SelectedPin` property is set to the new value, but only if this pin `CanBeSelected`.|
|PinDragStart|PinDragStartedCommand|Pin/object|Raised, when user starting drag the pin. The argument type of the command depends on how pin was added to the map.|
|PinDragging|PinDraggingCommand|Pin/object|Raised, when user is dragging the pin. The argument type of the command depends on how pin was added to the map.|
|PinDragEnd|PinDragEndedCommand|Pin/object|Raised, when pin dragging is ended. The argument type of the command depends on how pin was added to the map.|
|InfoWindowClick|InfoWindowClickedCommand|Pin/object|Raised, when user clicks on pin's info window. The argument type of the command depends on how pin was added to the map.|
|InfoWindowLongClick|InfoWindowLongClickedCommand|Pin/object|Raised, when user does long press on pin's info window. The argument type of the command depends on how pin was added to the map.|
|InfoWindowClosed|InfoWindowClosedCommand|Pin/object|Raised, when pin's info window was closed. The argument type of the command depends on how pin was added to the map.|

### Public methods, actions, funcs

|Method|Bindable property|Parameters|Return type|Description|
|-|-|-|-|-|
|TakeSnapshot|TakeSnapshotFunc| |Task&lt;Stream?&gt;|Takes snapshot of the map in current state. Returns stream of the taken snapshot.|
|ResetMinMaxZoom|ResetMinMaxZoomAction| | |Resets min and max zoom properties. Applies only to Android.|
|MoveCamera|MoveCameraAction|CameraUpdate newCameraPosition| |Instantly moves camera to the new position.|
|AnimateCamera|AnimateCameraFunc|CameraUpdate newCameraPosition, int durationMils|Task|Moves camera to the new position with animation. By default animation duration is 300 ms, but can be changed.|
|ProjectMapCoordsToScreenLocation|ProjectMapCoordsToScreenLocationFunc|Point latlng|Point?|Projects map coordinates to the coordinates on the screen within map control.|
|ProjectScreenLocationToMapCoords|ProjectScreenLocationToMapCoordsFunc|Point screenPoint|Point?|Projects coordinates on the screen to map coordinates.|

### Bindable properties

#### Read only properties

There are some readonly properties, which are not visible in XAML editor, but you can bind them. One condition, they can be bound only with `Mode=OneWayToSource`, otherwise it will not work. This means that value can be passed only from map control to the viewmodel.
Example of usage:
```xaml
<gm:GoogleMap MapCapabilities="{Binding MapCapabilities, Mode=OneWayToSource}" />
```

|Readonly bindable property|Property type|Description|
|-|-|-|
|IsNativeMapReady|bool|Indicates whether native map is initialized, rendered and ready to use. Otherwise `false`.|
|MapCapabilities|MapCapabilities|Allows to track the availability of each map capability. Always has value after `IsNativeMapReady` set to `true`.|
|FocusedBuilding|IndoorBuilding|Represents a currently focused building by camera position. Can be `null` if there is no building focused at the moment.|
|ActiveLevel|IndoorLevel|Represents an active level of a currently focused building by camera position. Can be `null` if there is no building focused at the moment.|
|CameraPosition|CameraPosition|Represents current position of the camera and current zoom level. Never `null` after `IsNativeMapReady` set to `true`.|
|VisibleRegion|VisibleRegion|Represents the visible map region the last time camera was idle. Always has value after `IsNativeMapReady` set to `true`.|
|ResetMinMaxZoomAction|Action|Can be bound and called from viewmodel to reset min and max zoom properties. Never `null`.|
|TakeSnapshotFunc|Func&lt;Task&lt;Stream?&gt;&gt;|Can be bound and called from viewmodel to take snapshots of the current map state. Never `null`.|
|AnimateCameraFunc|Func&lt;CameraUpdate, int, Task&gt;|Can be bound and called from viewmodel to move camera to the new position with animation. By default animation duration is 300 ms, but can be changed. Never `null`.|
|MoveCameraAction|Action&lt;CameraUpdate, int&gt;|Can be bound and called from viewmodel to instantly move camera to the new position. Never `null`.|
|ProjectMapCoordsToScreenLocationFunc|Func&lt;Point,Point?&gt;|Can be bound and called from viewmodel to project map coordinates to coordinates on the screen whithin map control. Never `null`.|
|ProjectScreenLocationToMapCoordsFunc|Func&lt;Point,Point?&gt;|Can be bound and called from viewmodel to project coordinates on the screen to map coordinates. Never `null`.|

#### Properties related to objects on the map

|Bindable property|Property type|Description|
|-|-|-|
|Polylines|IEnumerable&lt;Polyline&gt;|Represents collection of the polylines. Supports `INotifyCollectionChanged` collection types, such as `ObservableCollection<Polyline>`.|
|PolylinesSource|IEnumerable|Analogue to the `ItemsSource` of the `CollectionView`. Supports `INotifyCollectionChanged` collection types, such as `ObservableCollection<AnyObject>`. This is a `BindingContext` source to the `Polylines` property. Works in pair with `PolylineItemTemplate`, otherwise ignored. When this proeprty is set, the `Polylines` property will be replaced by new collection of polylines created from `PolylineItemTemplate`. The `BindingContext` of each polyline will be represented as each item of this collection.|
|PolylineItemTemplate|DataTemplate|Analogue to the `ItemTemplate` of the `CollectionView`. Supports `DataTemplateSelector`. Works only in pair with `PolylinesSource` property, otherwise ignored. The root of this template should be `typeof(Polyline)`, otherwise will crash.|
|Polygons|IEnumerable&lt;Polygon&gt;|Same as `Polylines`, but designed for polygons.|
|PolygonsSource|IEnumerable|Same as `PolylinesSource`, but designed for polygons.|
|PolygonItemTemplate|DataTemplate|Same as `PolylineItemTemplate`, but designed for polygons.|
|Circles|IEnumerable&lt;Circle&gt;|Same as `Polylines`, but designed for circles.|
|CirclesSource|IEnumerable|Same as `PolylinesSource`, but designed for circles.|
|CircleItemTemplate|DataTemplate|Same as `PolylineItemTemplate`, but designed for circles.|
|TileOverlays|IEnumerable&lt;TileOverlay&gt;|Same as `Polylines`, but designed for tiles.|
|TileOverlaysSource|IEnumerable|Same as `PolylinesSource`, but designed for tiles.|
|TileOverlayItemTemplate|DataTemplate|Same as `PolylineItemTemplate`, but designed for tiles.|
|GroundOverlays|IEnumerable&lt;GroundOverlay&gt;|Same as `Polylines`, but designed for ground overlays.|
|GroundOverlaysSource|IEnumerable|Same as `PolylinesSource`, but designed for ground overlays.|
|GroundOverlayItemTemplate|DataTemplate|Same as `PolylineItemTemplate`, but designed for ground overlays.|
|Pins|IEnumerable&lt;Pin&gt;|Same as `Polylines`, but designed for pins.|
|PinsSource|IEnumerable|Same as `PolylinesSource`, but designed for pins.|
|PinItemTemplate|DataTemplate|Same as `PolylineItemTemplate`, but designed for pins.|
|SelectedPin|Pin|Represents selected pin at the moment. `null` if there is no selected pin at the moment.|
|SelectedPinData|object|This is a `BindingContext` of currently selected pin. `null` if there is no selected pin at the moment.|
|InfoWindowTemplate|DataTemplate|Setting this property will instruct map to show desired `DataTemplate` as 'Info Window' of the pin. Supports `DataTemplateSelector`. Can be shown only when `Pin`'s `CanBeSelected` and `ShowInfoWindowOnPinSelection` both are `true`. The `BindingContext` of this template will be the `BindingContext` of the pin, or if pin does not have `BindingContext` set, this will be the pin itself. When `null`, the default SDK's 'Info Window' will be shown. By default is `null`.|

#### Other properties

|Bindable property|Property type|Description|
|-|-|-|
|InitialCameraPosition|CameraUpdate|The initial position of the camera at the moment of map rendering. By default is `null`. Value of this property will be taken into account only before `IsNativeMapReady` property is set to `true`, otherwise it will be ignored.|
|RestrictPanningToArea|LatLngBounds?|Setting this property to a non-`null` value will restrict user to a pan only in the bounds set. Settings this property to a `null` will remove pan restriction. By default is `null`.|
|IndoorEnabled|bool|Enables indoor buildings to be able to be focuses. Works only with `MapType=Normal`. By default is `false`.|
|BuildingsEnabled|bool|Enables 3D buildings on the map. Works only with `MapType=Normal`. By default is `false`.|
|TrafficEnabled|bool|Enables traffic. By default is `false`.|
|MyLocationEnabled|bool|Enables 'My location' to be shown on the map. By default is `false`. Before setting this property to `true` you must be sure the location permissions to are granted.|
|MapType|MapType|Represents the type of the map. By default is `Normal`.|
|MapColorScheme|MapColorScheme|Represents the map color theme. By default is `FollowSystem`. Applies only for Android.|
|Padding|Thickness|Represents the edge insents of the map. By default is `default(Thickness)` or 0.|
|MinZoom|float|Represents the minimum zoom the camera can take. By default is 2. For different `MapType` SDK may ignore minimum zoom settings.|
|MaxZoom|float|Represents the maximum zoom the camera can take. By default is 21. For different `MapType` SDK may ignore maximum zoom settings.|
|MapStyleJson|string|Setting this property you can change visual styling of the map. It can take url, file with `.json` extension from assets, or json code directly. More about styling you can read [Android](https://developers.google.com/maps/documentation/android-sdk/styling) and [iOS](https://developers.google.com/maps/documentation/ios-sdk/styling). For iOS this is the only way to change map color scheme. By default is `null`.|
|HandlePoiClick|bool|Indicates whether the map control should handle clicks on 'Point of Interest'. By default is `false`.|
|MyLocationButtonEnabled|bool|Indicates whether the map control should show 'My location' button, this will work only if `MyLocationEnabled` is `true`. By default is `true`.|
|IndoorLevelPickerEnabled|bool|Indicates whether the map control should show indoor level picker buttons, this will work only if `IndoorEnabled` is `true`. By default is `true`.|
|CompassEnabled|bool|Indicates whether the map control should show compass button. By default is `true`.|
|MapToolbarEnabled|bool|Indicates whether the map control should show toolbar buttons. By default is `true`. Applies only for Android.|
|ZoomControlsEnabled|bool|Indicates whether the map control should show zoom +- buttons. By default is `true`. Applies only for Android.|
|ZoomGesturesEnabled|bool|Indicates whether zoom gestures are enabled. By default is `true`.|
|ScrollGesturesEnabled|bool|Indicates whether pan (scroll) gestures are enabled. By default is `true`.|
|TiltGesturesEnabled|bool|Indicates whether tilt (two fingers swipe) gestures are enabled. By default is `true`.|
|RotateGesturesEnabled|bool|Indicates whether rotate (two fingers) gestures are enabled. By default is `true`.|

### Models

#### Point

This structure is taken from `Microsoft.Maui.Graphics` namespace just to simplify the framework and not to 'invent the bicycle'. Basically it is used to represent coordinates on the map, where `X` - Latitude, `Y` - Longitude. But in some cases `Point` represents just point on the screen where `X` - x coordinate, `Y` - y coordinate. It depends on the context of usage of this structure.

#### LatLngBounds

A structure that represents a latitude/longitude aligned rectangle.

|Property|Type|Description|
|-|-|-|
|SouthWest|Point|Southwest corner of the bound.|
|NorthEast|Point|Northeast corner of the bound.|

#### VisibleRegion

`VisibleRegion` is a struct. Contains the four points defining the four-sided polygon that is visible in a map's camera. This polygon can be a trapezoid instead of a rectangle, because a camera can have tilt. If the camera is directly over the center of the camera, the shape is rectangular, but if the camera is tilted, the shape will appear to be a trapezoid whose smallest side is closest to the point of view.

|Property|Type|Description|
|-|-|-|
|Bounds|LatLngBounds|The smallest bounding box that includes the visible region defined in this class.|
|FarLeft|Point|Point on the map that defines the far left corner of the camera.|
|FarRight|Point|Point on the map that defines the far right corner of the camera.|
|NearLeft|Point|Point on the map that defines the bottom left corner of the camera.|
|NearRight|Point|Point on the map that defines the bottom right corner of the camera.|

#### MapCapabilities

`MapCapabilities` is a struct that allows customers to track the availability of each capability.

|Property|Type|Description|
|-|-|-|
|AreAdvancedMarkersAvailable|bool|`true` if advanced markers are available.|
|IsDataDrivenStylingAvailable|bool|`true` if data-driven styling is available.|

#### PointOfInterest

`PointOfInterest` is a struct that contains information about a 'Point of Interest' that was clicked on.

|Property|Type|Description|
|-|-|-|
|Position|Point|The position of the POI.|
|PlaceId|string|The placeId of the POI.|
|Name|string|The name of the POI.|

#### CameraPosition

An immutable class that aggregates all camera position parameters such as location, zoom level, tilt angle, and bearing. Use `CameraPosition.Builder` to construct a `CameraPosition` instance, which you can then use in conjunction with `CameraUpdateFactory`.

|Property|Type|Description|
|-|-|-|
|Target|Point|The location on the map that the camera is pointing at.|
|Zoom|float|Zoom level near the center of the screen.|
|Bearing|float|Direction that the camera is pointing in, in degrees clockwise from north.|
|Tilt|float|The angle, in degrees, of the camera angle from the nadir (directly facing the Earth).|

#### CameraUpdate

An abstract class that is used to update camera position. Can be created by different factory methods of `CameraUpdateFactory`.

#### CameraUpdateFactory

A class containing methods for creating `CameraUpdate` objects that change a map's camera. Each method returns `CameraUpdate` object.

|Method|Parameters|Description|
|-|-|-|
|ZoomIn| |Returns a `CameraUpdate` that zooms in on the map by moving the viewpoint's height closer to the Earth's surface.|
|ZoomOut| |Returns a `CameraUpdate` that zooms out on the map by moving the viewpoint's height farther away from the Earth's surface.|
|ZoomTo|float zoom|Returns a `CameraUpdate` that moves the camera viewpoint to a particular zoom level.|
|ZoomBy|float zoomDelta|Returns a `CameraUpdate` that shifts the zoom level of the current camera viewpoint.|
|ZoomBy|float zoomDelta, Point focusPointOnScreen|Returns a `CameraUpdate` that shifts the zoom level of the current camera viewpoint, focusing at the spcified point on the screen.|
|ScrollBy|float dxPixels, float dyPixels|Returns a `CameraUpdate` that scrolls the camera over the map, shifting the center of view by the specified number of pixels in the x and y directions.|
|NewCameraPosition|CameraPosition cameraPosition|Returns a `CameraUpdate` that moves the camera to a specified `CameraPosition`.|
|NewLatLng|Point latLng|Returns a `CameraUpdate` that moves the center of the screen to a latitude and longitude specified by a point on the map.|
|NewLatLngZoom|Point latLng, float zoom|Returns a `CameraUpdate` that moves the center of the screen to a latitude and longitude specified by a point on the map, and moves to the given zoom level.|
|NewLatLngBounds|LatLngBounds bounds, double padding|Returns a `CameraUpdate` that transforms the camera such that the specified latitude/longitude bounds are centered on screen at the greatest possible zoom level.|
|NewLatLngBounds|LatLngBounds bounds, double padding, Size size|Returns a `CameraUpdate` that transforms the camera such that the specified latitude/longitude bounds are centered on screen within a bounding box of specified dimensions at the greatest possible zoom level. Applies only for Android.|
|FromCenterAndRadius|Point center, double radiusMeters|Returns a `CameraUpdate` that transforms the camera such that the specified latitude/longitude point are centered on screen within a bounding box of specified radius.|

#### IndoorBuilding

`IndoorBuilding` is a record. Represents a building on the map that can be focused.

|Property|Type|Description|
|-|-|-|
|DefaultLevelIndex|int|The index in the `Levels` list of the default level for this building.|
|IsUnderground|bool|`true` if the building is entirely underground.|
|Levels|IList&lt;IndoorLevel&gt;|Represents levels in the building.|

#### IndoorLevel

`IndoorLevel` is a record. Represents a level in a building.

|Property|Type|Description|
|-|-|-|
|Name|string|Localized display name for the level, e.g.|
|ShortName|string|Localized short display name for the level, e.g.|

#### CameraMoveReason

`CameraMoveReason` is an enum. It indicates the reason of camera movement.

|Value|Description|
|-|-|
|Gesture|Camera motion initiated in response to user gestures on the map.|
|ApiAnimation|Non-gesture animation initiated in response to user actions.|
|DeveloperAnimation|Developer initiated animation. Applies only for Android.|

#### MapType

`MapType` is an enum. Indicates what type of map tiles that should be displayed.

|Value|Description|
|-|-|
|None|No base map tiles.|
|Normal|Basic map.|
|Satellite|Satellite imagery.|
|Terrain|Topographic data.|
|Hybrid|Satellite imagery with roads and labels.|

#### MapColorScheme

`MapColorScheme` is an enum. Indicates what color mode should be used to show the map. Applies only for Android.

|Value|Description|
|-|-|
|Light|Represents light mode.|
|Dark|Represents dark mode.|
|FolllowSystem|Represents color mode used by system.|

#### Distance

`Distance` is an utility static class. It provides utility methods to calculate distance between coordinates or converting other units to meters, and has other useful methods and constants. All methods that operate with distance return distance in meters.

### Map objects

There 6 types of objects that can be added to the map: `Pin`, `Circle`, `Polyline`, `Polygon`, `TileOverlay`, `GroundOverlay`. To simplify implementation of the library for polylines and polygons were used already existing classes from `Microsoft.Maui.Controls.Shapes` namespace. `Circle` is derived from `Shape` class. `Pin`, `TileOverlay`, `GroundOverlay` are derived from `VisualElement` class.

#### Pin

`Pin` is a class inherited from `VisualElement`. It is an icon placed at a particular point on the map's surface. A pin icon is drawn oriented against the device's screen rather than the map's surface; i.e., it will not necessarily change orientation due to map rotations, tilting, or zooming.

|Property|Type|Description|
|-|-|-|
|Position|Point|Represents the latitude/longitude position of pin on the map. Can be changed any time.|
|AnchorX|double|Represents the X coordinate of the pin's anchor. Can be changed any time. Can take values 0.0-1.0. Default is 0.5|
|AnchorY|double|Represents the Y coordinate of the pin's anchor. Can be changed any time. Can take values 0.0-1.0. Default is 1.0|
|Opacity|double|Sets the opacity of the pin. Default is 1.0|
|Title|string|A text string that's displayed in default info window when the user taps the pin. Can be changed any time.|
|Snippet|string|Additional text that's displayed below the title. You can change this value at any time.|
|Draggable|bool|Indicates whether the pin can be dragged. Can be changed any time. Deafult is `false`|
|ShowInfoWindowOnPinSelection|bool|Indicates whether the pin's should be shown when pin selected. Default is `true`.|
|CanBeSelected|bool|Indicates whether the pin can be selected. Default is `true`.|
|InfoWindowAnchor|Point|Represents the point in the pin image at which to anchor the info window when it is displayed. Default is `new Point(0.5, 0.0)`.|
|Rotation|double|Represents the rotation of the marker in degrees clockwise about the pin's anchor point. Default is 0.0|
|IsFlat|bool|Indicates whether the pin should be flat against the map `true` or a billboard facing the camera `false`. Default is `false`.|
|ZIndex|int|The draw order for the pins. The pins are drawn in order of the `ZIndex`, with the highest `ZIndex` pin drawn on top. Default is 0.|
|IsVisible|bool|Indicates whether the pin is visible. Default is `true`.|
|IsEnabled|bool|Indicates whether the pin can be clicked on. If `false` `PinClicked` event will not be fired whe user clicks the pin, also it will not be selected. Default is `true`.|
|Icon|ImageSource|The MAUI's `ImageSource` that can be used as an icon for pin. This means that you can use different sources, such as url, stream, file etc to set the icon. Also, you can use `ViewImageSource` to provide a custom view as pin icon. If the icon is left unset, a default icon will be displayed.|
