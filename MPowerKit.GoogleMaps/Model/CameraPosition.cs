﻿namespace MPowerKit.GoogleMaps;

public class CameraPosition
{
    public Point Target { get; set; }
    public float Zoom { get; set; }
    public float Bearing { get; set; }
    public float Tilt { get; set; }

    public override string ToString()
    {
        return $"{{Lat={Target.X:F2}, Lon={Target.Y:F2}}}, Zoom={Zoom:F1}, Bearing={Bearing:F1}, Tilt={Tilt:F1}";
    }

    public class Builder
    {
        private readonly CameraPosition _cameraPosition = new();

        public CameraPosition Build()
        {
            return _cameraPosition;
        }

        public Builder Target(Point target)
        {
            _cameraPosition.Target = target;
            return this;
        }

        public Builder Zoom(float zoom)
        {
            _cameraPosition.Zoom = zoom;
            return this;
        }

        public Builder Bearing(float bearing)
        {
            _cameraPosition.Bearing = bearing;
            return this;
        }

        public Builder Tilt(float tilt)
        {
            _cameraPosition.Tilt = tilt;
            return this;
        }
    }
}