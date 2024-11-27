namespace MPowerKit.GoogleMaps;

public abstract class CameraUpdate { }

public class ZoomInCameraUpdate : CameraUpdate { }

public class ZoomOutCameraUpdate : CameraUpdate { }

public class ZoomToCameraUpdate : CameraUpdate
{
    public float Zoom { get; set; }
}

public class ZoomByCameraUpdate : CameraUpdate
{
    public float Amount { get; set; }
}

public class ZoomByAndFocusCameraUpdate : ZoomByCameraUpdate
{
    public Point Focus { get; set; }
}

public class ScrollByCameraUpdate : CameraUpdate
{
    public float Dx { get; set; }
    public float Dy { get; set; }
}

public class NewCameraPositionCameraUpdate : CameraUpdate
{
    public required CameraPosition CameraPosition { get; set; }
}

public class NewLatLngCameraUpdate : CameraUpdate
{
    public Point LatLng { get; set; }
}

public class NewLatLngZoomCameraUpdate : NewLatLngCameraUpdate
{
    public float Zoom { get; set; }
}

public class NewLatLngBoundsCameraUpdate : CameraUpdate
{
    public LatLngBounds Bounds { get; set; }
    public double Padding { get; set; }
}

public class NewLatLngBoundsSizeCameraUpdate : NewLatLngBoundsCameraUpdate
{
    public Size Size { get; set; }
}

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
        return new ZoomToCameraUpdate() { Zoom = zoom };
    }

    public static CameraUpdate ZoomBy(float amount)
    {
        return new ZoomByCameraUpdate() { Amount = amount };
    }

    public static CameraUpdate ZoomBy(float amount, Point focusPointOnScreen)
    {
        return new ZoomByAndFocusCameraUpdate() { Amount = amount, Focus = focusPointOnScreen };
    }

    public static CameraUpdate ScrollBy(float dxPixels, float dyPixels)
    {
        return new ScrollByCameraUpdate() { Dx = dxPixels, Dy = dyPixels };
    }

    public static CameraUpdate NewCameraPosition(CameraPosition cameraPosition)
    {
        return new NewCameraPositionCameraUpdate() { CameraPosition = cameraPosition };
    }

    public static CameraUpdate NewLatLng(Point latLng)
    {
        return new NewLatLngCameraUpdate() { LatLng = latLng };
    }

    public static CameraUpdate NewLatLngZoom(Point latLng, float zoom)
    {
        return new NewLatLngZoomCameraUpdate() { LatLng = latLng, Zoom = zoom };
    }

    public static CameraUpdate NewLatLngBounds(LatLngBounds bounds, double padding)
    {
        return new NewLatLngBoundsCameraUpdate() { Bounds = bounds, Padding = padding };
    }

    public static CameraUpdate NewLatLngBounds(LatLngBounds bounds, double padding, Size size)
    {
        return new NewLatLngBoundsSizeCameraUpdate() { Bounds = bounds, Padding = padding, Size = size };
    }

    public static CameraUpdate FromCenterAndRadius(Point center, double radiusMeters)
    {
        var latOffset = Distance.ToLattitudeDegrees(radiusMeters);
        var lngOffset = Distance.ToLongitudeDegrees(radiusMeters, center.X);

        Point southWest = new(center.X - latOffset, center.Y - lngOffset);
        Point northEast = new(center.X + latOffset, center.Y + lngOffset);

        return NewLatLngBounds(new(southWest, northEast), 0);
    }
}