using System.Xml;

namespace MPowerKit.GoogleMaps.Data;

/// <summary>
/// Parses the styles of a given KML file into a KmlStyle object.
/// </summary>
public static class KmlStyleParser
{
    /// <summary>
    /// Parses the IconStyle, LineStyle, and PolyStyle tags into a KmlStyle object.
    /// </summary>
    public static KmlStyle CreateStyle(XmlReader reader)
    {
        var styleProperties = new KmlStyle();
        SetStyleId(reader.GetAttribute(Constants.Id), styleProperties);

        while (reader.NodeType != XmlNodeType.EndElement || reader.Name != Constants.Style)
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                switch (reader.Name)
                {
                    case Constants.IconStyle:
                        CreateIconStyle(reader, styleProperties);
                        break;
                    case Constants.LineStyle:
                        CreateLineStyle(reader, styleProperties);
                        break;
                    case Constants.PolyStyle:
                        CreatePolyStyle(reader, styleProperties);
                        break;
                    case Constants.BalloonStyle:
                        CreateBalloonStyle(reader, styleProperties);
                        break;
                }
            }

            if (!reader.Read()) break;
        }

        return styleProperties;
    }

    /// <summary>
    /// Sets the style ID for the KmlStyle object.
    /// </summary>
    private static void SetStyleId(string? id, KmlStyle styleProperties)
    {
        if (!string.IsNullOrEmpty(id))
        {
            // Append # to a local styleUrl
            styleProperties.StyleId = $"#{id}";
        }
    }

    /// <summary>
    /// Parses the IconStyle tag and assigns relevant properties to the KmlStyle object.
    /// </summary>
    private static void CreateIconStyle(XmlReader reader, KmlStyle style)
    {
        while (reader.NodeType != XmlNodeType.EndElement || reader.Name != Constants.IconStyle)
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                switch (reader.Name)
                {
                    case Constants.Heading:
                        style.Heading = reader.ReadElementContentAsFloat();
                        break;
                    case Constants.Icon:
                        SetIconUrl(reader, style);
                        break;
                    case Constants.HotSpot:
                        SetIconHotSpot(reader, style);
                        break;
                    case Constants.IconStyleScale:
                        style.IconScale = reader.ReadElementContentAsDouble();
                        break;
                    case Constants.Color:
                        style.MarkerColor = reader.ReadElementContentAsString();
                        break;
                    case Constants.ColorStyleMode:
                        style.SetIconColorMode(reader.ReadElementContentAsString());
                        break;
                }
            }

            if (!reader.Read()) break;
        }
    }

    /// <summary>
    /// Parses the StyleMap property and stores the ID and the normal style tag.
    /// </summary>
    public static Dictionary<string, string> CreateStyleMap(XmlReader reader)
    {
        Dictionary<string, string> styleMaps = [];
        var isNormalStyleMapValue = false;
        var styleId = $"#{reader.GetAttribute(Constants.Id)}";

        while (reader.NodeType != XmlNodeType.EndElement || reader.Name != Constants.StyleMap)
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                if (reader.Name == Constants.StyleMapKey && reader.ReadElementContentAsString() == Constants.StyleMapNormalStyle)
                    isNormalStyleMapValue = true;
                else if (reader.Name == Constants.StyleUrl && isNormalStyleMapValue)
                {
                    styleMaps[styleId] = reader.ReadElementContentAsString();
                    isNormalStyleMapValue = false;
                }
            }

            if (!reader.Read()) break;
        }

        return styleMaps;
    }

    /// <summary>
    /// Parses the BalloonStyle tag and assigns relevant properties to the KmlStyle object.
    /// </summary>
    private static void CreateBalloonStyle(XmlReader reader, KmlStyle style)
    {
        while (reader.NodeType != XmlNodeType.EndElement || reader.Name != Constants.BalloonStyle)
        {
            if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals(Constants.Text, StringComparison.OrdinalIgnoreCase))
                style.InfoWindowText = reader.ReadElementContentAsString();
            else if (!reader.Read()) break;
        }
    }

    /// <summary>
    /// Sets the icon URL for the style.
    /// </summary>
    private static void SetIconUrl(XmlReader reader, KmlStyle style)
    {
        while (reader.NodeType != XmlNodeType.EndElement || reader.Name != Constants.Icon)
        {
            if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals(Constants.Href, StringComparison.OrdinalIgnoreCase))
                style.IconUrl = reader.ReadElementContentAsString();
            else if (!reader.Read()) break;
        }
    }

    /// <summary>
    /// Sets the hot spot for the icon.
    /// </summary>
    private static void SetIconHotSpot(XmlReader reader, KmlStyle style)
    {
        try
        {
            var xValue = float.Parse(reader.GetAttribute("x") ?? "");
            var yValue = float.Parse(reader.GetAttribute("y") ?? "");
            var xUnits = reader.GetAttribute("xunits") ?? "";
            var yUnits = reader.GetAttribute("yunits") ?? "";
            style.SetHotSpot(xValue, yValue, xUnits, yUnits);
        }
        catch (NullReferenceException)
        {
            Console.WriteLine("Missing 'x' or 'y' attributes in hotSpot for style with ID: " + style.StyleId);
        }
        catch (FormatException)
        {
            Console.WriteLine("Invalid number in 'x' or 'y' attributes in hotSpot for style with ID: " + style.StyleId);
        }
    }

    /// <summary>
    /// Parses the LineStyle tag and assigns relevant properties to the KmlStyle object.
    /// </summary>
    private static void CreateLineStyle(XmlReader reader, KmlStyle style)
    {
        while (reader.NodeType != XmlNodeType.EndElement || reader.Name != Constants.LineStyle)
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                switch (reader.Name)
                {
                    case Constants.Color:
                        style.OutlineColor = reader.ReadElementContentAsString();
                        break;
                    case Constants.Width:
                        style.Width = reader.ReadElementContentAsFloat();
                        break;
                    case Constants.ColorStyleMode:
                        style.SetLineColorMode(reader.ReadElementContentAsString());
                        break;
                }
            }

            if (!reader.Read()) break;
        }
    }

    /// <summary>
    /// Parses the PolyStyle tag and assigns relevant properties to the KmlStyle object.
    /// </summary>
    private static void CreatePolyStyle(XmlReader reader, KmlStyle style)
    {
        while (reader.NodeType != XmlNodeType.EndElement || reader.Name != Constants.PolyStyle)
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                switch (reader.Name)
                {
                    case Constants.Color:
                        style.FillColor = reader.ReadElementContentAsString();
                        break;
                    case Constants.Outline:
                        style.HasOutline = KmlUtil.ParseBoolean(reader.ReadElementContentAsString());
                        break;
                    case Constants.Fill:
                        style.Fill = KmlUtil.ParseBoolean(reader.ReadElementContentAsString());
                        break;
                    case Constants.ColorStyleMode:
                        style.SetPolyColorMode(reader.ReadElementContentAsString());
                        break;
                }
            }

            if (!reader.Read()) break;
        }
    }
}