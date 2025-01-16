using System.ComponentModel;
using System.Windows.Input;

namespace MPowerKit.GoogleMaps;

public partial class GoogleMap
{
    public const string CameraManagerName = nameof(CameraManagerName);

    public event Action<CameraMoveReason>? CameraMoveStart;
    public event Action? CameraMoveCanceled;
    public event Action? CameraMove;
    public event Action<CameraPosition>? CameraPositionChanged;
    public event Action<VisibleRegion>? CameraIdle;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Action<CameraUpdate>? MoveCameraActionInternal;
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Func<CameraUpdate, int, Task>? AnimateCameraFuncInternal;
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Action? ResetMinMaxZoomActionInternal;

    protected virtual void InitCamera()
    {
        MoveCameraAction = MoveCamera;
        AnimateCameraFunc = AnimateCamera;
        ResetMinMaxZoomAction = ResetMinMaxZoom;
    }

    public virtual void ResetMinMaxZoom()
    {
        ResetMinMaxZoomActionInternal?.Invoke();
    }

    public virtual void MoveCamera(CameraUpdate newCameraPosition)
    {
        MoveCameraActionInternal?.Invoke(newCameraPosition);
    }

    public virtual Task AnimateCamera(CameraUpdate newCameraPosition, int durationMils = 300)
    {
        if (AnimateCameraFuncInternal is null) return Task.CompletedTask;

        return AnimateCameraFuncInternal.Invoke(newCameraPosition, durationMils);
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
    public virtual void SendCameraChanged(CameraPosition cameraPosition, bool raiseEvent = false)
    {
        CameraPosition = cameraPosition;

        if (!raiseEvent) return;

        CameraPositionChanged?.Invoke(cameraPosition);

        if (CameraPositionChangedCommand?.CanExecute(cameraPosition) is true)
            CameraPositionChangedCommand.Execute(cameraPosition);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void SendCameraIdle(VisibleRegion region, bool raiseEvent = true)
    {
        VisibleRegion = region;

        if (!raiseEvent) return;

        CameraIdle?.Invoke(region);

        if (CameraIdleCommand?.CanExecute(region) is true)
            CameraIdleCommand.Execute(region);
    }

    #region VisibleRegion
    public VisibleRegion VisibleRegion
    {
        get => (VisibleRegion)GetValue(VisibleRegionProperty);
        protected set => SetValue(VisibleRegionProperty, value);
    }

    public static readonly BindableProperty VisibleRegionProperty =
        BindableProperty.Create(
            nameof(VisibleRegion),
            typeof(VisibleRegion),
            typeof(GoogleMap),
            defaultBindingMode: BindingMode.OneWayToSource);
    #endregion

    #region CameraPosition
    public CameraPosition CameraPosition
    {
        get => (CameraPosition)GetValue(CameraPositionProperty);
        protected set => SetValue(CameraPositionProperty, value);
    }

    public static readonly BindableProperty CameraPositionProperty =
        BindableProperty.Create(
            nameof(CameraPosition),
            typeof(CameraPosition),
            typeof(GoogleMap),
            defaultBindingMode: BindingMode.OneWayToSource);
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

    #region InitialCameraPosition
    public CameraUpdate InitialCameraPosition
    {
        get => (CameraUpdate)GetValue(InitialCameraPositionProperty);
        set => SetValue(InitialCameraPositionProperty, value);
    }

    public static readonly BindableProperty InitialCameraPositionProperty =
        BindableProperty.Create(
            nameof(InitialCameraPosition),
            typeof(CameraUpdate),
            typeof(GoogleMap));
    #endregion

    #region RestrictPanningToArea
    public LatLngBounds? RestrictPanningToArea
    {
        get => (LatLngBounds?)GetValue(RestrictPanningToAreaProperty);
        set => SetValue(RestrictPanningToAreaProperty, value);
    }

    public static readonly BindableProperty RestrictPanningToAreaProperty =
        BindableProperty.Create(
            nameof(RestrictPanningToArea),
            typeof(LatLngBounds?),
            typeof(GoogleMap));
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
            2f
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

    #region CameraPositionChangedCommand
    public ICommand CameraPositionChangedCommand
    {
        get => (ICommand)GetValue(CameraPositionChangedCommandProperty);
        set => SetValue(CameraPositionChangedCommandProperty, value);
    }

    public static readonly BindableProperty CameraPositionChangedCommandProperty =
        BindableProperty.Create(
            nameof(CameraPositionChangedCommand),
            typeof(ICommand),
            typeof(GoogleMap));
    #endregion

    #region CameraIdleCommand
    public ICommand CameraIdleCommand
    {
        get => (ICommand)GetValue(CameraIdleCommandProperty);
        set => SetValue(CameraIdleCommandProperty, value);
    }

    public static readonly BindableProperty CameraIdleCommandProperty =
        BindableProperty.Create(
            nameof(CameraIdleCommand),
            typeof(ICommand),
            typeof(GoogleMap));
    #endregion
}