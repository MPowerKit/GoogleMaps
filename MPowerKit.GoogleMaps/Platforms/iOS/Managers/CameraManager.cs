using System.ComponentModel;

using CoreAnimation;

using Google.Maps;

using NCameraPosition = Google.Maps.CameraPosition;
using VCameraPosition = MPowerKit.GoogleMaps.CameraPosition;

namespace MPowerKit.GoogleMaps;

public class CameraManager : IMapFeatureManager<GoogleMap, MapView, GoogleMapHandler>
{
    protected GoogleMap? VirtualView { get; set; }
    protected MapView? NativeView { get; set; }
    protected GoogleMapHandler? Handler { get; set; }

    public virtual void Connect(GoogleMap virtualView, MapView platformView, GoogleMapHandler handler)
    {
        VirtualView = virtualView;
        NativeView = platformView;
        Handler = handler;

        virtualView.PropertyChanged += VirtualView_PropertyChanged;
        virtualView.PropertyChanging += VirtualView_PropertyChanging;

        platformView.CameraPositionChanged += PlatformView_CameraPositionChanged;
        platformView.CameraPositionIdle += PlatformView_CameraPositionIdle;
        platformView.WillMove += PlatformView_WillMove;

        virtualView.MoveCameraActionInternal = MoveCamera;
        virtualView.AnimateCameraFuncInternal = AnimateCamera;

        InitCamera();
    }

    public virtual void Disconnect(GoogleMap virtualView, MapView platformView, GoogleMapHandler handler)
    {
        virtualView.MoveCameraActionInternal = null;
        virtualView.AnimateCameraFuncInternal = null;

        platformView.CameraPositionChanged -= PlatformView_CameraPositionChanged;
        platformView.CameraPositionIdle -= PlatformView_CameraPositionIdle;
        platformView.WillMove -= PlatformView_WillMove;

        virtualView.PropertyChanged -= VirtualView_PropertyChanged;
        virtualView.PropertyChanging -= VirtualView_PropertyChanging;

        VirtualView = null;
        NativeView = null;
        Handler = null;
    }

    protected virtual void InitCamera()
    {
        NativeView!.SetMinMaxZoom(VirtualView!.MinZoom, VirtualView.MaxZoom);

        if (VirtualView.RestrictPanningToArea is not null)
        {
            NativeView.CameraTargetBounds = VirtualView.RestrictPanningToArea?.ToNative();
            return;
        }

        if (VirtualView.InitialCameraPosition is not null)
        {
            NativeView.MoveCamera(VirtualView.InitialCameraPosition.ToNative());
            return;
        }

        VirtualView.SendCameraChange(NativeView.Camera.ToCrossPlatform(), false);
        VirtualView.SendCameraIdle(NativeView!.Projection.VisibleRegion.ToCrossPlatform(), false);
    }

    protected virtual void VirtualView_PropertyChanging(object sender, Microsoft.Maui.Controls.PropertyChangingEventArgs e)
    {

    }

    protected virtual void VirtualView_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == GoogleMap.MinZoomProperty.PropertyName
            || e.PropertyName == GoogleMap.MaxZoomProperty.PropertyName)
        {
            NativeView!.SetMinMaxZoom(VirtualView!.MinZoom, VirtualView!.MaxZoom);
        }
        else if (e.PropertyName == GoogleMap.RestrictPanningToAreaProperty.PropertyName)
        {
            NativeView!.CameraTargetBounds = VirtualView!.RestrictPanningToArea?.ToNative();
        }
    }

    protected virtual void PlatformView_CameraPositionChanged(object? sender, GMSCameraEventArgs e)
    {
        VirtualView!.SendCameraChange(e.Position.ToCrossPlatform());
        VirtualView!.SendCameraMove();
    }

    protected virtual void PlatformView_CameraPositionIdle(object? sender, GMSCameraEventArgs e)
    {
        VirtualView!.SendCameraChange(e.Position.ToCrossPlatform());
        VirtualView!.SendCameraIdle(NativeView!.Projection.VisibleRegion.ToCrossPlatform());
    }

    protected virtual void PlatformView_WillMove(object? sender, GMSWillMoveEventArgs e)
    {
        VirtualView!.SendCameraMoveStart(e.Gesture ? CameraMoveReason.Gesture : CameraMoveReason.ApiAnimation);
    }

    protected virtual void NativeMap_CameraMoveCanceled(object? sender, EventArgs e)
    {
        VirtualView!.SendCameraMoveCanceled();
    }

    protected virtual void NativeMap_CameraMove(object? sender, EventArgs e)
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
            NativeView!.Animate(native);
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
        NativeView!.MoveCamera(update.ToNative());
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