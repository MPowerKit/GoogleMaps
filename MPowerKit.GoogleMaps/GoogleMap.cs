using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

using Microsoft.Maui.Controls.Shapes;

namespace MPowerKit.GoogleMaps;

public class GoogleMap : View
{
    public event Action<Polygon>? PolygonClick;
    public event Action<Polyline>? PolylineClick;
    public event Action<Circle>? CircleClick;
    public event Action<GroundOverlay>? GroundOverlayClick;
    public event Action<Pin>? PinClick;
    public event Action<Pin>? PinDragStart;
    public event Action<Pin>? PinDragging;
    public event Action<Pin>? PinDragEnd;
    public event Action<Pin>? InfoWindowClick;
    public event Action<Pin>? InfoWindowLongClick;
    public event Action<Pin>? InfoWindowClose;
    public event Action<PointOfInterest>? PoiClick;
    public event Action<Point>? MapClick;
    public event Action<Point>? MapLongClick;
    public event Action? NativeMapReady;
    public event Action<CameraPosition>? CameraChange;
    public event Action<CameraMoveReason>? CameraMoveStart;
    public event Action? CameraMoveCanceled;
    public event Action? CameraMove;
    public event Action<IndoorBuilding?>? IndoorBuildingFocused;
    public event Action<IndoorLevel?>? IndoorLevelActivated;
    public event Action<MapRegion>? VisibleRegionChanged;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Action<CameraUpdate>? MoveCameraActionInternal;
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Func<CameraUpdate, int, Task>? AnimateCameraFuncInternal;
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Func<Point, Point>? MapCoordsToScreenLocationFuncInternal;
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Func<Point, Point>? ScreenLocationToMapCoordsFuncInternal;
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Action? ResetMinMaxZoomActionInternal;
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Func<Task<Stream?>>? TakeSnapshotFuncInternal;

    protected Pin PrevSelectedPin { get; set; }

    public GoogleMap()
    {
        MapCoordsToScreenLocationFunc = MapCoordsToScreenLocation;
        ScreenLocationToMapCoordsFunc = ScreenLocationToMapCoords;
        MoveCameraAction = MoveCamera;
        AnimateCameraFunc = AnimateCamera;
        ResetMinMaxZoomAction = ResetMinMaxZoom;
        TakeSnapshotFunc = TakeSnapshot;
    }

    public virtual async Task<Stream?> TakeSnapshot()
    {
        if (TakeSnapshotFuncInternal is null) return null;

        return await TakeSnapshotFuncInternal.Invoke();
    }

    public virtual void ResetMinMaxZoom()
    {
        ResetMinMaxZoomActionInternal?.Invoke();
    }

    public virtual void MoveCamera(CameraUpdate cameraUpdate)
    {
        MoveCameraActionInternal?.Invoke(cameraUpdate);
    }

    public virtual Task AnimateCamera(CameraUpdate cameraUpdate, int durationMils = 300)
    {
        if (AnimateCameraFuncInternal is null) return Task.CompletedTask;

        return AnimateCameraFuncInternal.Invoke(cameraUpdate, durationMils);
    }

    public virtual Point? MapCoordsToScreenLocation(Point latlng)
    {
        return MapCoordsToScreenLocationFuncInternal?.Invoke(latlng);
    }

    public virtual Point? ScreenLocationToMapCoords(Point point)
    {
        return ScreenLocationToMapCoordsFuncInternal?.Invoke(point);
    }

    protected override void OnPropertyChanging([CallerMemberName] string? propertyName = null)
    {
        base.OnPropertyChanging(propertyName);

        if (propertyName == PolylinesSourceProperty.PropertyName
            || propertyName == PolylineItemTemplateProperty.PropertyName)
        {
            var polylines = Polylines;
            Reset(PolylinesSource, ref polylines);
            Polylines = polylines;
        }
        else if (propertyName == PolygonsSourceProperty.PropertyName
            || propertyName == PolygonItemTemplateProperty.PropertyName)
        {
            var polygons = Polygons;
            Reset(PolygonsSource, ref polygons);
            Polygons = polygons;
        }
        else if (propertyName == CirclesSourceProperty.PropertyName
            || propertyName == CircleItemTemplateProperty.PropertyName)
        {
            var circles = Circles;
            Reset(CirclesSource, ref circles);
            Circles = circles;
        }
        else if (propertyName == TileOverlaysSourceProperty.PropertyName
            || propertyName == TileOverlayItemTemplateProperty.PropertyName)
        {
            var tileOverlays = TileOverlays;
            Reset(TileOverlaysSource, ref tileOverlays);
            TileOverlays = tileOverlays;
        }
        else if (propertyName == GroundOverlaysSourceProperty.PropertyName
            || propertyName == GroundOverlayItemTemplateProperty.PropertyName)
        {
            var groundOverlays = GroundOverlays;
            Reset(GroundOverlaysSource, ref groundOverlays);
            GroundOverlays = groundOverlays;
        }
        else if (propertyName == PinsSourceProperty.PropertyName
            || propertyName == PinItemTemplateProperty.PropertyName)
        {
            var pins = Pins;
            Reset(PinsSource, ref pins);
            Pins = pins;
        }
        else if (propertyName == SelectedPinProperty.PropertyName)
        {
            PrevSelectedPin = SelectedPin;
        }
    }

    protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName == PolylinesSourceProperty.PropertyName
            || propertyName == PolylineItemTemplateProperty.PropertyName)
        {
            var polylines = Polylines;
            Init(PolylinesSource, PolylineItemTemplate, ref polylines);
            Polylines = polylines;
        }
        else if (propertyName == PolygonsSourceProperty.PropertyName
            || propertyName == PolygonItemTemplateProperty.PropertyName)
        {
            var polygons = Polygons;
            Init(PolygonsSource, PolygonItemTemplate, ref polygons);
            Polygons = polygons;
        }
        else if (propertyName == CirclesSourceProperty.PropertyName
            || propertyName == CircleItemTemplateProperty.PropertyName)
        {
            var circles = Circles;
            Init(CirclesSource, CircleItemTemplate, ref circles);
            Circles = circles;
        }
        else if (propertyName == TileOverlaysSourceProperty.PropertyName
            || propertyName == TileOverlayItemTemplateProperty.PropertyName)
        {
            var tileOverlays = TileOverlays;
            Init(TileOverlaysSource, TileOverlayItemTemplate, ref tileOverlays);
            TileOverlays = tileOverlays;
        }
        else if (propertyName == GroundOverlaysSourceProperty.PropertyName
            || propertyName == GroundOverlayItemTemplateProperty.PropertyName)
        {
            var groundOverlays = GroundOverlays;
            Init(GroundOverlaysSource, GroundOverlayItemTemplate, ref groundOverlays);
            GroundOverlays = groundOverlays;
        }
        else if (propertyName == PinsSourceProperty.PropertyName
            || propertyName == PinItemTemplateProperty.PropertyName)
        {
            var pins = Pins;
            Init(PinsSource, PinItemTemplate, ref pins);
            Pins = pins;
        }
        else if (propertyName == SelectedPinDataProperty.PropertyName && PinsSource is not null)
        {
            var selectedPin = Pins.FirstOrDefault(p => p.BindingContext == SelectedPinData);
            if (selectedPin?.CanBeSelected is true)
            {
                SelectedPin = selectedPin;
            }
            else
            {
                SelectedPinData = null;
            }
        }
        else if (propertyName == SelectedPinProperty.PropertyName)
        {
            PrevSelectedPin?.HideInfoWindow();

            PinSelectionChanged();
        }
    }

    protected virtual void PinSelectionChanged()
    {
        if (PinsSource is not null)
        {
            var selectedPinData = PinsSource.OfType<object>().FirstOrDefault(p => p == SelectedPin?.BindingContext);
            SelectedPinData = selectedPinData;
        }

        SelectedPin?.ShowInfoWindow();
    }

    protected virtual void Reset<T>(IEnumerable source, ref IEnumerable<T>? mapObjects)
        where T : VisualElement
    {
        if (source is INotifyCollectionChanged collectionChanged)
        {
            collectionChanged.CollectionChanged -= Source_CollectionChanged<T>;
        }

        (mapObjects as ObservableCollection<T>)?.Clear();
        mapObjects = null;
        SelectedPin = null;
    }

    protected virtual void Init<T>(IEnumerable source, DataTemplate itemTemplate, ref IEnumerable<T> mapObjects)
        where T : VisualElement
    {
        if (source is null || itemTemplate is null) return;

        if (source is INotifyCollectionChanged collectionChanged)
        {
            collectionChanged.CollectionChanged += Source_CollectionChanged<T>;
        }

        mapObjects = new ObservableCollection<T>();

        AddMapObjects(source, (mapObjects as ObservableCollection<T>)!, 0, itemTemplate);
    }

    protected virtual (ObservableCollection<T>?, DataTemplate?) GetMapObjectsAndTemplate<T>()
        where T : VisualElement
    {
        var mapObjects = typeof(T).Name switch
        {
            nameof(Polyline) => Polylines as ObservableCollection<T>,
            nameof(Polygon) => Polygons as ObservableCollection<T>,
            nameof(Circle) => Circles as ObservableCollection<T>,
            nameof(TileOverlay) => TileOverlays as ObservableCollection<T>,
            nameof(GroundOverlay) => GroundOverlays as ObservableCollection<T>,
            nameof(Pin) => Pins as ObservableCollection<T>,
            _ => null
        };

        var itemTemplate = typeof(T).Name switch
        {
            nameof(Polyline) => PolylineItemTemplate,
            nameof(Polygon) => PolygonItemTemplate,
            nameof(Circle) => CircleItemTemplate,
            nameof(TileOverlay) => TileOverlayItemTemplate,
            nameof(GroundOverlay) => GroundOverlayItemTemplate,
            nameof(Pin) => PinItemTemplate,
            _ => null
        };

        return (mapObjects, itemTemplate);
    }

    protected virtual void Source_CollectionChanged<T>(object? sender, NotifyCollectionChangedEventArgs e)
        where T : VisualElement
    {
        var (mapObjects, itemTemplate) = GetMapObjectsAndTemplate<T>();

        if (itemTemplate is null || mapObjects is null) return;

        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                AddMapObjects(e.NewItems!, mapObjects!, e.NewStartingIndex, itemTemplate!);
                break;
            case NotifyCollectionChangedAction.Remove:
                RemoveMapObjects(e.OldItems!, mapObjects!, e.OldStartingIndex);
                break;
            case NotifyCollectionChangedAction.Replace:
                RemoveMapObjects(e.OldItems!, mapObjects!, e.OldStartingIndex);
                AddMapObjects(e.NewItems!, mapObjects!, e.NewStartingIndex, itemTemplate!);
                break;
            case NotifyCollectionChangedAction.Move:
                break;
            case NotifyCollectionChangedAction.Reset:
                ResetMapObjects(mapObjects, itemTemplate);
                break;
        }
    }

    protected virtual void AddMapObjects<T>(IEnumerable source, IList<T> dest, int fromIndex, DataTemplate itemTemplate)
        where T : VisualElement
    {
        var index = fromIndex;

        foreach (var item in source)
        {
            var template = itemTemplate;
            while (template is DataTemplateSelector selector)
            {
                template = selector.SelectTemplate(item, this);
            }

            var @object = template?.CreateContent();

            var typeName = typeof(T).Name;

            if (@object is not T mo) throw new InvalidOperationException($"{typeName}ItemTemplate must return a {typeName}");
            mo.BindingContext = item;

            dest.Insert(index++, mo);
        }
    }

    protected virtual void RemoveMapObjects<T>(IEnumerable source, IList<T> dest, int fromIndex)
        where T : VisualElement
    {
        var index = fromIndex;

        foreach (var item in source)
        {
            dest.RemoveAt(index);
        }

        if (typeof(T).Name == nameof(Pin) && dest.Count == 0)
        {
            SelectedPin = null;
        }
    }

    protected virtual void ResetMapObjects<T>(IList<T> dest, DataTemplate itemTemplate)
        where T : VisualElement
    {
        dest.Clear();

        var source = typeof(T).Name switch
        {
            nameof(Polyline) => PolylinesSource,
            nameof(Polygon) => PolygonsSource,
            nameof(Circle) => CirclesSource,
            nameof(TileOverlay) => TileOverlaysSource,
            nameof(GroundOverlay) => GroundOverlaysSource,
            nameof(Pin) => PinsSource,
            _ => null
        };

        if (source is null || dest is null || itemTemplate is null) return;

        AddMapObjects(source, dest, 0, itemTemplate);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void SendPolygonClick(Polygon polygon)
    {
        PolygonClick?.Invoke(polygon);

        if (PolygonClickedCommand?.CanExecute(polygon) is true)
            PolygonClickedCommand.Execute(polygon);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void SendCircleClick(Circle circe)
    {
        CircleClick?.Invoke(circe);

        if (CircleClickedCommand?.CanExecute(circe) is true)
            CircleClickedCommand.Execute(circe);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void SendPolylineClick(Polyline polyline)
    {
        PolylineClick?.Invoke(polyline);

        if (PolylineClickedCommand?.CanExecute(polyline) is true)
            PolylineClickedCommand.Execute(polyline);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void SendGroundOverlayClick(GroundOverlay groundOverlay)
    {
        GroundOverlayClick?.Invoke(groundOverlay);

        if (GroundOverlayClickedCommand?.CanExecute(groundOverlay) is true)
            GroundOverlayClickedCommand.Execute(groundOverlay);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void SendPinClick(Pin pin)
    {
        if (pin.CanBeSelected)
        {
            SelectedPin = pin;
        }

        PinClick?.Invoke(pin);

        var parameter = pin.BindingContext ?? pin;

        if (PinClickedCommand?.CanExecute(parameter) is true)
            PinClickedCommand.Execute(parameter);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void SendPinDragStart(Pin pin)
    {
        PinDragStart?.Invoke(pin);

        var parameter = pin.BindingContext ?? pin;

        if (PinDragStartedCommand?.CanExecute(parameter) is true)
            PinDragStartedCommand.Execute(parameter);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void SendPinDragging(Pin pin)
    {
        PinDragging?.Invoke(pin);

        var parameter = pin.BindingContext ?? pin;

        if (PinDraggingCommand?.CanExecute(parameter) is true)
            PinDraggingCommand.Execute(parameter);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void SendPinDragEnd(Pin pin)
    {
        PinDragEnd?.Invoke(pin);

        var parameter = pin.BindingContext ?? pin;

        if (PinDragEndedCommand?.CanExecute(parameter) is true)
            PinDragEndedCommand.Execute(parameter);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void SendInfoWindowClick(Pin pin)
    {
        InfoWindowClick?.Invoke(pin);

        var parameter = pin.BindingContext ?? pin;

        if (InfoWindowClickedCommand?.CanExecute(parameter) is true)
            InfoWindowClickedCommand.Execute(parameter);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void SendInfoWindowLongClick(Pin pin)
    {
        InfoWindowLongClick?.Invoke(pin);

        var parameter = pin.BindingContext ?? pin;

        if (InfoWindowLongClickedCommand?.CanExecute(parameter) is true)
            InfoWindowLongClickedCommand.Execute(parameter);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void SendInfoWindowClose(Pin pin)
    {
        InfoWindowClose?.Invoke(pin);

        var parameter = pin.BindingContext ?? pin;

        if (InfoWindowClosedCommand?.CanExecute(parameter) is true)
            InfoWindowClosedCommand.Execute(parameter);

        // Not sure if this needs to be here
        //if (SelectedPin is null || SelectedPin != pin) return;

        //SelectedPin = null;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void SendPoiClick(PointOfInterest poi)
    {
        PoiClick?.Invoke(poi);

        if (PoiClickedCommand?.CanExecute(poi) is true)
            PoiClickedCommand.Execute(poi);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void SendMapClick(Point point)
    {
        MapClick?.Invoke(point);

        if (MapClickedCommand?.CanExecute(point) is true)
            MapClickedCommand.Execute(point);

        SelectedPin = null;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void SendMapLongClick(Point point)
    {
        MapLongClick?.Invoke(point);

        if (MapLongClickedCommand?.CanExecute(point) is true)
            MapLongClickedCommand.Execute(point);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void SendVisibleRegionChanged(MapRegion region)
    {
        VisibleRegion = region;

        VisibleRegionChanged?.Invoke(region);

        if (VisibleRegionChangedCommand?.CanExecute(region) is true)
            VisibleRegionChangedCommand.Execute(region);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void SendNativeMapReady()
    {
        IsNativeMapReady = true;

        NativeMapReady?.Invoke();

        if (NativeMapReadyCommand?.CanExecute(null) is true)
            NativeMapReadyCommand.Execute(null);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void SendCameraChange(CameraPosition cameraPosition)
    {
        CameraChange?.Invoke(cameraPosition);

        if (CameraChangedCommand?.CanExecute(cameraPosition) is true)
            CameraChangedCommand.Execute(cameraPosition);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void SendCameraMoveStart(CameraMoveReason reason)
    {
        CameraMoveStart?.Invoke(reason);

        if (CameraMoveStartedCommand?.CanExecute(reason) is true)
            CameraMoveStartedCommand.Execute(reason);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void SendCameraMoveCanceled()
    {
        CameraMoveCanceled?.Invoke();

        if (CameraMoveCanceledCommand?.CanExecute(null) is true)
            CameraMoveCanceledCommand.Execute(null);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void SendCameraMove()
    {
        CameraMove?.Invoke();

        if (CameraMoveCommand?.CanExecute(null) is true)
            CameraMoveCommand.Execute(null);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void SendMapCapabilitiesChanged(MapCapabilities mapCapabilities)
    {
        MapCapabilities = mapCapabilities;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void SendIndoorBuildingFocused(IndoorBuilding? indoorBuilding)
    {
        FocusedBuilding = indoorBuilding;

        IndoorBuildingFocused?.Invoke(indoorBuilding);

        if (IndoorBuildingFocusedCommand?.CanExecute(indoorBuilding) is true)
            IndoorBuildingFocusedCommand.Execute(indoorBuilding);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void SendIndoorLevelActivated(IndoorLevel? activeLevel)
    {
        ActiveLevel = activeLevel;

        IndoorLevelActivated?.Invoke(activeLevel);

        if (IndoorLevelActivatedCommand?.CanExecute(activeLevel) is true)
            IndoorLevelActivatedCommand.Execute(activeLevel);
    }

    #region MapCapabilities
    public MapCapabilities MapCapabilities
    {
        get => (MapCapabilities)GetValue(MapCapabilitiesProperty);
        protected set => SetValue(MapCapabilitiesProperty, value);
    }

    public static readonly BindableProperty MapCapabilitiesProperty =
        BindableProperty.Create(
            nameof(MapCapabilities),
            typeof(MapCapabilities),
            typeof(GoogleMap),
            defaultBindingMode: BindingMode.OneWayToSource);
    #endregion

    #region FocusedBuilding
    public IndoorBuilding FocusedBuilding
    {
        get => (IndoorBuilding)GetValue(FocusedBuildingProperty);
        protected set => SetValue(FocusedBuildingProperty, value);
    }

    public static readonly BindableProperty FocusedBuildingProperty =
        BindableProperty.Create(
            nameof(FocusedBuilding),
            typeof(IndoorBuilding),
            typeof(GoogleMap),
            defaultBindingMode: BindingMode.OneWayToSource);
    #endregion

    #region ActiveLevel
    public IndoorLevel ActiveLevel
    {
        get => (IndoorLevel)GetValue(ActiveLevelProperty);
        protected set => SetValue(ActiveLevelProperty, value);
    }

    public static readonly BindableProperty ActiveLevelProperty =
        BindableProperty.Create(
            nameof(ActiveLevel),
            typeof(IndoorLevel),
            typeof(GoogleMap),
            defaultBindingMode: BindingMode.OneWayToSource);
    #endregion

    #region IndoorEnabled
    public bool IndoorEnabled
    {
        get => (bool)GetValue(IndoorEnabledProperty);
        set => SetValue(IndoorEnabledProperty, value);
    }

    public static readonly BindableProperty IndoorEnabledProperty =
        BindableProperty.Create(
            nameof(IndoorEnabled),
            typeof(bool),
            typeof(GoogleMap)
            );
    #endregion

    #region BuildingsEnabled
    public bool BuildingsEnabled
    {
        get => (bool)GetValue(BuildingsEnabledProperty);
        set => SetValue(BuildingsEnabledProperty, value);
    }

    public static readonly BindableProperty BuildingsEnabledProperty =
        BindableProperty.Create(
            nameof(BuildingsEnabled),
            typeof(bool),
            typeof(GoogleMap)
            );
    #endregion

    #region TrafficEnabled
    public bool TrafficEnabled
    {
        get => (bool)GetValue(TrafficEnabledProperty);
        set => SetValue(TrafficEnabledProperty, value);
    }

    public static readonly BindableProperty TrafficEnabledProperty =
        BindableProperty.Create(
            nameof(TrafficEnabled),
            typeof(bool),
            typeof(GoogleMap)
            );
    #endregion

    #region MyLocationEnabled
    public bool MyLocationEnabled
    {
        get => (bool)GetValue(MyLocationEnabledProperty);
        set => SetValue(MyLocationEnabledProperty, value);
    }

    public static readonly BindableProperty MyLocationEnabledProperty =
        BindableProperty.Create(
            nameof(MyLocationEnabled),
            typeof(bool),
            typeof(GoogleMap)
            );
    #endregion

    #region MapType
    public MapType MapType
    {
        get => (MapType)GetValue(MapTypeProperty);
        set => SetValue(MapTypeProperty, value);
    }

    public static readonly BindableProperty MapTypeProperty =
        BindableProperty.Create(
            nameof(MapType),
            typeof(MapType),
            typeof(GoogleMap),
            MapType.Normal
            );
    #endregion

    #region MapColorScheme
    public MapColorScheme MapColorScheme
    {
        get => (MapColorScheme)GetValue(MapColorSchemeProperty);
        set => SetValue(MapColorSchemeProperty, value);
    }

    public static readonly BindableProperty MapColorSchemeProperty =
        BindableProperty.Create(
            nameof(MapColorScheme),
            typeof(MapColorScheme),
            typeof(GoogleMap),
            MapColorScheme.FollowSystem
            );
    #endregion

    #region MyLocationButtonEnabled
    public bool MyLocationButtonEnabled
    {
        get => (bool)GetValue(MyLocationButtonEnabledProperty);
        set => SetValue(MyLocationButtonEnabledProperty, value);
    }

    public static readonly BindableProperty MyLocationButtonEnabledProperty =
        BindableProperty.Create(
            nameof(MyLocationButtonEnabled),
            typeof(bool),
            typeof(GoogleMap),
            true
            );
    #endregion

    #region IndoorLevelPickerEnabled
    public bool IndoorLevelPickerEnabled
    {
        get => (bool)GetValue(IndoorLevelPickerEnabledProperty);
        set => SetValue(IndoorLevelPickerEnabledProperty, value);
    }

    public static readonly BindableProperty IndoorLevelPickerEnabledProperty =
        BindableProperty.Create(
            nameof(IndoorLevelPickerEnabled),
            typeof(bool),
            typeof(GoogleMap),
            true
            );
    #endregion

    #region CompassEnabled
    public bool CompassEnabled
    {
        get => (bool)GetValue(CompassEnabledProperty);
        set => SetValue(CompassEnabledProperty, value);
    }

    public static readonly BindableProperty CompassEnabledProperty =
        BindableProperty.Create(
            nameof(CompassEnabled),
            typeof(bool),
            typeof(GoogleMap),
            true
            );
    #endregion

    #region MapToolbarEnabled
    public bool MapToolbarEnabled
    {
        get => (bool)GetValue(MapToolbarEnabledProperty);
        set => SetValue(MapToolbarEnabledProperty, value);
    }

    public static readonly BindableProperty MapToolbarEnabledProperty =
        BindableProperty.Create(
            nameof(MapToolbarEnabled),
            typeof(bool),
            typeof(GoogleMap),
            true
            );
    #endregion

    #region ZoomControlsEnabled
    public bool ZoomControlsEnabled
    {
        get => (bool)GetValue(ZoomControlsEnabledProperty);
        set => SetValue(ZoomControlsEnabledProperty, value);
    }

    public static readonly BindableProperty ZoomControlsEnabledProperty =
        BindableProperty.Create(
            nameof(ZoomControlsEnabled),
            typeof(bool),
            typeof(GoogleMap),
            true
            );
    #endregion

    #region ZoomGesturesEnabled
    public bool ZoomGesturesEnabled
    {
        get => (bool)GetValue(ZoomGesturesEnabledProperty);
        set => SetValue(ZoomGesturesEnabledProperty, value);
    }

    public static readonly BindableProperty ZoomGesturesEnabledProperty =
        BindableProperty.Create(
            nameof(ZoomGesturesEnabled),
            typeof(bool),
            typeof(GoogleMap),
            true
            );
    #endregion

    #region ScrollGesturesEnabled
    public bool ScrollGesturesEnabled
    {
        get => (bool)GetValue(ScrollGesturesEnabledProperty);
        set => SetValue(ScrollGesturesEnabledProperty, value);
    }

    public static readonly BindableProperty ScrollGesturesEnabledProperty =
        BindableProperty.Create(
            nameof(ScrollGesturesEnabled),
            typeof(bool),
            typeof(GoogleMap),
            true
            );
    #endregion

    #region TiltGesturesEnabled
    public bool TiltGesturesEnabled
    {
        get => (bool)GetValue(TiltGesturesEnabledProperty);
        set => SetValue(TiltGesturesEnabledProperty, value);
    }

    public static readonly BindableProperty TiltGesturesEnabledProperty =
        BindableProperty.Create(
            nameof(TiltGesturesEnabled),
            typeof(bool),
            typeof(GoogleMap),
            true
            );
    #endregion

    #region RotateGesturesEnabled
    public bool RotateGesturesEnabled
    {
        get => (bool)GetValue(RotateGesturesEnabledProperty);
        set => SetValue(RotateGesturesEnabledProperty, value);
    }

    public static readonly BindableProperty RotateGesturesEnabledProperty =
        BindableProperty.Create(
            nameof(RotateGesturesEnabled),
            typeof(bool),
            typeof(GoogleMap),
            true
            );
    #endregion

    #region Padding
    public Thickness Padding
    {
        get => (Thickness)GetValue(PaddingProperty);
        set => SetValue(PaddingProperty, value);
    }

    public static readonly BindableProperty PaddingProperty =
        BindableProperty.Create(
            nameof(Padding),
            typeof(Thickness),
            typeof(GoogleMap)
            );
    #endregion

    #region MinZoom
    public float MinZoom
    {
        get => (float)GetValue(MinZoomProperty);
        set => SetValue(MinZoomProperty, value);
    }

    public static readonly BindableProperty MinZoomProperty =
        BindableProperty.Create(
            nameof(MinZoom),
            typeof(float),
            typeof(GoogleMap),
            1f
            );
    #endregion

    #region MaxZoom
    public float MaxZoom
    {
        get => (float)GetValue(MaxZoomProperty);
        set => SetValue(MaxZoomProperty, value);
    }

    public static readonly BindableProperty MaxZoomProperty =
        BindableProperty.Create(
            nameof(MaxZoom),
            typeof(float),
            typeof(GoogleMap),
            21f
            );
    #endregion

    #region InfoWindowTemplate
    public DataTemplate InfoWindowTemplate
    {
        get => (DataTemplate)GetValue(InfoWindowTemplateProperty);
        set => SetValue(InfoWindowTemplateProperty, value);
    }

    public static readonly BindableProperty InfoWindowTemplateProperty =
        BindableProperty.Create(
            nameof(InfoWindowTemplate),
            typeof(DataTemplate),
            typeof(GoogleMap)
            );
    #endregion

    #region IsNativeMapReady
    public bool IsNativeMapReady
    {
        get => (bool)GetValue(IsNativeMapReadyProperty);
        protected set => SetValue(IsNativeMapReadyProperty, value);
    }

    public static readonly BindableProperty IsNativeMapReadyProperty =
        BindableProperty.Create(
            nameof(IsNativeMapReady),
            typeof(bool),
            typeof(GoogleMap),
            defaultBindingMode: BindingMode.OneWayToSource);
    #endregion

    #region MapStyleJson
    public string MapStyleJson
    {
        get => (string)GetValue(MapStyleJsonProperty);
        set => SetValue(MapStyleJsonProperty, value);
    }

    public static readonly BindableProperty MapStyleJsonProperty =
        BindableProperty.Create(
            nameof(MapStyleJson),
            typeof(string),
            typeof(GoogleMap)
            );
    #endregion

    #region ResetMinMaxZoomAction
    public Action ResetMinMaxZoomAction
    {
        get => (Action)GetValue(ResetMinMaxZoomActionProperty);
        protected set => SetValue(ResetMinMaxZoomActionProperty, value);
    }

    public static readonly BindableProperty ResetMinMaxZoomActionProperty =
        BindableProperty.Create(
            nameof(ResetMinMaxZoomAction),
            typeof(Action),
            typeof(GoogleMap),
            defaultBindingMode: BindingMode.OneWayToSource);
    #endregion

    #region TakeSnapshotFunc
    public Func<Task<Stream?>> TakeSnapshotFunc
    {
        get => (Func<Task<Stream?>>)GetValue(TakeSnapshotFuncProperty);
        protected set => SetValue(TakeSnapshotFuncProperty, value);
    }

    public static readonly BindableProperty TakeSnapshotFuncProperty =
        BindableProperty.Create(
            nameof(TakeSnapshotFunc),
            typeof(Func<Task<Stream?>>),
            typeof(GoogleMap),
            defaultBindingMode: BindingMode.OneWayToSource);
    #endregion

    #region MapCoordsToScreenLocationFunc
    public Func<Point, Point?> MapCoordsToScreenLocationFunc
    {
        get => (Func<Point, Point?>)GetValue(MapCoordsToScreenLocationFuncProperty);
        protected set => SetValue(MapCoordsToScreenLocationFuncProperty, value);
    }

    public static readonly BindableProperty MapCoordsToScreenLocationFuncProperty =
        BindableProperty.Create(
            nameof(MapCoordsToScreenLocationFunc),
            typeof(Func<Point, Point?>),
            typeof(GoogleMap),
            defaultBindingMode: BindingMode.OneWayToSource);
    #endregion

    #region ScreenLocationToMapCoordsFunc
    public Func<Point, Point?> ScreenLocationToMapCoordsFunc
    {
        get => (Func<Point, Point?>)GetValue(ScreenLocationToMapCoordsFuncProperty);
        protected set => SetValue(ScreenLocationToMapCoordsFuncProperty, value);
    }

    public static readonly BindableProperty ScreenLocationToMapCoordsFuncProperty =
        BindableProperty.Create(
            nameof(ScreenLocationToMapCoordsFunc),
            typeof(Func<Point, Point?>),
            typeof(GoogleMap),
            defaultBindingMode: BindingMode.OneWayToSource);
    #endregion

    #region AnimateCameraFunc
    public Func<CameraUpdate, int, Task> AnimateCameraFunc
    {
        get => (Func<CameraUpdate, int, Task>)GetValue(AnimateCameraFuncProperty);
        protected set => SetValue(AnimateCameraFuncProperty, value);
    }

    public static readonly BindableProperty AnimateCameraFuncProperty =
        BindableProperty.Create(
            nameof(AnimateCameraFunc),
            typeof(Func<CameraUpdate, int, Task>),
            typeof(GoogleMap),
            defaultBindingMode: BindingMode.OneWayToSource);
    #endregion

    #region MoveCameraAction
    public Action<CameraUpdate> MoveCameraAction
    {
        get => (Action<CameraUpdate>)GetValue(MoveCameraActionProperty);
        protected set => SetValue(MoveCameraActionProperty, value);
    }

    public static readonly BindableProperty MoveCameraActionProperty =
        BindableProperty.Create(
            nameof(MoveCameraAction),
            typeof(Action<CameraUpdate>),
            typeof(GoogleMap),
            defaultBindingMode: BindingMode.OneWayToSource);
    #endregion

    #region VisibleRegion
    public MapRegion VisibleRegion
    {
        get => (MapRegion)GetValue(VisibleRegionProperty);
        protected set => SetValue(VisibleRegionProperty, value);
    }

    public static readonly BindableProperty VisibleRegionProperty =
        BindableProperty.Create(
            nameof(VisibleRegion),
            typeof(MapRegion),
            typeof(GoogleMap),
            defaultBindingMode: BindingMode.OneWayToSource);
    #endregion

    #region HandlePoiClick
    public bool HandlePoiClick
    {
        get => (bool)GetValue(HandlePoiClickProperty);
        set => SetValue(HandlePoiClickProperty, value);
    }

    public static readonly BindableProperty HandlePoiClickProperty =
        BindableProperty.Create(
            nameof(HandlePoiClick),
            typeof(bool),
            typeof(GoogleMap));
    #endregion

    #region Polylines
    public IEnumerable<Polyline> Polylines
    {
        get => (IEnumerable<Polyline>)GetValue(PolylinesProperty);
        set => SetValue(PolylinesProperty, value);
    }

    public static readonly BindableProperty PolylinesProperty =
        BindableProperty.Create(
            nameof(Polylines),
            typeof(IEnumerable<Polyline>),
            typeof(GoogleMap));
    #endregion

    #region PolylinesSource
    public IEnumerable PolylinesSource
    {
        get => (IEnumerable)GetValue(PolylinesSourceProperty);
        set => SetValue(PolylinesSourceProperty, value);
    }

    public static readonly BindableProperty PolylinesSourceProperty =
        BindableProperty.Create(
            nameof(PolylinesSource),
            typeof(IEnumerable),
            typeof(GoogleMap));
    #endregion

    #region PolylineItemTemplate
    public DataTemplate PolylineItemTemplate
    {
        get => (DataTemplate)GetValue(PolylineItemTemplateProperty);
        set => SetValue(PolylineItemTemplateProperty, value);
    }

    public static readonly BindableProperty PolylineItemTemplateProperty =
        BindableProperty.Create(
            nameof(PolylineItemTemplate),
            typeof(DataTemplate),
            typeof(GoogleMap));
    #endregion

    #region Polygons
    public IEnumerable<Polygon> Polygons
    {
        get => (IEnumerable<Polygon>)GetValue(PolygonsProperty);
        set => SetValue(PolygonsProperty, value);
    }

    public static readonly BindableProperty PolygonsProperty =
        BindableProperty.Create(
            nameof(Polygons),
            typeof(IEnumerable<Polygon>),
            typeof(GoogleMap));
    #endregion

    #region PolygonsSource
    public IEnumerable PolygonsSource
    {
        get => (IEnumerable)GetValue(PolygonsSourceProperty);
        set => SetValue(PolygonsSourceProperty, value);
    }

    public static readonly BindableProperty PolygonsSourceProperty =
        BindableProperty.Create(
            nameof(PolygonsSource),
            typeof(IEnumerable),
            typeof(GoogleMap));
    #endregion

    #region PolygonItemTemplate
    public DataTemplate PolygonItemTemplate
    {
        get => (DataTemplate)GetValue(PolygonItemTemplateProperty);
        set => SetValue(PolygonItemTemplateProperty, value);
    }

    public static readonly BindableProperty PolygonItemTemplateProperty =
        BindableProperty.Create(
            nameof(PolygonItemTemplate),
            typeof(DataTemplate),
            typeof(GoogleMap));
    #endregion

    #region Circles
    public IEnumerable<Circle> Circles
    {
        get => (IEnumerable<Circle>)GetValue(CirclesProperty);
        set => SetValue(CirclesProperty, value);
    }

    public static readonly BindableProperty CirclesProperty =
        BindableProperty.Create(
            nameof(Circles),
            typeof(IEnumerable<Circle>),
            typeof(GoogleMap));
    #endregion

    #region CirclesSource
    public IEnumerable CirclesSource
    {
        get => (IEnumerable)GetValue(CirclesSourceProperty);
        set => SetValue(CirclesSourceProperty, value);
    }

    public static readonly BindableProperty CirclesSourceProperty =
        BindableProperty.Create(
            nameof(CirclesSource),
            typeof(IEnumerable),
            typeof(GoogleMap));
    #endregion

    #region CircleItemTemplate
    public DataTemplate CircleItemTemplate
    {
        get => (DataTemplate)GetValue(CircleItemTemplateProperty);
        set => SetValue(CircleItemTemplateProperty, value);
    }

    public static readonly BindableProperty CircleItemTemplateProperty =
        BindableProperty.Create(
            nameof(CircleItemTemplate),
            typeof(DataTemplate),
            typeof(GoogleMap));
    #endregion

    #region TileOverlays
    public IEnumerable<TileOverlay> TileOverlays
    {
        get => (IEnumerable<TileOverlay>)GetValue(TileOverlaysProperty);
        set => SetValue(TileOverlaysProperty, value);
    }

    public static readonly BindableProperty TileOverlaysProperty =
        BindableProperty.Create(
            nameof(TileOverlays),
            typeof(IEnumerable<TileOverlay>),
            typeof(GoogleMap));
    #endregion

    #region TileOverlaysSource
    public IEnumerable TileOverlaysSource
    {
        get => (IEnumerable)GetValue(TileOverlaysSourceProperty);
        set => SetValue(TileOverlaysSourceProperty, value);
    }

    public static readonly BindableProperty TileOverlaysSourceProperty =
        BindableProperty.Create(
            nameof(TileOverlaysSource),
            typeof(IEnumerable),
            typeof(GoogleMap));
    #endregion

    #region TileOverlayItemTemplate
    public DataTemplate TileOverlayItemTemplate
    {
        get => (DataTemplate)GetValue(TileOverlayItemTemplateProperty);
        set => SetValue(TileOverlayItemTemplateProperty, value);
    }

    public static readonly BindableProperty TileOverlayItemTemplateProperty =
        BindableProperty.Create(
            nameof(TileOverlayItemTemplate),
            typeof(DataTemplate),
            typeof(GoogleMap));
    #endregion

    #region GroundOverlays
    public IEnumerable<GroundOverlay> GroundOverlays
    {
        get => (IEnumerable<GroundOverlay>)GetValue(GroundOverlaysProperty);
        set => SetValue(GroundOverlaysProperty, value);
    }

    public static readonly BindableProperty GroundOverlaysProperty =
        BindableProperty.Create(
            nameof(GroundOverlays),
            typeof(IEnumerable<GroundOverlay>),
            typeof(GoogleMap));
    #endregion

    #region GroundOverlaysSource
    public IEnumerable GroundOverlaysSource
    {
        get => (IEnumerable)GetValue(GroundOverlaysSourceProperty);
        set => SetValue(GroundOverlaysSourceProperty, value);
    }

    public static readonly BindableProperty GroundOverlaysSourceProperty =
        BindableProperty.Create(
            nameof(GroundOverlaysSource),
            typeof(IEnumerable),
            typeof(GoogleMap));
    #endregion

    #region GroundOverlayItemTemplate
    public DataTemplate GroundOverlayItemTemplate
    {
        get => (DataTemplate)GetValue(GroundOverlayItemTemplateProperty);
        set => SetValue(GroundOverlayItemTemplateProperty, value);
    }

    public static readonly BindableProperty GroundOverlayItemTemplateProperty =
        BindableProperty.Create(
            nameof(GroundOverlayItemTemplate),
            typeof(DataTemplate),
            typeof(GoogleMap));
    #endregion

    #region Pins
    public IEnumerable<Pin> Pins
    {
        get => (IEnumerable<Pin>)GetValue(PinsProperty);
        set => SetValue(PinsProperty, value);
    }

    public static readonly BindableProperty PinsProperty =
        BindableProperty.Create(
            nameof(Pins),
            typeof(IEnumerable<Pin>),
            typeof(GoogleMap)
            );
    #endregion

    #region PinsSource
    public IEnumerable PinsSource
    {
        get => (IEnumerable)GetValue(PinsSourceProperty);
        set => SetValue(PinsSourceProperty, value);
    }

    public static readonly BindableProperty PinsSourceProperty =
        BindableProperty.Create(
            nameof(PinsSource),
            typeof(IEnumerable),
            typeof(GoogleMap));
    #endregion

    #region SelectedPin
    public Pin SelectedPin
    {
        get => (Pin)GetValue(SelectedPinProperty);
        set => SetValue(SelectedPinProperty, value);
    }

    public static readonly BindableProperty SelectedPinProperty =
        BindableProperty.Create(
            nameof(SelectedPin),
            typeof(Pin),
            typeof(GoogleMap),
            defaultBindingMode: BindingMode.TwoWay
            );
    #endregion

    #region SelectedPinData
    public object SelectedPinData
    {
        get => (object)GetValue(SelectedPinDataProperty);
        set => SetValue(SelectedPinDataProperty, value);
    }

    public static readonly BindableProperty SelectedPinDataProperty =
        BindableProperty.Create(
            nameof(SelectedPinData),
            typeof(object),
            typeof(GoogleMap),
            defaultBindingMode: BindingMode.TwoWay
            );
    #endregion

    #region PinItemTemplate
    public DataTemplate PinItemTemplate
    {
        get => (DataTemplate)GetValue(PinItemTemplateProperty);
        set => SetValue(PinItemTemplateProperty, value);
    }

    public static readonly BindableProperty PinItemTemplateProperty =
        BindableProperty.Create(
            nameof(PinItemTemplate),
            typeof(DataTemplate),
            typeof(GoogleMap));
    #endregion

    #region PolylineClickedCommand
    public ICommand PolylineClickedCommand
    {
        get => (ICommand)GetValue(PolylineClickedCommandProperty);
        set => SetValue(PolylineClickedCommandProperty, value);
    }

    public static readonly BindableProperty PolylineClickedCommandProperty =
        BindableProperty.Create(
            nameof(PolylineClickedCommand),
            typeof(ICommand),
            typeof(GoogleMap));
    #endregion

    #region PolygonClickedCommand
    public ICommand PolygonClickedCommand
    {
        get => (ICommand)GetValue(PolygonClickedCommandProperty);
        set => SetValue(PolygonClickedCommandProperty, value);
    }

    public static readonly BindableProperty PolygonClickedCommandProperty =
        BindableProperty.Create(
            nameof(PolygonClickedCommand),
            typeof(ICommand),
            typeof(GoogleMap));
    #endregion

    #region CircleClickedCommand
    public ICommand CircleClickedCommand
    {
        get => (ICommand)GetValue(CircleClickedCommandProperty);
        set => SetValue(CircleClickedCommandProperty, value);
    }

    public static readonly BindableProperty CircleClickedCommandProperty =
        BindableProperty.Create(
            nameof(CircleClickedCommand),
            typeof(ICommand),
            typeof(GoogleMap));
    #endregion

    #region GroundOverlayClickedCommand
    public ICommand GroundOverlayClickedCommand
    {
        get => (ICommand)GetValue(GroundOverlayClickedCommandProperty);
        set => SetValue(GroundOverlayClickedCommandProperty, value);
    }

    public static readonly BindableProperty GroundOverlayClickedCommandProperty =
        BindableProperty.Create(
            nameof(GroundOverlayClickedCommand),
            typeof(ICommand),
            typeof(GoogleMap));
    #endregion

    #region PinClickedCommand
    public ICommand PinClickedCommand
    {
        get => (ICommand)GetValue(PinClickedCommandProperty);
        set => SetValue(PinClickedCommandProperty, value);
    }

    public static readonly BindableProperty PinClickedCommandProperty =
        BindableProperty.Create(
            nameof(PinClickedCommand),
            typeof(ICommand),
            typeof(GoogleMap));
    #endregion

    #region PinDragStartedCommand
    public ICommand PinDragStartedCommand
    {
        get => (ICommand)GetValue(PinDragStartedCommandProperty);
        set => SetValue(PinDragStartedCommandProperty, value);
    }

    public static readonly BindableProperty PinDragStartedCommandProperty =
        BindableProperty.Create(
            nameof(PinDragStartedCommand),
            typeof(ICommand),
            typeof(GoogleMap));
    #endregion

    #region PinDraggedCommand
    public ICommand PinDraggingCommand
    {
        get => (ICommand)GetValue(PinDraggingCommandProperty);
        set => SetValue(PinDraggingCommandProperty, value);
    }

    public static readonly BindableProperty PinDraggingCommandProperty =
        BindableProperty.Create(
            nameof(PinDraggingCommand),
            typeof(ICommand),
            typeof(GoogleMap));
    #endregion

    #region PinDragEndedCommand
    public ICommand PinDragEndedCommand
    {
        get => (ICommand)GetValue(PinDragEndedCommandProperty);
        set => SetValue(PinDragEndedCommandProperty, value);
    }

    public static readonly BindableProperty PinDragEndedCommandProperty =
        BindableProperty.Create(
            nameof(PinDragEndedCommand),
            typeof(ICommand),
            typeof(GoogleMap));
    #endregion

    #region InfoWindowClickedCommand
    public ICommand InfoWindowClickedCommand
    {
        get => (ICommand)GetValue(InfoWindowClickedCommandProperty);
        set => SetValue(InfoWindowClickedCommandProperty, value);
    }

    public static readonly BindableProperty InfoWindowClickedCommandProperty =
        BindableProperty.Create(
            nameof(InfoWindowClickedCommand),
            typeof(ICommand),
            typeof(GoogleMap));
    #endregion

    #region InfoWindowLongClickedCommand
    public ICommand InfoWindowLongClickedCommand
    {
        get => (ICommand)GetValue(InfoWindowLongClickedCommandProperty);
        set => SetValue(InfoWindowLongClickedCommandProperty, value);
    }

    public static readonly BindableProperty InfoWindowLongClickedCommandProperty =
        BindableProperty.Create(
            nameof(InfoWindowLongClickedCommand),
            typeof(ICommand),
            typeof(GoogleMap));
    #endregion

    #region InfoWindowClosedCommand
    public ICommand InfoWindowClosedCommand
    {
        get => (ICommand)GetValue(InfoWindowClosedCommandProperty);
        set => SetValue(InfoWindowClosedCommandProperty, value);
    }

    public static readonly BindableProperty InfoWindowClosedCommandProperty =
        BindableProperty.Create(
            nameof(InfoWindowClosedCommand),
            typeof(ICommand),
            typeof(GoogleMap));
    #endregion

    #region PoiClickedCommand
    public ICommand PoiClickedCommand
    {
        get => (ICommand)GetValue(PoiClickedCommandProperty);
        set => SetValue(PoiClickedCommandProperty, value);
    }

    public static readonly BindableProperty PoiClickedCommandProperty =
        BindableProperty.Create(
            nameof(PoiClickedCommand),
            typeof(ICommand),
            typeof(GoogleMap));
    #endregion

    #region MapClickedCommand
    public ICommand MapClickedCommand
    {
        get => (ICommand)GetValue(MapClickedCommandProperty);
        set => SetValue(MapClickedCommandProperty, value);
    }

    public static readonly BindableProperty MapClickedCommandProperty =
        BindableProperty.Create(
            nameof(MapClickedCommand),
            typeof(ICommand),
            typeof(GoogleMap));
    #endregion

    #region MapLongClickedCommand
    public ICommand MapLongClickedCommand
    {
        get => (ICommand)GetValue(MapLongClickedCommandProperty);
        set => SetValue(MapLongClickedCommandProperty, value);
    }

    public static readonly BindableProperty MapLongClickedCommandProperty =
        BindableProperty.Create(
            nameof(MapLongClickedCommand),
            typeof(ICommand),
            typeof(GoogleMap));
    #endregion

    #region NativeMapReadyCommand
    public ICommand NativeMapReadyCommand
    {
        get => (ICommand)GetValue(NativeMapReadyCommandProperty);
        set => SetValue(NativeMapReadyCommandProperty, value);
    }

    public static readonly BindableProperty NativeMapReadyCommandProperty =
        BindableProperty.Create(
            nameof(NativeMapReadyCommand),
            typeof(ICommand),
            typeof(GoogleMap));
    #endregion

    #region CameraChangedCommand
    public ICommand CameraChangedCommand
    {
        get => (ICommand)GetValue(CameraChangedCommandProperty);
        set => SetValue(CameraChangedCommandProperty, value);
    }

    public static readonly BindableProperty CameraChangedCommandProperty =
        BindableProperty.Create(
            nameof(CameraChangedCommand),
            typeof(ICommand),
            typeof(GoogleMap));
    #endregion

    #region CameraMoveStartedCommand
    public ICommand CameraMoveStartedCommand
    {
        get => (ICommand)GetValue(CameraMoveStartedCommandProperty);
        set => SetValue(CameraMoveStartedCommandProperty, value);
    }

    public static readonly BindableProperty CameraMoveStartedCommandProperty =
        BindableProperty.Create(
            nameof(CameraMoveStartedCommand),
            typeof(ICommand),
            typeof(GoogleMap));
    #endregion

    #region CameraMoveCanceledCommand
    public ICommand CameraMoveCanceledCommand
    {
        get => (ICommand)GetValue(CameraMoveCanceledCommandProperty);
        set => SetValue(CameraMoveCanceledCommandProperty, value);
    }

    public static readonly BindableProperty CameraMoveCanceledCommandProperty =
        BindableProperty.Create(
            nameof(CameraMoveCanceledCommand),
            typeof(ICommand),
            typeof(GoogleMap));
    #endregion

    #region CameraMoveCommand
    public ICommand CameraMoveCommand
    {
        get => (ICommand)GetValue(CameraMoveCommandProperty);
        set => SetValue(CameraMoveCommandProperty, value);
    }

    public static readonly BindableProperty CameraMoveCommandProperty =
        BindableProperty.Create(
            nameof(CameraMoveCommand),
            typeof(ICommand),
            typeof(GoogleMap));
    #endregion

    #region IndoorBuildingFocusedCommand
    public ICommand IndoorBuildingFocusedCommand
    {
        get => (ICommand)GetValue(IndoorBuildingFocusedCommandProperty);
        set => SetValue(IndoorBuildingFocusedCommandProperty, value);
    }

    public static readonly BindableProperty IndoorBuildingFocusedCommandProperty =
        BindableProperty.Create(
            nameof(IndoorBuildingFocusedCommand),
            typeof(ICommand),
            typeof(GoogleMap));
    #endregion

    #region IndoorLevelActivatedCommand
    public ICommand IndoorLevelActivatedCommand
    {
        get => (ICommand)GetValue(IndoorLevelActivatedCommandProperty);
        set => SetValue(IndoorLevelActivatedCommandProperty, value);
    }

    public static readonly BindableProperty IndoorLevelActivatedCommandProperty =
        BindableProperty.Create(
            nameof(IndoorLevelActivatedCommand),
            typeof(ICommand),
            typeof(GoogleMap));
    #endregion

    #region VisibleRegionChangedCommand
    public ICommand VisibleRegionChangedCommand
    {
        get => (ICommand)GetValue(VisibleRegionChangedCommandProperty);
        set => SetValue(VisibleRegionChangedCommandProperty, value);
    }

    public static readonly BindableProperty VisibleRegionChangedCommandProperty =
        BindableProperty.Create(
            nameof(VisibleRegionChangedCommand),
            typeof(ICommand),
            typeof(GoogleMap));
    #endregion
}