using System.Globalization;
using System.Xml;

namespace MPowerKit.GoogleMaps.Data;

/// <summary>
/// Parses the feature of a given KML file into a KmlPlacemark or KmlGroundOverlay object.
/// </summary>
public static partial class KmlFeatureParser
{
    private const int LongitudeIndex = 0;
    private const int LatitudeIndex = 1;

    /// <summary>
    /// Creates a new Placemark object.
    /// </summary>
    public static KmlPlacemark CreatePlacemark(XmlReader reader)
    {
        string? styleId = null;
        KmlStyle? inlineStyle = null;
        Dictionary<string, string> properties = [];
        IGeometry? geometry = null;

        while (reader.NodeType != XmlNodeType.EndElement || reader.Name != Constants.Placemark)
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                switch (reader.Name)
                {
                    case Constants.StyleUrl:
                        styleId = reader.ReadElementContentAsString();
                        break;
                    case var _ when Constants.GetGeometryRegex().IsMatch(reader.Name):
                        geometry = CreateGeometry(reader, reader.Name);
                        break;
                    case var _ when Constants.GetPropertyRegex().IsMatch(reader.Name):
                        properties[reader.Name] = reader.ReadElementContentAsString();
                        break;
                    case Constants.ExtendedData:
                        foreach (var kvp in SetExtendedDataProperties(reader))
                        {
                            properties[kvp.Key] = kvp.Value;
                        }
                        break;
                    case Constants.Style:
                        inlineStyle = KmlStyleParser.CreateStyle(reader);
                        break;
                }
            }

