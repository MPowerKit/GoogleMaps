using System.ComponentModel;

using CoreAnimation;

using Google.Maps;

using NCameraPosition = Google.Maps.CameraPosition;
using VCameraPosition = MPowerKit.GoogleMaps.CameraPosition;

namespace MPowerKit.GoogleMaps;

public class CameraManager : IMapFeatureManager<GoogleMap, MapView, GoogleMapHandler>
{
    protected GoogleMap? VirtualView { get; set; }
    protected MapView? PlatformView { get; set; }
    protected GoogleMapHandler? Handler { get; set; }

    public virtual void Connect(GoogleMap virtualView, MapView platformView, GoogleMapHandler handler)
    {
        VirtualView = virtualView;
        PlatformView = platformView;
        Handler = handler;

        InitCamera(virtualView, platformView, handler);

        SubscribeToEvents(virtualView, platformView, handler);
    }

    public virtual void Disconnect(GoogleMap virtualView, MapView platformView, GoogleMapHandler handler)
    {
        UnsubscribeFromEvents(virtualView, platformView, handler);

        VirtualView = null;
        PlatformView = null;
        Handler = null;
    }

    protected virtual void InitCamera(GoogleMap virtualView, MapView platformView, GoogleMapHandler handler)
    {
        OnMinMaxZoomChanged(virtualView, platformView);

        if (virtualView.RestrictPanningToArea is not null)
        {
            OnRestrictPanningToAreaChanged(virtualView, platformView);
            return;
        }

        if (virtualView.InitialCameraPosition is not null)
        {
            platformView.MoveCamera(virtualView.InitialCameraPosition.ToNative());
            return;
        }

        virtualView.SendCameraChange(platformView.Camera.ToCrossPlatform(), false);
        virtualView.SendCameraIdle(platformView.Projection.VisibleRegion.ToCrossPlatform(), false);
    }

    protected virtual void SubscribeToEvents(GoogleMap virtualView, MapView platformView, GoogleMapHandler handler)
    {
        virtualView.PropertyChanged += VirtualView_PropertyChanged;
        virtualView.PropertyChanging += VirtualView_PropertyChanging;

        platformView.CameraPositionChanged += PlatformView_CameraPositionChanged;
        platformView.CameraPositionIdle += PlatformView_CameraPositionIdle;
        platformView.WillMove += PlatformView_WillMove;

        virtualView.MoveCameraActionInternal = MoveCamera;
        virtualView.AnimateCameraFuncInternal = AnimateCamera;
    }

    protected virtual void UnsubscribeFromEvents(GoogleMap virtualView, MapView platformView, GoogleMapHandler handler)
    {
        virtualView.MoveCameraActionInternal = null;
        virtualView.AnimateCameraFuncInternal = null;

        platformView.CameraPositionChanged -= PlatformView_CameraPositionChanged;
        platformView.CameraPositionIdle -= PlatformView_CameraPositionIdle;
        platformView.WillMove -= PlatformView_WillMove;

        virtualView.PropertyChanged -= VirtualView_PropertyChanged;
        virtualView.PropertyChanging -= VirtualView_PropertyChanging;
    }

    protected virtual void VirtualView_PropertyChanging(object sender, Microsoft.Maui.Controls.PropertyChangingEventArgs e)
    {

    }

    protected virtual void VirtualView_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        var virtualView = VirtualView!;
        var platformView = PlatformView!;

        if (e.PropertyName == GoogleMap.MinZoomProperty.PropertyName
            || e.PropertyName == GoogleMap.MaxZoomProperty.PropertyName)
        {
            OnMinMaxZoomChanged(virtualView, platformView);
        }
        else if (e.PropertyName == GoogleMap.RestrictPanningToAreaProperty.PropertyName)
        {
            OnRestrictPanningToAreaChanged(virtualView, platformView);
        }
    }

    protected virtual void OnMinMaxZoomChanged(GoogleMap virtualView, MapView platformView)
    {
        platformView.SetMinMaxZoom(virtualView.MinZoom, virtualView.MaxZoom);
    }

    protected virtual void OnRestrictPanningToAreaChanged(GoogleMap virtualView, MapView platformView)
    {
        platformView.CameraTargetBounds = virtualView.RestrictPanningToArea?.ToNative();
    }

    protected virtual void PlatformView_CameraPositionChanged(object? sender, GMSCameraEventArgs e)
    {
        VirtualView!.SendCameraChange(e.Position.ToCrossPlatform());
        VirtualView!.SendCameraMove();
    }

    protected virtual void PlatformView_CameraPositionIdle(object? sender, GMSCameraEventArgs e)
    {
        VirtualView!.SendCameraChange(e.Position.ToCrossPlatform());
        VirtualView!.SendCameraIdle(PlatformView!.Projection.VisibleRegion.ToCrossPlatform());
    }

    protected virtual void PlatformView_WillMove(object? sender, GMSWillMoveEventArgs e)
    {
        VirtualView!.SendCameraMoveStart(e.Gesture ? CameraMoveReason.Gesture : CameraMoveReason.ApiAnimation);
    }

    protected virtual void PlatformView_CameraMoveCanceled(object? sender, EventArgs e)
    {
        VirtualView!.SendCameraMoveCanceled();
    }

    protected virtual void PlatformView_CameraMove(object? sender, EventArgs e)
    {
        VirtualView!.SendCameraMove();
    }

    protected virtual Task AnimateCamera(CameraUpdate update, int durationMils = 300)
    {
        TaskCompletionSource tcs = new();

        try
        {
            var native = update.ToNative();

            CATransaction.Begin();
            CATransaction.AnimationTimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseOut);
            CATransaction.AnimationDuration = durationMils / 1000f;
            CATransaction.CompletionBlock = tcs.SetResult;
            PlatformView!.Animate(native);
            CATransaction.Commit();
        }
        catch (Exception ex)
        {
            tcs.SetException(ex);
        }

        return tcs.Task;
    }

    protected virtual void MoveCamera(CameraUpdate update)
    {
        PlatformView!.MoveCamera(update.ToNative());
    }
}

public static class CameraExtensions
{
    public static VCameraPosition ToCrossPlatform(this NCameraPosition position)
    {
        return new()
        {
            Target = position.Target.ToCrossPlatformPoint(),
            Zoom = position.Zoom,
            Tilt = (float)position.ViewingAngle,
            Bearing = (float)position.Bearing
        };
    }
    public static Google.Maps.CameraUpdate ToNative(this CameraUpdate cameraUpdate)
    {
        return new CameraUpadateToNativeConverter().ToNative(cameraUpdate);
    }

    public static NCameraPosition ToNative(this VCameraPosition position)
    {
        return new(
            position.Target.ToCoord(),
            position.Zoom,
            position.Bearing,
            position.Tilt
        );
    }

    public static VisibleRegion ToCrossPlatform(this Google.Maps.VisibleRegion visibleRegion)
    {
        return new(
            new(visibleRegion.NearLeft.ToCrossPlatformPoint(), visibleRegion.FarRight.ToCrossPlatformPoint()),
            visibleRegion.FarLeft.ToCrossPlatformPoint(),
            visibleRegion.FarRight.ToCrossPlatformPoint(),
            visibleRegion.NearLeft.ToCrossPlatformPoint(),
            visibleRegion.NearRight.ToCrossPlatformPoint()
        );
    }
}