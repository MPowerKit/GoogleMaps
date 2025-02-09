using System.Xml;

namespace MPowerKit.GoogleMaps.Data;

/// <summary>
/// Parses the container of a given KML file into a KmlContainer object.
/// </summary>
public static class KmlContainerParser
{
    /// <summary>
    /// Creates a KmlContainer object by parsing the XML data.
    /// </summary>
    public static KmlContainer CreateContainer(XmlReader reader)
    {
        return AssignPropertiesToContainer(reader);
    }

    /// <summary>
    /// Assigns properties to a KmlContainer by parsing the XML data.
    /// </summary>
    private static KmlContainer AssignPropertiesToContainer(XmlReader reader)
    {
        var startTag = reader.Name;
        var containerId = reader.GetAttribute(Constants.Id);
        Dictionary<string, string> containerProperties = [];
        Dictionary<string, KmlStyle> containerStyles = [];
        HashSet<KmlPlacemark> containerPlacemarks = [];
        List<KmlContainer> nestedContainers = [];
        Dictionary<string, string> containerStyleMaps = [];
        HashSet<KmlGroundOverlay> containerGroundOverlays = [];

        reader.Read();

        while (reader.NodeType != XmlNodeType.EndElement || reader.Name != startTag)
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                switch (reader.Name)
                {
                    case var _ when Constants.GetUnsupportedRegex().IsMatch(reader.Name):
                        KmlParser.Skip(reader);
                        break;
                    case var _ when Constants.GetContainerRegex().IsMatch(reader.Name):
                        nestedContainers.Add(AssignPropertiesToContainer(reader));
                        break;
                    case var _ when Constants.GetPropertyRegex().IsMatch(reader.Name):
                        containerProperties[reader.Name] = reader.ReadElementContentAsString();
                        break;
                    case Constants.StyleMap:
                        SetContainerStyleMap(reader, containerStyleMaps);
                        break;
                    case Constants.Style:
                        SetContainerStyle(reader, containerStyles);
                        break;
                    case Constants.Placemark:
                        SetContainerPlacemark(reader, containerPlacemarks);
                        break;
                    case Constants.ExtendedData:
                        SetExtendedDataProperties(reader, containerProperties);
                        break;
                    case Constants.GroundOverlay:
                        containerGroundOverlays.Add(KmlFeatureParser.CreateGroundOverlay(reader));
                        break;
                }
            }

            if (!reader.Read()) break;
        }

        return new(
            containerProperties,
            containerStyles,
            containerPlacemarks,
            containerStyleMaps,
            nestedContainers,
            containerGroundOverlays,
            containerId
        );
    }

    /// <summary>
    /// Sets the style map for the container by parsing the XML data.
    /// </summary>
    private static void SetContainerStyleMap(XmlReader reader, Dictionary<string, string> containerStyleMap)
    {
        var styleMaps = KmlStyleParser.CreateStyleMap(reader);
        foreach (var styleMap in styleMaps)
            containerStyleMap[styleMap.Key] = styleMap.Value;
    }

    /// <summary>
    /// Sets extended data properties for the container by parsing the XML data.
    /// </summary>
    private static void SetExtendedDataProperties(XmlReader reader, Dictionary<string, string> containerProperties)
    {
        string? propertyKey = null;

        while (reader.NodeType != XmlNodeType.EndElement || reader.Name != Constants.ExtendedData)
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                if (reader.Name == Constants.Data)
                    propertyKey = reader.GetAttribute(Constants.Name);
                else if (reader.Name == Constants.Value && propertyKey is not null)
                {
                    containerProperties[propertyKey] = reader.ReadElementContentAsString();
                    propertyKey = null;
                }
            }

            if (!reader.Read()) break;
        }
    }

    /// <summary>
    /// Sets the style for the container by parsing the XML data.
    /// </summary>
    private static void SetContainerStyle(XmlReader reader, Dictionary<string, KmlStyle> containerStyles)
    {
        var styleId = reader.GetAttribute(Constants.Id);
        if (!string.IsNullOrEmpty(styleId))
        {
            var style = KmlStyleParser.CreateStyle(reader);
            containerStyles[style.StyleId!] = style;
        }
    }

    /// <summary>
    /// Sets the placemark for the container by parsing the XML data.
    /// </summary>
    private static void SetContainerPlacemark(XmlReader reader, HashSet<KmlPlacemark> containerPlacemarks)
    {
        containerPlacemarks.Add(KmlFeatureParser.CreatePlacemark(reader));
    }
}