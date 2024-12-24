using Android.Content;

using GMap = Android.Gms.Maps.GoogleMap;
using NCameraPosition = Android.Gms.Maps.Model.CameraPosition;
using NCameraUpdate = Android.Gms.Maps.CameraUpdate;
using VCameraPosition = MPowerKit.GoogleMaps.CameraPosition;
using VCameraUpdate = MPowerKit.GoogleMaps.CameraUpdate;

namespace MPowerKit.GoogleMaps;

public class CameraManager : MapFeatureManager<GoogleMap, GMap, GoogleMapHandler>
{
    protected override void Init(GoogleMap virtualView, GMap platformView, GoogleMapHandler handler)
    {
        base.Init(virtualView, platformView, handler);

        OnMinZoomChanged(virtualView, platformView);
        OnMaxZoomChanged(virtualView, platformView);

        if (virtualView.RestrictPanningToArea is not null)
        {
            OnRestrictPanningToAreaChanged(virtualView, platformView);
            return;
        }

        if (virtualView.InitialCameraPosition is not null)
        {
            platformView.MoveCamera(virtualView.InitialCameraPosition.ToNative(handler.Context));
            return;
        }

        virtualView.SendCameraChange(platformView.CameraPosition.ToCrossPlatform(), false);
        virtualView.SendCameraIdle(platformView.Projection.VisibleRegion.ToCrossPlatform(), false);
    }

    protected override void SubscribeToEvents(GoogleMap virtualView, GMap platformView, GoogleMapHandler handler)
    {
        base.SubscribeToEvents(virtualView, platformView, handler);

        platformView.CameraChange += PlatformView_CameraChange;
        platformView.CameraIdle += PlatformView_CameraIdle;
        platformView.CameraMove += PlatformView_CameraMove;
        platformView.CameraMoveCanceled += PlatformView_CameraMoveCanceled;
        platformView.CameraMoveStarted += PlatformView_CameraMoveStarted;

        virtualView.MoveCameraActionInternal = MoveCamera;
        virtualView.AnimateCameraFuncInternal = AnimateCamera;
        virtualView.ResetMinMaxZoomActionInternal = ResetMinMaxZoom;
    }

    protected override void UnsubscribeFromEvents(GoogleMap virtualView, GMap platformView, GoogleMapHandler handler)
    {
        virtualView.MoveCameraActionInternal = null;
        virtualView.AnimateCameraFuncInternal = null;
        virtualView.ResetMinMaxZoomActionInternal = null;

        platformView.CameraChange -= PlatformView_CameraChange;
        platformView.CameraIdle -= PlatformView_CameraIdle;
        platformView.CameraMove -= PlatformView_CameraMove;
        platformView.CameraMoveCanceled -= PlatformView_CameraMoveCanceled;
        platformView.CameraMoveStarted -= PlatformView_CameraMoveStarted;

        base.UnsubscribeFromEvents(virtualView, platformView, handler);
    }

    protected override void VirtualViewPropertyChanged(GoogleMap virtualView, GMap platformView, string? propertyName)
    {
        base.VirtualViewPropertyChanged(virtualView, platformView, propertyName);

        if (propertyName == GoogleMap.MinZoomProperty.PropertyName)
        {
            OnMinZoomChanged(virtualView, platformView);
        }
        else if (propertyName == GoogleMap.MaxZoomProperty.PropertyName)
        {
            OnMaxZoomChanged(virtualView, platformView);
        }
        else if (propertyName == GoogleMap.RestrictPanningToAreaProperty.PropertyName)
        {
            OnRestrictPanningToAreaChanged(virtualView, platformView);
        }
    }

    protected virtual void OnMinZoomChanged(GoogleMap virtualView, GMap platformView)
    {
        platformView.SetMinZoomPreference(virtualView.MinZoom);
    }

    protected virtual void OnMaxZoomChanged(GoogleMap virtualView, GMap platformView)
    {
        platformView.SetMaxZoomPreference(virtualView.MinZoom);
    }

    protected virtual void OnRestrictPanningToAreaChanged(GoogleMap virtualView, GMap platformView)
    {
        platformView.SetLatLngBoundsForCameraTarget(virtualView.RestrictPanningToArea?.ToNative());
    }

    protected virtual void PlatformView_CameraMoveStarted(object? sender, GMap.CameraMoveStartedEventArgs e)
    {
        VirtualView!.SendCameraMoveStart((CameraMoveReason)e.Reason);
    }

    protected virtual void PlatformView_CameraMoveCanceled(object? sender, EventArgs e)
    {
        VirtualView!.SendCameraMoveCanceled();
    }

    protected virtual void PlatformView_CameraMove(object? sender, EventArgs e)
    {
        VirtualView!.SendCameraMove();
    }

    protected virtual void PlatformView_CameraIdle(object? sender, EventArgs e)
    {
        VirtualView!.SendCameraIdle(PlatformView!.Projection.VisibleRegion.ToCrossPlatform());
    }

    protected virtual void PlatformView_CameraChange(object? sender, GMap.CameraChangeEventArgs e)
    {
        VirtualView!.SendCameraChange(e.Position.ToCrossPlatform());
    }

    protected virtual void ResetMinMaxZoom()
    {
        PlatformView!.ResetMinMaxZoomPreference();
        VirtualView!.MinZoom = PlatformView.MinZoomLevel;
        VirtualView!.MaxZoom = PlatformView.MaxZoomLevel;
    }

    protected virtual Task AnimateCamera(VCameraUpdate update, int durationMils = 300)
    {
        TaskCompletionSource tcs = new();

        PlatformView!.AnimateCamera(update.ToNative(Handler!.Context), durationMils, new CancelableCallback(tcs));

        return tcs.Task;
    }

    protected virtual void MoveCamera(VCameraUpdate update)
    {
        PlatformView!.MoveCamera(update.ToNative(Handler!.Context));
    }

    public class CancelableCallback : Java.Lang.Object, GMap.ICancelableCallback
    {
        private readonly TaskCompletionSource _tcs;

        public CancelableCallback(TaskCompletionSource tcs)
        {
            _tcs = tcs;
        }

        public void OnCancel()
        {
            _tcs.SetCanceled();
        }

        public void OnFinish()
        {
            _tcs.SetResult();
        }
    }
}

public static class CameraExtensions
{
    public static VCameraPosition ToCrossPlatform(this NCameraPosition position)
    {
        return new
        (
            position.Target.ToCrossPlatformPoint(),
            position.Zoom,
            position.Bearing,
            position.Tilt
        );
    }

    public static NCameraUpdate ToNative(this VCameraUpdate cameraUpdate, Context context)
    {
        return new CameraUpadateToNativeConverter().ToNative(cameraUpdate, context);
    }

    public static NCameraPosition ToNative(this VCameraPosition position)
    {
        return new(
            position.Target.ToLatLng(),
            position.Zoom,
            position.Tilt,
            position.Bearing
        );
    }

    public static VisibleRegion ToCrossPlatform(this Android.Gms.Maps.Model.VisibleRegion visibleRegion)
    {
        return new(
            visibleRegion.LatLngBounds.ToCrossPlatform(),
            visibleRegion.FarLeft.ToCrossPlatformPoint(),
            visibleRegion.FarRight.ToCrossPlatformPoint(),
            visibleRegion.NearLeft.ToCrossPlatformPoint(),
            visibleRegion.NearRight.ToCrossPlatformPoint()
        );
    }
}