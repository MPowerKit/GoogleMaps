using System.Xml;

namespace MPowerKit.GoogleMaps.Data;

/// <summary>
/// Parses a given KML file into KmlStyle, KmlPlacemark, KmlGroundOverlay, and KmlContainer objects.
/// </summary>
public partial class KmlParser
{
    private readonly XmlReader _parser;

    /// <summary>
    /// Gets the list of styles created by the parser.
    /// </summary>
    public Dictionary<string, KmlStyle> Styles { get; private set; } = [];

    /// <summary>
    /// Gets the list of KmlPlacemark objects.
    /// </summary>
    public HashSet<KmlPlacemark> Placemarks { get; private set; } = [];

    /// <summary>
    /// Gets the list of Kml Style Maps.
    /// </summary>
    public Dictionary<string, string> StyleMaps { get; private set; } = [];

    /// <summary>
    /// Gets the list of Kml Folders and Documents.
    /// </summary>
    public HashSet<KmlContainer> Containers { get; private set; } = [];

    /// <summary>
    /// Gets the list of Ground Overlays.
    /// </summary>
    public HashSet<KmlGroundOverlay> GroundOverlays { get; private set; } = [];

    public Dictionary<string, byte[]>? Images { get; set; }

    /// <summary>
    /// Creates a new KmlParser object.
    /// </summary>
    /// <param name="parser">XmlReader containing the KML file to parse.</param>
    public KmlParser(XmlReader parser)
    {
        _parser = parser ?? throw new ArgumentNullException(nameof(parser));
    }

    /// <summary>
    /// Parses the KML file and stores the created KmlStyle, KmlPlacemark, KmlGroundOverlay, and KmlContainer objects.
    /// </summary>
    public void ParseKml()
    {
        while (_parser.NodeType != XmlNodeType.EndElement)
        {
            if (_parser.NodeType == XmlNodeType.Element)
            {
                var elementName = _parser.Name;
                if (Constants.GetUnsupportedRegex().IsMatch(elementName))
                {
                    Skip(_parser);
                }
                else if (Constants.GetContainerRegex().IsMatch(elementName))
                {
                    Containers.Add(KmlContainerParser.CreateContainer(_parser));
                }
                else if (elementName == Constants.Style)
                {
                    var style = KmlStyleParser.CreateStyle(_parser);
                    if (style is not null)
                    {
                        Styles[style.StyleId!] = style;
                    }
                }
                else if (elementName == Constants.StyleMap)
                {
                    var styleMap = KmlStyleParser.CreateStyleMap(_parser);
                    foreach (var pair in styleMap)
                    {
                        StyleMaps[pair.Key] = pair.Value;
                    }
                }
                else if (elementName == Constants.Placemark)
                {
                    var placemark = KmlFeatureParser.CreatePlacemark(_parser);
                    if (placemark is not null)
                    {
                        Placemarks.Add(placemark);
                    }
                }
                else if (elementName == Constants.GroundOverlay)
                {
                    var groundOverlay = KmlFeatureParser.CreateGroundOverlay(_parser);
                    if (groundOverlay is not null)
                    {
                        GroundOverlays.Add(groundOverlay);
                    }
                }
            }

            if (!_parser.Read()) break;
        }

        MapStyles(Styles, StyleMaps);
        FlattenObjects(Containers);
        AssignStyles(Placemarks, Styles);
    }

    private static void AssignStyles(HashSet<KmlPlacemark> placemarks, Dictionary<string, KmlStyle> styles)
    {
        foreach (var placemark in placemarks)
        {
            if (placemark.StyleUrl is not null)
            {
                placemark.Style = styles[placemark.StyleUrl];
            }
        }
    }

    private static void MapStyles(Dictionary<string, KmlStyle> styles, Dictionary<string, string> styleMaps)
    {
        foreach (var kvp in styleMaps)
        {
            if (styles.TryGetValue(kvp.Value, out KmlStyle? value))
            {
                styles[kvp.Key] = value;
            }
        }
    }

    private void FlattenObjects(IEnumerable<KmlContainer> containers, bool isParentVisible = true)
    {
        foreach (var container in containers)
        {
            MapStyles(container.Styles, container.StyleMap);
            foreach (var kvp in container.Styles)
            {
                if (!Styles.ContainsKey(kvp.Key))
                {
                    Styles[kvp.Key] = kvp.Value;
                }
            }

            container.IsVisible = GetItemVisibility(container, isParentVisible);

            foreach (var go in container.GroundOverlays)
            {
                go.IsVisible = go.IsVisible && GetItemVisibility(go, container.IsVisible);

                GroundOverlays.Add(go);
            }

            foreach (var placemark in container.Placemarks)
            {
                placemark.IsVisible = GetItemVisibility(placemark, container.IsVisible);

                if (placemark.HasProperty(Constants.DrawOrder))
                {
                    var drawOrder = placemark.GetProperty(Constants.DrawOrder);
                    if (int.TryParse(drawOrder, out var zIndex))
                    {
                        placemark.ZIndex = zIndex;
                    }
                }

                if (placemark.HasProperty(Constants.Name))
                {
                    placemark.Name = placemark.GetProperty(Constants.Name);
                }

                if (placemark.HasProperty(Constants.Description))
                {
                    placemark.Description = placemark.GetProperty(Constants.Description);
                }

                Placemarks.Add(placemark);
            }

            FlattenObjects(container.Containers, container.IsVisible);
        }
    }

    private static bool GetItemVisibility(IHasProperty item, bool isParentVisible = true)
    {
        var isItemVisible = true;
        if (item.HasProperty(Constants.Visibility))
        {
            var itemVisibility = item.GetProperty(Constants.Visibility);
            if (int.TryParse(itemVisibility, out var visibleInt))
            {
                isItemVisible = visibleInt != 0;
            }
        }

        return isParentVisible && isItemVisible;
    }

    /// <summary>
    /// Skips tags from START TAG to END TAG.
    /// </summary>
    /// <param name="parser">XmlReader to skip elements from.</param>
    public static void Skip(XmlReader parser)
    {
        if (parser.NodeType != XmlNodeType.Element)
        {
            throw new InvalidOperationException("Parser must be positioned on a start tag.");
        }

        var depth = 1;
        while (depth > 0 && parser.Read())
        {
            switch (parser.NodeType)
            {
                case XmlNodeType.EndElement:
                    depth--;
                    break;
                case XmlNodeType.Element:
                    if (!parser.IsEmptyElement)
                    {
                        depth++;
                    }
                    break;
            }
        }
    }
}