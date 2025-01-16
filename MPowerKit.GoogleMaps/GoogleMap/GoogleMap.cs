using System.ComponentModel;
using System.Windows.Input;

namespace MPowerKit.GoogleMaps;

public partial class GoogleMap : View
{
    public event Action? NativeMapReady;

    public string MapId { get; init; }

    public GoogleMap()
    {
        InitCamera();
        InitMapFeatures();
        InitUiSettings();
        InitItems();
        InitPins();
        InitClusters();
        InitPolylines();
        InitPolygons();
        InitCircles();
        InitTiles();
        InitGroundOverlays();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void SendNativeMapReady()
    {
        IsNativeMapReady = true;

        NativeMapReady?.Invoke();

        if (NativeMapReadyCommand?.CanExecute(null) is true)
            NativeMapReadyCommand.Execute(null);
    }

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
}