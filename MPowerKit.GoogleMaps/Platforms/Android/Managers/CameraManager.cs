using System.ComponentModel;

using GMap = Android.Gms.Maps.GoogleMap;

namespace MPowerKit.GoogleMaps;

public class CameraManager : IMapFeatureManager<GoogleMap, GMap, GoogleMapHandler>
{
    protected GoogleMap? VirtualView { get; set; }
    protected GMap? NativeView { get; set; }
    protected GoogleMapHandler? Handler { get; set; }

    public virtual void Connect(GoogleMap virtualView, GMap platformView, GoogleMapHandler handler)
    {
        VirtualView = virtualView;
        NativeView = platformView;
        Handler = handler;

        virtualView.PropertyChanged += VirtualView_PropertyChanged;
        virtualView.PropertyChanging += VirtualView_PropertyChanging;

        platformView.CameraChange += NativeMap_CameraChange;
        platformView.CameraIdle += NativeMap_CameraIdle;
        platformView.CameraMove += NativeMap_CameraMove;
        platformView.CameraMoveCanceled += NativeMap_CameraMoveCanceled;
        platformView.CameraMoveStarted += NativeMap_CameraMoveStarted;

        VirtualView.MoveCameraActionInternal = MoveCamera;
        VirtualView.AnimateCameraFuncInternal = AnimateCamera;
        VirtualView.ResetMinMaxZoomActionInternal = ResetMinMaxZoom;

        NativeView.SetMinZoomPreference(VirtualView!.MinZoom);
        NativeView.SetMaxZoomPreference(VirtualView!.MaxZoom);

        OnVisibleRegionChanged();
    }

    public virtual void Disconnect(GoogleMap virtualView, GMap platformView, GoogleMapHandler handler)
    {
        VirtualView!.MoveCameraActionInternal = null;
        VirtualView.AnimateCameraFuncInternal = null;

        platformView.CameraChange -= NativeMap_CameraChange;
        platformView.CameraIdle -= NativeMap_CameraIdle;
        platformView.CameraMove -= NativeMap_CameraMove;
        platformView.CameraMoveCanceled -= NativeMap_CameraMoveCanceled;
        platformView.CameraMoveStarted -= NativeMap_CameraMoveStarted;

        virtualView.PropertyChanged -= VirtualView_PropertyChanged;
        virtualView.PropertyChanging -= VirtualView_PropertyChanging;

        VirtualView = null;
        NativeView = null;
        Handler = null;
    }

    protected virtual void VirtualView_PropertyChanging(object sender, Microsoft.Maui.Controls.PropertyChangingEventArgs e)
    {

    }

    protected virtual void VirtualView_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == GoogleMap.MinZoomProperty.PropertyName)
        {
            NativeView!.SetMinZoomPreference(VirtualView!.MinZoom);
        }
        else if (e.PropertyName == GoogleMap.MaxZoomProperty.PropertyName)
        {
            NativeView!.SetMaxZoomPreference(VirtualView!.MaxZoom);
        }
    }

    protected virtual void NativeMap_CameraMoveStarted(object? sender, GMap.CameraMoveStartedEventArgs e)
    {
        VirtualView!.SendCameraMoveStart((CameraMoveReason)e.Reason);
    }

    protected virtual void NativeMap_CameraMoveCanceled(object? sender, EventArgs e)
    {
        VirtualView!.SendCameraMoveCanceled();
    }

    protected virtual void NativeMap_CameraMove(object? sender, EventArgs e)
    {
        VirtualView!.SendCameraMove();
    }

    protected virtual void NativeMap_CameraIdle(object? sender, EventArgs e)
    {
        OnVisibleRegionChanged();
    }

    protected virtual void OnVisibleRegionChanged()
    {
        VirtualView!.SendVisibleRegionChanged(NativeView!.Projection.VisibleRegion.ToCrossPlatform());
    }

    protected virtual void NativeMap_CameraChange(object? sender, GMap.CameraChangeEventArgs e)
    {
        VirtualView!.SendCameraChange(e.Position.ToCrossPlatform());
    }

    protected virtual void ResetMinMaxZoom()
    {
        NativeView!.ResetMinMaxZoomPreference();
    }

    protected virtual Task AnimateCamera(CameraUpdate update, int durationMils = 300)
    {
        TaskCompletionSource tcs = new();

        NativeView!.AnimateCamera(update.ToNative(Handler!.Context), durationMils, new CancelableCallback(tcs));

        return tcs.Task;
    }

    protected virtual void MoveCamera(CameraUpdate update)
    {
        NativeView!.MoveCamera(update.ToNative(Handler!.Context));
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