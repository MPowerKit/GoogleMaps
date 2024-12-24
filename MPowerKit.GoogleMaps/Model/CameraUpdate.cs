namespace MPowerKit.GoogleMaps;

public abstract record CameraUpdate;

public record ZoomInCameraUpdate : CameraUpdate;

public record ZoomOutCameraUpdate : CameraUpdate;

public record ZoomToCameraUpdate(float Zoom) : CameraUpdate;

public record ZoomByCameraUpdate(float Amount) : CameraUpdate;

public record ZoomByAndFocusCameraUpdate(float Amount, Point Focus) : ZoomByCameraUpdate(Amount);

public record ScrollByCameraUpdate(float Dx, float Dy) : CameraUpdate;

public record NewCameraPositionCameraUpdate(CameraPosition CameraPosition) : CameraUpdate;

public record NewLatLngCameraUpdate(Point LatLng) : CameraUpdate;

public record NewLatLngZoomCameraUpdate(Point LatLng, float Zoom) : NewLatLngCameraUpdate(LatLng);

public record NewLatLngBoundsCameraUpdate(LatLngBounds Bounds, double Padding) : CameraUpdate;

public record NewLatLngBoundsSizeCameraUpdate(LatLngBounds Bounds, double Padding, Size Size)
    : NewLatLngBoundsCameraUpdate(Bounds, Padding);

public static class CameraUpdateFactory
{
    public static CameraUpdate ZoomIn()
    {
        return new ZoomInCameraUpdate();
    }

    public static CameraUpdate ZoomOut()
    {
        return new ZoomOutCameraUpdate();
    }

    public static CameraUpdate ZoomTo(float zoom)
    {
        return new ZoomToCameraUpdate(zoom);
    }

    public static CameraUpdate ZoomBy(float zoomDelta)
    {
        return new ZoomByCameraUpdate(zoomDelta);
    }

    public static CameraUpdate ZoomBy(float zoomDelta, Point focusPointOnScreen)
    {
        return new ZoomByAndFocusCameraUpdate(zoomDelta, focusPointOnScreen);
    }

    public static CameraUpdate ScrollBy(float dxPixels, float dyPixels)
    {
        return new ScrollByCameraUpdate(dxPixels, dyPixels);
    }

    public static CameraUpdate NewCameraPosition(CameraPosition cameraPosition)
    {
        return new NewCameraPositionCameraUpdate(cameraPosition);
    }

    public static CameraUpdate NewLatLng(Point latLng)
    {
        return new NewLatLngCameraUpdate(latLng);
    }

    public static CameraUpdate NewLatLngZoom(Point latLng, float zoom)
    {
        return new NewLatLngZoomCameraUpdate(latLng, zoom);
    }

    public static CameraUpdate NewLatLngBounds(LatLngBounds bounds, double padding)
    {
        return new NewLatLngBoundsCameraUpdate(bounds, padding);
    }

    public static CameraUpdate NewLatLngBounds(LatLngBounds bounds, double padding, Size size)
    {
        return new NewLatLngBoundsSizeCameraUpdate(bounds, padding, size);
    }

    public static CameraUpdate FromCenterAndRadius(Point center, double radiusMeters)
    {
        var latOffset = Distance.ToLatitudeDegrees(radiusMeters);
        var lngOffset = Distance.ToLongitudeDegrees(radiusMeters, center.X);

        Point southWest = new(center.X - latOffset, center.Y - lngOffset);
        Point northEast = new(center.X + latOffset, center.Y + lngOffset);

        return NewLatLngBounds(new(southWest, northEast), 0);
    }
}