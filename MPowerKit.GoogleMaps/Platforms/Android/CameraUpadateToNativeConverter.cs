using Android.Content;

using Microsoft.Maui.Platform;

using VCameraUpdate = MPowerKit.GoogleMaps.CameraUpdate;
using NCameraUpdate = Android.Gms.Maps.CameraUpdate;
using NCameraUpdateFactory = Android.Gms.Maps.CameraUpdateFactory;

namespace MPowerKit.GoogleMaps;

public class CameraUpadateToNativeConverter
{
    public static Dictionary<Type, Func<VCameraUpdate, Context, NCameraUpdate>> CameraUpdateMapper = new()
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

    public virtual NCameraUpdate ToNative(VCameraUpdate cameraUpdate, Context context)
    {
        if (!CameraUpdateMapper.ContainsKey(cameraUpdate.GetType()))
            throw new NotSupportedException($"CameraUpdate type {cameraUpdate.GetType()} is not registered in the mapper");

        var func = CameraUpdateMapper[cameraUpdate.GetType()];

        return func(cameraUpdate, context);
    }

    public static NCameraUpdate ZoomInCameraUpdate(VCameraUpdate cameraUpdate, Context context)
    {
        return NCameraUpdateFactory.ZoomIn();
    }

    public static NCameraUpdate ZoomOutCameraUpdate(VCameraUpdate cameraUpdate, Context context)
    {
        return NCameraUpdateFactory.ZoomOut();
    }

    public static NCameraUpdate ZoomToCameraUpdate(VCameraUpdate cameraUpdate, Context context)
    {
        var zoomTo = (ZoomToCameraUpdate)cameraUpdate;
        return NCameraUpdateFactory.ZoomTo(zoomTo.Zoom);
    }

    public static NCameraUpdate ZoomByCameraUpdate(VCameraUpdate cameraUpdate, Context context)
    {
        var zoomBy = (ZoomByCameraUpdate)cameraUpdate;
        return NCameraUpdateFactory.ZoomBy(zoomBy.Amount);
    }

    public static NCameraUpdate ZoomByAndFocusCameraUpdate(VCameraUpdate cameraUpdate, Context context)
    {
        var zoomByAndFocus = (ZoomByAndFocusCameraUpdate)cameraUpdate;
        return NCameraUpdateFactory.ZoomBy(zoomByAndFocus.Amount, zoomByAndFocus.Focus.ToNativePoint(context));
    }

    public static NCameraUpdate ScrollByCameraUpdate(VCameraUpdate cameraUpdate, Context context)
    {
        var scrollBy = (ScrollByCameraUpdate)cameraUpdate;
        return NCameraUpdateFactory.ScrollBy(scrollBy.Dx, scrollBy.Dy);
    }

    public static NCameraUpdate NewCameraPositionCameraUpdate(VCameraUpdate cameraUpdate, Context context)
    {
        var newCameraPosition = (NewCameraPositionCameraUpdate)cameraUpdate;
        return NCameraUpdateFactory.NewCameraPosition(newCameraPosition.CameraPosition.ToNative());
    }

    public static NCameraUpdate NewLatLngZoomCameraUpdate(VCameraUpdate cameraUpdate, Context context)
    {
        var newLatLngZoom = (NewLatLngZoomCameraUpdate)cameraUpdate;
        return NCameraUpdateFactory.NewLatLngZoom(newLatLngZoom.LatLng.ToLatLng(), newLatLngZoom.Zoom);
    }

    public static NCameraUpdate NewLatLngCameraUpdate(VCameraUpdate cameraUpdate, Context context)
    {
        var newLatLng = (NewLatLngCameraUpdate)cameraUpdate;
        return NCameraUpdateFactory.NewLatLng(newLatLng.LatLng.ToLatLng());
    }

    public static NCameraUpdate NewLatLngBoundsSizeCameraUpdate(VCameraUpdate cameraUpdate, Context context)
    {
        var newLatLngBoundsSize = (NewLatLngBoundsSizeCameraUpdate)cameraUpdate;
        return NCameraUpdateFactory.NewLatLngBounds(newLatLngBoundsSize.Bounds.ToNative(), (int)context.ToPixels(newLatLngBoundsSize.Size.Width), (int)context.ToPixels(newLatLngBoundsSize.Size.Height), (int)context.ToPixels(newLatLngBoundsSize.Padding));
    }

    public static NCameraUpdate NewLatLngBoundsCameraUpdate(VCameraUpdate cameraUpdate, Context context)
    {
        var newLatLngBounds = (NewLatLngBoundsCameraUpdate)cameraUpdate;
        return NCameraUpdateFactory.NewLatLngBounds(newLatLngBounds.Bounds.ToNative(), (int)context.ToPixels(newLatLngBounds.Padding));
    }
}