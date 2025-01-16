namespace MPowerKit.GoogleMaps;

public partial class GoogleMap
{
    public const string UiSettingsManagerName = nameof(UiSettingsManagerName);

    protected virtual void InitUiSettings()
    {

    }

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
}