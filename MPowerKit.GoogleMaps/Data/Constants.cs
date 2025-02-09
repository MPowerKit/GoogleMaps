using System.Text.RegularExpressions;

namespace MPowerKit.GoogleMaps.Data;

public static partial class Constants
{
    public const string Id = "id";
    public const string StyleUrl = "styleUrl";
    public const string IconStyleScale = "scale";
    public const string Color = "color";
    public const string ColorStyleMode = "colorMode";
    public const string StyleMapKey = "key";
    public const string StyleMapNormalStyle = "normal";
    public const string Fill = "fill";
    public const string Name = "name";
    public const string Description = "description";
    public const string Value = "value";
    public const string Text = "text";
    public const string Href = "href";
    public const string Fraction = "fraction";
    public const string DrawOrder = "drawOrder";
    public const string Coord = "coord";
    public const string Coordinates = "coordinates";
    public const string Visibility = "visibility";
    public const string Rotation = "rotation";
    public const string IconScale = "iconScale";
    public const string Outline = "outline";
    public const string IconUrl = "iconUrl";
    public const string FillColor = "fillColor";
    public const string MarkerColor = "markerColor";
    public const string Heading = "heading";
    public const string HotSpot = "hotSpot";
    public const string IconColorMode = "iconColorMode";
    public const string LineColorMode = "lineColorMode";
    public const string PolyColorMode = "polyColorMode";
    public const string OutlineColor = "outlineColor";
    public const string Width = "width";
    public const string Random = "random";
    public const string Type = "type";
    public const string Features = "features";
    public const string BoundingBox = "bbox";
    public const string Geometry = "geometry";
    public const string Geometries = "geometries";
    public const string Properties = "properties";

    public const string Icon = nameof(Icon);
    public const string Style = nameof(Style);
    public const string IconStyle = nameof(IconStyle);
    public const string LineStyle = nameof(LineStyle);
    public const string PolyStyle = nameof(PolyStyle);
    public const string BalloonStyle = nameof(BalloonStyle);
    public const string StyleMap = nameof(StyleMap);
    public const string ExtendedData = nameof(ExtendedData);
    public const string Data = nameof(Data);

    public const string Feature = nameof(Feature);
    public const string FeatureCollection = nameof(FeatureCollection);
    public const string GeometryCollection = nameof(GeometryCollection);
    public const string Container = nameof(Container);
    public const string Placemark = nameof(Placemark);
    public const string GroundOverlay = nameof(GroundOverlay);
    public const string Point = nameof(Point);
    public const string LineString = nameof(LineString);
    public const string Polygon = nameof(Polygon);
    public const string Polyline = nameof(Polyline);
    public const string Track = nameof(Track);
    public const string MultiTrack = nameof(MultiTrack);
    public const string MultiGeometry = nameof(MultiGeometry);
    public const string MultiPoint = nameof(MultiPoint);
    public const string MultiLineString = nameof(MultiLineString);
    public const string MultiPolygon = nameof(MultiPolygon);

    public const string ContainerRegex = "Folder|Document";
    public const string UnsupportedRegex = "altitude|altitudeModeGroup|altitudeMode|" +
        "begin|bottomFov|cookie|displayName|displayMode|end|expires|extrude|" +
        "flyToView|gridOrigin|httpQuery|leftFov|linkDescription|linkName|linkSnippet|" +
        "listItemType|maxSnippetLines|maxSessionLength|message|minAltitude|minFadeExtent|" +
        "minLodPixels|minRefreshPeriod|maxAltitude|maxFadeExtent|maxLodPixels|maxHeight|" +
        "maxWidth|near|NetworkLink|NetworkLinkControl|overlayXY|range|refreshMode|" +
        "refreshInterval|refreshVisibility|rightFov|roll|rotationXY|screenXY|shape|sourceHref|" +
        "state|targetHref|tessellate|tileSize|topFov|viewBoundScale|viewFormat|viewRefreshMode|" +
        "viewRefreshTime|when";
    public const string PropertyRegex = "name|description|visibility|open|address|phoneNumber";
    public const string GeometryRegex = "Point|LineString|Polygon|MultiGeometry|Track|MultiTrack";
    public const string CompassRegex = "north|south|east|west";
    public const string BoundaryRegex = "outerBoundaryIs|innerBoundaryIs";

    [GeneratedRegex(UnsupportedRegex)]
    public static partial Regex GetUnsupportedRegex();
    [GeneratedRegex(ContainerRegex)]
    public static partial Regex GetContainerRegex();
    [GeneratedRegex(PropertyRegex)]
    public static partial Regex GetPropertyRegex();
    [GeneratedRegex(GeometryRegex)]
    public static partial Regex GetGeometryRegex();
    [GeneratedRegex(CompassRegex)]
    public static partial Regex GetCompassRegex();
    [GeneratedRegex(BoundaryRegex)]
    public static partial Regex GetBoundaryRegex();
}