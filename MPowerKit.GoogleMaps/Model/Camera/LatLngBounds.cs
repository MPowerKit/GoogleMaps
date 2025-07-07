namespace MPowerKit.GoogleMaps;

public record struct LatLngBounds(Point SouthWest, Point NorthEast)
{
    public readonly IEnumerable<Point> ToPoints()
    {
        Point southwest = SouthWest;
        Point northeast = NorthEast;

        // Calculate the other two corners
        Point northwest = new(NorthEast.X, SouthWest.Y);
        Point southeast = new(SouthWest.X, NorthEast.Y);

        return [northwest, northeast, southeast, southwest];
    }

    public readonly bool Contains(Point point)
    {
        // Check if the point's latitude is between the bounds' latitudes
        bool isLatInBounds = point.X >= SouthWest.X && point.X <= NorthEast.X;

        // Check if the point's longitude is between the bounds' longitudes
        bool isLngInBounds = point.Y >= SouthWest.Y && point.Y <= NorthEast.Y;

        // Return true if both latitude and longitude are within bounds
        return isLatInBounds && isLngInBounds;
    }

    public override readonly string ToString()
    {
        return $"SouthWest={{Lat={SouthWest.X:F2}, Lon={SouthWest.Y:F2}}}, NorthEast={{Lat={NorthEast.X:F2}, Lon={NorthEast.Y:F2}}}";
    }

    public class Builder
    {
        private double _minLat = 90d;
        private double _minLng = 180d;
        private double _maxLat = -90d;
        private double _maxLng = -180d;

        // Include a single LatLng point into the bounds
        public Builder Include(Point point)
        {
            // Validate latitude bounds
            if (point.X < -90d || point.X > 90d)
            {
                throw new ArgumentOutOfRangeException(nameof(point), "Latitude must be between -90 and 90 degrees.");
            }

            // Validate longitude bounds
            if (point.Y < -180d || point.Y > 180d)
            {
                throw new ArgumentOutOfRangeException(nameof(point), "Longitude must be between -180 and 180 degrees.");
            }

            // Update the minimum and maximum latitude and longitude
            if (point.X < _minLat)
            {
                _minLat = point.X;
            }
            if (point.X > _maxLat)
            {
                _maxLat = point.X;
            }
            if (point.Y < _minLng)
            {
                _minLng = point.Y;
            }
            if (point.Y > _maxLng)
            {
                _maxLng = point.Y;
            }

            return this;
        }

        // Include a collection of LatLng points into the bounds
        public Builder Include(IEnumerable<Point> points)
        {
            // Iterate over each point and include it
            foreach (var point in points)
            {
                Include(point);
            }

            return this;
        }

        // Build and return the LatLngBounds object
        public LatLngBounds Build()
        {
            if (_minLat > _maxLat)
            {
                throw new InvalidOperationException("Bounds cannot be built without including at least one point.");
            }

            // Handle longitude wrapping to minimize the span
            var minLongitude = _minLng;
            var maxLongitude = _maxLng;

            // Calculate the difference between the longitudes directly
            var longitudeDifference = maxLongitude - minLongitude;

            // If the difference is greater than 180 degrees, adjust the bounds
            if (longitudeDifference > 180)
            {
                // Swap the longitudes to wrap around the 180 meridian for minimal bounding
                (maxLongitude, minLongitude) = (minLongitude, maxLongitude);
            }

            Point southwest = new(_minLat, minLongitude);
            Point northeast = new(_maxLat, maxLongitude);

            return new(southwest, northeast);
        }
    }
}