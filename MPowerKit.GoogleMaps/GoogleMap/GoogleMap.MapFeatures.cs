using System.ComponentModel;
using System.Windows.Input;

namespace MPowerKit.GoogleMaps;

public partial class GoogleMap
{
    public const string MapManagerName = nameof(MapManagerName);

    public event Action<PointOfInterest>? PoiClick;
    public event Action<Point>? MapClick;
    public event Action<Point>? MapLongClick;
    public event Action<IndoorBuilding?>? IndoorBuildingFocused;
    public event Action<IndoorLevel?>? IndoorLevelActivated;
    public event Action<MapCapabilities>? MapCapabilitiesChanged;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Func<Point, Point>? ProjectMapCoordsToScreenLocationFuncInternal;
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Func<Point, Point>? ProjectScreenLocationToMapCoordsFuncInternal;
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Func<Task<Stream?>>? TakeSnapshotFuncInternal;

    protected virtual void InitMapFeatures()
    {
        ProjectMapCoordsToScreenLocationFunc = ProjectMapCoordsToScreenLocation;
        ProjectScreenLocationToMapCoordsFunc = ProjectScreenLocationToMapCoords;
        TakeSnapshotFunc = TakeSnapshot;
    }

    public virtual async Task<Stream?> TakeSnapshot()
    {
        if (TakeSnapshotFuncInternal is null) return null;

        return await TakeSnapshotFuncInternal.Invoke();
    }

    public virtual Point? ProjectMapCoordsToScreenLocation(Point latlng)
    {
        return ProjectMapCoordsToScreenLocationFuncInternal?.Invoke(latlng);
    }

    public virtual Point? ProjectScreenLocationToMapCoords(Point screenPoint)
    {
        return ProjectScreenLocationToMapCoordsFuncInternal?.Invoke(screenPoint);
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
    public virtual void SendMapCapabilitiesChanged(MapCapabilities mapCapabilities, bool raiseEvent = true)
    {
        MapCapabilities = mapCapabilities;

        if (!raiseEvent) return;

        MapCapabilitiesChanged?.Invoke(mapCapabilities);

        if (MapCapabilitiesChangedCommand?.CanExecute(mapCapabilities) is true)
            MapCapabilitiesChangedCommand.Execute(mapCapabilities);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void SendIndoorBuildingFocused(IndoorBuilding? indoorBuilding, bool raiseEvent = true)
    {
        FocusedBuilding = indoorBuilding;

        if (!raiseEvent) return;

        IndoorBuildingFocused?.Invoke(indoorBuilding);

        if (IndoorBuildingFocusedCommand?.CanExecute(indoorBuilding) is true)
            IndoorBuildingFocusedCommand.Execute(indoorBuilding);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void SendIndoorLevelActivated(IndoorLevel? activeLevel, bool raiseEvent = true)
    {
        ActiveLevel = activeLevel;

        if (!raiseEvent) return;

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

    #region ProjectMapCoordsToScreenLocationFunc
    public Func<Point, Point?> ProjectMapCoordsToScreenLocationFunc
    {
        get => (Func<Point, Point?>)GetValue(ProjectMapCoordsToScreenLocationFuncProperty);
        protected set => SetValue(ProjectMapCoordsToScreenLocationFuncProperty, value);
    }

    public static readonly BindableProperty ProjectMapCoordsToScreenLocationFuncProperty =
        BindableProperty.Create(
            nameof(ProjectMapCoordsToScreenLocationFunc),
            typeof(Func<Point, Point?>),
            typeof(GoogleMap),
            defaultBindingMode: BindingMode.OneWayToSource);
    #endregion

    #region ProjectScreenLocationToMapCoordsFunc
    public Func<Point, Point?> ProjectScreenLocationToMapCoordsFunc
    {
        get => (Func<Point, Point?>)GetValue(ProjectScreenLocationToMapCoordsFuncProperty);
        protected set => SetValue(ProjectScreenLocationToMapCoordsFuncProperty, value);
    }

    public static readonly BindableProperty ProjectScreenLocationToMapCoordsFuncProperty =
        BindableProperty.Create(
            nameof(ProjectScreenLocationToMapCoordsFunc),
            typeof(Func<Point, Point?>),
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

    #region MapCapabilitiesChangedCommand
    public ICommand MapCapabilitiesChangedCommand
    {
        get => (ICommand)GetValue(MapCapabilitiesChangedCommandProperty);
        set => SetValue(MapCapabilitiesChangedCommandProperty, value);
    }

    public static readonly BindableProperty MapCapabilitiesChangedCommandProperty =
        BindableProperty.Create(
            nameof(MapCapabilitiesChangedCommand),
            typeof(ICommand),
            typeof(GoogleMap));
    #endregion
}