            if (!reader.Read()) break;
        }

        return new(geometry, styleId, inlineStyle, properties);
    }

    /// <summary>
    /// Creates a new GroundOverlay object.
    /// </summary>
    public static KmlGroundOverlay CreateGroundOverlay(XmlReader reader)
    {
        var drawOrder = 0f;
        var rotation = 0f;
        var visibility = 1;
        string? imageUrl = null;
        Dictionary<string, string> properties = [];
        Dictionary<string, double> compassPoints = [];

        while (reader.NodeType != XmlNodeType.EndElement || reader.Name != Constants.GroundOverlay)
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                switch (reader.Name)
                {
                    case Constants.Icon:
                        imageUrl = GetImageUrl(reader);
                        break;
                    case Constants.DrawOrder:
                        drawOrder = reader.ReadElementContentAsFloat();
                        break;
                    case Constants.Visibility:
                        visibility = reader.ReadElementContentAsInt();
                        break;
                    case Constants.ExtendedData:
                        foreach (var kvp in SetExtendedDataProperties(reader))
                        {
                            properties[kvp.Key] = kvp.Value;
                        }
                        break;
                    case Constants.Rotation:
                        rotation = GetRotation(reader);
                        break;
                    case var _ when Constants.GetPropertyRegex().IsMatch(reader.Name) || reader.Name == Constants.Color:
                        properties[reader.Name] = reader.ReadElementContentAsString();
                        break;
                    case var _ when Constants.GetCompassRegex().IsMatch(reader.Name):
                        compassPoints[reader.Name] = reader.ReadElementContentAsDouble();
                        break;
                }
            }

            if (!reader.Read()) break;
        }

        var latLonBox = CreateLatLngBounds(
            compassPoints.GetValueOrDefault("north"),
            compassPoints.GetValueOrDefault("south"),
            compassPoints.GetValueOrDefault("east"),
            compassPoints.GetValueOrDefault("west")
        );

        return new(imageUrl, latLonBox, drawOrder, visibility, properties, rotation);
    }

    private static float GetRotation(XmlReader reader)
    {
        return -reader.ReadElementContentAsFloat();
    }

    private static string? GetImageUrl(XmlReader reader)
    {
        while (reader.NodeType != XmlNodeType.EndElement || reader.Name != Constants.Icon)
        {
            if (reader.NodeType == XmlNodeType.Element && reader.Name == Constants.Href)
                return reader.ReadElementContentAsString();

            if (!reader.Read()) break;
        }

        return null;
    }

    private static IGeometry? CreateGeometry(XmlReader reader, string geometryType)
    {
        while (reader.NodeType != XmlNodeType.EndElement || reader.Name != geometryType)
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                switch (reader.Name)
                {
                    case Constants.Point:
                        return CreatePoint(reader);
                    case Constants.LineString:
                        return CreateLineString(reader);
                    case Constants.Polygon:
                        return CreatePolygon(reader);
                    case Constants.MultiGeometry:
                        return CreateMultiGeometry(reader);
                    case Constants.Track:
                        return CreateTrack(reader);
                    case Constants.MultiTrack:
                        return CreateMultiTrack(reader);
                }
            }

            if (!reader.Read()) break;
        }

        return null;
    }

    private static Dictionary<string, string> SetExtendedDataProperties(XmlReader reader)
    {
        Dictionary<string, string> properties = [];
        string? propertyKey = null;

        while (reader.NodeType != XmlNodeType.EndElement || reader.Name != Constants.ExtendedData)
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                if (reader.Name == Constants.Data)
                    propertyKey = reader.GetAttribute(Constants.Name);
                else if (reader.Name == Constants.Value && propertyKey is not null)
                {
                    properties[propertyKey] = reader.ReadElementContentAsString();
                    propertyKey = null;
                }
            }

            if (!reader.Read()) break;
        }

        return properties;
    }

    private static PointGeometry CreatePoint(XmlReader reader)
    {
        Point point = default;

        while (reader.NodeType != XmlNodeType.EndElement || reader.Name != Constants.Point)
        {
            if (reader.NodeType == XmlNodeType.Element && reader.Name == Constants.Coordinates)
                point = ConvertToLatLng(reader.ReadElementContentAsString());

            if (!reader.Read()) break;
        }

        return new(point);
    }

    private static LineString CreateLineString(XmlReader reader)
    {
        List<Point> coordinates = [];

        while (reader.NodeType != XmlNodeType.EndElement || reader.Name != Constants.LineString)
        {
            if (reader.NodeType == XmlNodeType.Element && reader.Name == Constants.Coordinates)
            {
                var points = ConvertToLatLngArray(reader.ReadElementContentAsString());
                foreach (var point in points)
                {
                    coordinates.Add(point);
                }
            }

            if (!reader.Read()) break;
        }

        return new(coordinates);
    }

    private static KmlPolygon CreatePolygon(XmlReader reader)
    {
        List<Point> outerBoundary = [];
        List<List<Point>> innerBoundaries = [];
        var isOuterBoundary = false;

        while (reader.NodeType != XmlNodeType.EndElement || reader.Name != Constants.Polygon)
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                if (Constants.GetBoundaryRegex().IsMatch(reader.Name))
                {
                    isOuterBoundary = reader.Name == "outerBoundaryIs";
                }
                else if (reader.Name == Constants.Coordinates)
                {
                    if (isOuterBoundary)
                        outerBoundary = ConvertToLatLngArray(reader.ReadElementContentAsString());
                    else
                        innerBoundaries.Add(ConvertToLatLngArray(reader.ReadElementContentAsString()));
                }
            }

            if (!reader.Read()) break;
        }

        return new(outerBoundary, innerBoundaries);
    }

    private static KmlTrack CreateTrack(XmlReader reader)
    {
        List<Point> points = [];
        Dictionary<string, string> properties = [];

        while (reader.NodeType != XmlNodeType.EndElement || reader.Name != Constants.Track)
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                switch (reader.Name)
                {
                    case Constants.Coord:
                        var coordinateString = reader.ReadElementContentAsString();
                        var coordinates = ConvertToLatLngArray(coordinateString);
                        points.AddRange(coordinates);
                        break;
                    case Constants.ExtendedData:
                        foreach (var kvp in SetExtendedDataProperties(reader))
                        {
                            properties[kvp.Key] = kvp.Value;
                        }
                        break;
                }
            }

            if (!reader.Read()) break;
        }

        return new(points, properties);
    }

    private static KmlMultiTrack CreateMultiTrack(XmlReader reader)
    {
        List<KmlTrack> tracks = [];

        reader.Read();
        while (reader.NodeType != XmlNodeType.EndElement || reader.Name != Constants.MultiTrack)
        {
            if (reader.NodeType == XmlNodeType.Element && reader.Name == Constants.Track)
                tracks.Add(CreateTrack(reader));

            if (!reader.Read()) break;
        }

        return new(tracks);
    }

    private static MultiGeometry CreateMultiGeometry(XmlReader reader)
    {
        List<IGeometry?> geometries = [];

        reader.Read();
        while (reader.NodeType != XmlNodeType.EndElement || reader.Name != Constants.MultiGeometry)
        {
            if (reader.NodeType == XmlNodeType.Element && Constants.GetGeometryRegex().IsMatch(reader.Name))
                geometries.Add(CreateGeometry(reader, reader.Name));

            if (!reader.Read()) break;
        }

        return new(geometries.OfType<IGeometry>());
    }

    private static List<Point> ConvertToLatLngArray(string coordinatesString)
    {
        List<Point> points = [];
        var coordinates = coordinatesString.Trim().Split(Array.Empty<char>(), StringSplitOptions.RemoveEmptyEntries);

        foreach (var coordinate in coordinates)
            points.Add(ConvertToLatLng(coordinate));

        return points;
    }

    private static Point ConvertToLatLng(string coordinateString, string separator = ",")
    {
        var coordinate = coordinateString.Trim().Split(separator, StringSplitOptions.RemoveEmptyEntries);

        if (coordinate.Length < 2)
            throw new ArgumentException("Wrong coordinate, latitude and longitude must be set");

        var lat = double.Parse(coordinate[LatitudeIndex], CultureInfo.InvariantCulture);
        var lon = double.Parse(coordinate[LongitudeIndex], CultureInfo.InvariantCulture);

        return new(lat, lon);
    }

    private static LatLngBounds CreateLatLngBounds(double? north, double? south, double? east, double? west)
    {
        if (!north.HasValue || !south.HasValue || !east.HasValue || !west.HasValue)
            throw new ArgumentException("All compass points must be provided.");

        Point southWest = new(south.Value, west.Value);
        Point northEast = new(north.Value, east.Value);
        return new(southWest, northEast);
    }
}