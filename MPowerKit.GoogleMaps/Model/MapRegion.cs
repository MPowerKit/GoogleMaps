namespace MPowerKit.GoogleMaps;

public record struct MapRegion(LatLngBounds Bounds, Point FarLeft, Point FarRight, Point NearLeft, Point NearRight);