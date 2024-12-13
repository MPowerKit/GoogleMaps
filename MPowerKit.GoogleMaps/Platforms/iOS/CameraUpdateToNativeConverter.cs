using NCameraUpdate = Google.Maps.CameraUpdate;
using VCameraUpdate = MPowerKit.GoogleMaps.CameraUpdate;

namespace MPowerKit.GoogleMaps;

public class CameraUpadateToNativeConverter
{
    public static Dictionary<Type, Func<VCameraUpdate, NCameraUpdate>> CameraUpdateMapper = new()
    {
        { typeof(ZoomInCameraUpdate), ZoomInCameraUpdate },
        { typeof(ZoomOutCameraUpdate), ZoomOutCameraUpdate },
        { typeof(ZoomToCameraUpdate), ZoomToCameraUpdate },
        { typeof(ZoomByCameraUpdate), ZoomByCameraUpdate },
        { typeof(ZoomByAndFocusCameraUpdate), ZoomByAndFocusCameraUpdate },
        { typeof(ScrollByCameraUpdate), ScrollByCameraUpdate },
        { typeof(NewCameraPositionCameraUpdate), NewCameraPositionCameraUpdate },
        { typeof(NewLatLngZoomCameraUpdate), NewLatLngZoomCameraUpdate },
        { typeof(NewLatLngCameraUpdate), NewLatLngCameraUpdate },
        { typeof(NewLatLngBoundsSizeCameraUpdate), NewLatLngBoundsSizeCameraUpdate },
        { typeof(NewLatLngBoundsCameraUpdate), NewLatLngBoundsCameraUpdate }
    };

    public virtual NCameraUpdate ToNative(VCameraUpdate cameraUpdate)
    {
        if (!CameraUpdateMapper.ContainsKey(cameraUpdate.GetType()))
            throw new NotSupportedException($"CameraUpdate type {cameraUpdate.GetType()} is not registered in the mapper");

        var func = CameraUpdateMapper[cameraUpdate.GetType()];

        return func(cameraUpdate);
    }

    public static NCameraUpdate ZoomInCameraUpdate(VCameraUpdate cameraUpdate)
    {
        return NCameraUpdate.ZoomIn;
    }

    public static NCameraUpdate ZoomOutCameraUpdate(VCameraUpdate cameraUpdate)
    {
        return NCameraUpdate.ZoomOut;
    }

    public static NCameraUpdate ZoomToCameraUpdate(VCameraUpdate cameraUpdate)
    {
        var zoomTo = (ZoomToCameraUpdate)cameraUpdate;

        return NCameraUpdate.ZoomToZoom(zoomTo.Zoom);
    }

    public static NCameraUpdate ZoomByCameraUpdate(VCameraUpdate cameraUpdate)
    {
        var zoomBy = (ZoomByCameraUpdate)cameraUpdate;
        return NCameraUpdate.ZoomByDelta(zoomBy.Amount);
    }

    public static NCameraUpdate ZoomByAndFocusCameraUpdate(VCameraUpdate cameraUpdate)
    {
        var zoomByAndFocus = (ZoomByAndFocusCameraUpdate)cameraUpdate;
        return NCameraUpdate.ZoomByZoom(zoomByAndFocus.Amount, zoomByAndFocus.Focus);
    }

    public static NCameraUpdate ScrollByCameraUpdate(VCameraUpdate cameraUpdate)
    {
        var scrollBy = (ScrollByCameraUpdate)cameraUpdate;
        return NCameraUpdate.Scroll(scrollBy.Dx, scrollBy.Dy);
    }

    public static NCameraUpdate NewCameraPositionCameraUpdate(VCameraUpdate cameraUpdate)
    {
        var newCameraPosition = (NewCameraPositionCameraUpdate)cameraUpdate;
        return NCameraUpdate.SetCamera(newCameraPosition.CameraPosition.ToNative());
    }

    public static NCameraUpdate NewLatLngZoomCameraUpdate(VCameraUpdate cameraUpdate)
    {
        var newLatLngZoom = (NewLatLngZoomCameraUpdate)cameraUpdate;
        return NCameraUpdate.SetTarget(newLatLngZoom.LatLng.ToCoord(), newLatLngZoom.Zoom);
    }

    public static NCameraUpdate NewLatLngCameraUpdate(VCameraUpdate cameraUpdate)
    {
        var newLatLng = (NewLatLngCameraUpdate)cameraUpdate;
        return NCameraUpdate.SetTarget(newLatLng.LatLng.ToCoord());
    }

    public static NCameraUpdate NewLatLngBoundsSizeCameraUpdate(VCameraUpdate cameraUpdate)
    {
        throw new NotImplementedException("NewLatLngBoundsSizeCameraUpdate is not implemented in GoogleMaps iOS SDK");
    }

    public static NCameraUpdate NewLatLngBoundsCameraUpdate(VCameraUpdate cameraUpdate)
    {
        var newLatLngBounds = (NewLatLngBoundsCameraUpdate)cameraUpdate;
        return NCameraUpdate.FitBounds(newLatLngBounds.Bounds.ToNative(), (float)newLatLngBounds.Padding);
    }
}