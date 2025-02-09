using System.Text.Json;

namespace MPowerKit.GoogleMaps.Data;

public class GeoJsonParser
{
    private readonly byte[] _jsonData;

    public List<GeoJsonFeature> Features { get; private set; } = [];
    public LatLngBounds? BoundingBox { get; private set; }

    public GeoJsonParser(byte[] jsonData)
    {
        _jsonData = jsonData;
    }

    public void ParseGeoJson()
    {
        var reader = new Utf8JsonReader(_jsonData);
        ParseRoot(ref reader);

        foreach (var feature in Features)
        {
            feature.ParseStyles();
        }
    }

    private void ParseRoot(ref Utf8JsonReader reader)
    {
        if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Invalid GeoJSON document");

        string? type = null;
        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
        {
            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString();
                if (propertyName == Constants.Type)
                {
                    reader.Read();
                    type = reader.GetString();
                    break;
                }
                reader.Skip();
            }
        }

        if (type is null) throw new JsonException($"Missing '{Constants.Type}' property in GeoJSON document");

        reader = new Utf8JsonReader(_jsonData); // Reset reader
        ParseByType(type, ref reader);
    }

    private void ParseByType(string type, ref Utf8JsonReader reader)
    {
        switch (type)
        {
            case Constants.Feature:
                {
                    if (ParseFeature(ref reader) is GeoJsonFeature feature)
                        Features.Add(feature);
                }
                break;
            case Constants.FeatureCollection:
                ParseFeatureCollection(ref reader);
                break;
            case Constants.GeometryCollection:
            case Constants.Point:
            case Constants.LineString:
            case Constants.Polygon:
            case Constants.MultiPoint:
            case Constants.MultiLineString:
            case Constants.MultiPolygon:
                {
                    if (ParseGeometryToFeature(ref reader) is GeoJsonFeature geometryFeature)
                        Features.Add(geometryFeature);
                }
                break;
            default:
                throw new JsonException($"Unsupported GeoJSON type: {type}");
        }
    }

    private void ParseFeatureCollection(ref Utf8JsonReader reader)
    {
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propName = reader.GetString();
                reader.Read();

                if (propName == Constants.Features)
                {
                    if (reader.TokenType != JsonTokenType.StartArray)
                        throw new JsonException($"Invalid FeatureCollection - missing '{Constants.Features}' array");

                    while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                    {
                        if (reader.TokenType == JsonTokenType.StartObject
                            && ParseFeature(ref reader) is GeoJsonFeature validFeature)
                        {
                            Features.Add(validFeature);
                        }
                    }
                }
                else if (propName == Constants.BoundingBox)
                {
                    BoundingBox = ParseBoundingBox(ref reader);
                }
            }
        }
    }

    private static GeoJsonFeature? ParseFeature(ref Utf8JsonReader reader)
    {
        string? id = null;
        IGeometry? geometry = null;
        Dictionary<string, string> properties = [];
        LatLngBounds? bounds = null;

        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
        {
            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString();
                reader.Read();

                switch (propertyName)
                {
                    case Constants.Id:
                        id = GetPropertyValue(ref reader);
                        break;

                    case Constants.Geometry:
                        geometry = ParseGeometry(ref reader);
                        break;

                    case Constants.Properties:
                        foreach (var kvp in ParseProperties(ref reader))
                        {
                            properties[kvp.Key] = kvp.Value;
                        }
                        break;

                    case Constants.BoundingBox:
                        bounds = ParseBoundingBox(ref reader);
                        break;

                    default:
                        reader.Skip();
                        break;
                }
            }
        }

        return new(geometry, id, properties, bounds);
    }

    private static IGeometry? ParseGeometry(ref Utf8JsonReader reader)
    {
        string? type = null;
        JsonElement? coordinates = null;

        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
        {
            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString();
                reader.Read();

                switch (propertyName)
                {
                    case Constants.Type:
                        type = reader.GetString();
                        break;

                    case Constants.Coordinates:
                        coordinates = JsonDocument.ParseValue(ref reader).RootElement;
                        break;

                    case Constants.Geometries:
                        return ParseGeometryCollection(ref reader);

                    default:
                        reader.Skip();
                        break;
                }
            }
        }

        if (type is null || coordinates is null)
            return null;

        return CreateGeometry(type, coordinates.Value);
    }

    private static IGeometry? CreateGeometry(string type, JsonElement coordinates)
    {
        return type switch
        {
            Constants.Point => CreatePoint(coordinates),
            Constants.MultiPoint => CreateMultiPoint(coordinates),
            Constants.LineString => CreateLineString(coordinates),
            Constants.MultiLineString => CreateMultiLineString(coordinates),
            Constants.Polygon => CreatePolygon(coordinates),
            Constants.MultiPolygon => CreateMultiPolygon(coordinates),
            _ => null
        };
    }

    private static PointGeometry? CreatePoint(JsonElement coordinates)
    {
        return ParseCoordinate(coordinates) is Point p ? new(p) : null;
    }

    private static GeoJsonMultiPoint CreateMultiPoint(JsonElement coordinates)
    {
        var points = coordinates.EnumerateArray()
            .Select(CreatePoint)
            .OfType<PointGeometry>();

        return new(points);
    }

    private static LineString CreateLineString(JsonElement coordinates)
    {
        var points = ParseCoordinates(coordinates);
        return new(points);
    }

    private static GeoJsonMultiLineString CreateMultiLineString(JsonElement coordinates)
    {
        var lineStrings = coordinates.EnumerateArray()
            .Select(CreateLineString);

        return new(lineStrings);
    }

    private static GeoJsonPolygon CreatePolygon(JsonElement coordinates)
    {
        var rings = coordinates.EnumerateArray()
            .Select(ParseCoordinates);

        return new(rings);
    }

    private static GeoJsonMultiPolygon CreateMultiPolygon(JsonElement coordinates)
    {
        var polygons = coordinates.EnumerateArray()
            .Select(CreatePolygon);

        return new(polygons);
    }

    private static MultiGeometry ParseGeometryCollection(ref Utf8JsonReader reader)
    {
        List<IGeometry> geometries = [];

        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
        {
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                if (ParseGeometry(ref reader) is IGeometry geometry)
                {
                    geometries.Add(geometry);
                }
            }
        }

        return new(geometries);
    }

    private static GeoJsonFeature? ParseGeometryToFeature(ref Utf8JsonReader reader)
    {
        return ParseGeometry(ref reader) is IGeometry parsedGeometry
            ? new GeoJsonFeature(parsedGeometry, null, [], null)
            : null;
    }

    private static LatLngBounds ParseBoundingBox(ref Utf8JsonReader reader)
    {
        List<double> values = [];

        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                values.Add(reader.GetDouble());
            }
        }

        if (values.Count < 4) throw new JsonException("Invalid bbox array");

        Point sw = new(values[1], values[0]);
        Point ne = new(values[3], values[2]);
        return new(sw, ne);
    }

    private static Dictionary<string, string?> ParseProperties(ref Utf8JsonReader reader)
    {
        Dictionary<string, string?> properties = [];

        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
        {
            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString()!;
                reader.Read();
                properties[propertyName] = GetPropertyValue(ref reader);
            }
        }

        return properties;
    }

    private static string? GetPropertyValue(ref Utf8JsonReader reader)
    {
        return reader.TokenType switch
        {
            JsonTokenType.String => reader.GetString(),
            JsonTokenType.Number => reader.GetDouble().ToString(),
            JsonTokenType.True => "true",
            JsonTokenType.False => "false",
            _ => null
        };
    }

    private static IEnumerable<Point> ParseCoordinates(JsonElement coordinates)
    {
        foreach (var node in coordinates.EnumerateArray())
        {
            if (ParseCoordinate(node) is Point coord)
                yield return coord;
        }
    }

    private static Point? ParseCoordinate(JsonElement coordinate)
    {
        var values = coordinate.EnumerateArray()
            .Select(n => n.GetDouble())
            .ToArray();

        if (values.Length < 2) return null;

        return new(values[1], values[0]);
    }
}