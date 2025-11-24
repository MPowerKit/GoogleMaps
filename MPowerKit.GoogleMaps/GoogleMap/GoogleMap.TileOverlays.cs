using System.Collections;

namespace MPowerKit.GoogleMaps;

public partial class GoogleMap
{
    public const string TileOverlayManagerName = nameof(TileOverlayManagerName);

    protected virtual void InitTiles()
    {

    }

    #region TileOverlays
    public IEnumerable<TileOverlay>? TileOverlays
    {
        get => (IEnumerable<TileOverlay>?)GetValue(TileOverlaysProperty);
        set => SetValue(TileOverlaysProperty, value);
    }

    public static readonly BindableProperty TileOverlaysProperty =
        BindableProperty.Create(
            nameof(TileOverlays),
            typeof(IEnumerable<TileOverlay>),
            typeof(GoogleMap));
    #endregion

    #region TileOverlaysSource
    public IEnumerable? TileOverlaysSource
    {
        get => (IEnumerable?)GetValue(TileOverlaysSourceProperty);
        set => SetValue(TileOverlaysSourceProperty, value);
    }

    public static readonly BindableProperty TileOverlaysSourceProperty =
        BindableProperty.Create(
            nameof(TileOverlaysSource),
            typeof(IEnumerable),
            typeof(GoogleMap));
    #endregion

    #region TileOverlayItemTemplate
    public DataTemplate? TileOverlayItemTemplate
    {
        get => (DataTemplate?)GetValue(TileOverlayItemTemplateProperty);
        set => SetValue(TileOverlayItemTemplateProperty, value);
    }

    public static readonly BindableProperty TileOverlayItemTemplateProperty =
        BindableProperty.Create(
            nameof(TileOverlayItemTemplate),
            typeof(DataTemplate),
            typeof(GoogleMap));
    #endregion
}