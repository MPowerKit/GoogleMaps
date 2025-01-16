using System.Collections;
using System.ComponentModel;
using System.Windows.Input;

using Microsoft.Maui.Controls.Shapes;

namespace MPowerKit.GoogleMaps;

public partial class GoogleMap
{
    public const string PolygonManagerName = nameof(PolygonManagerName);

    public event Action<Polygon>? PolygonClick;

    protected virtual void InitPolygons()
    {

    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void SendPolygonClick(Polygon polygon)
    {
        PolygonClick?.Invoke(polygon);

        var parameter = polygon.BindingContext ?? polygon;

        if (PolygonClickedCommand?.CanExecute(parameter) is true)
            PolygonClickedCommand.Execute(parameter);
    }

    #region Polygons
    public IEnumerable<Polygon> Polygons
    {
        get => (IEnumerable<Polygon>)GetValue(PolygonsProperty);
        set => SetValue(PolygonsProperty, value);
    }

    public static readonly BindableProperty PolygonsProperty =
        BindableProperty.Create(
            nameof(Polygons),
            typeof(IEnumerable<Polygon>),
            typeof(GoogleMap));
    #endregion

    #region PolygonsSource
    public IEnumerable PolygonsSource
    {
        get => (IEnumerable)GetValue(PolygonsSourceProperty);
        set => SetValue(PolygonsSourceProperty, value);
    }

    public static readonly BindableProperty PolygonsSourceProperty =
        BindableProperty.Create(
            nameof(PolygonsSource),
            typeof(IEnumerable),
            typeof(GoogleMap));
    #endregion

    #region PolygonItemTemplate
    public DataTemplate PolygonItemTemplate
    {
        get => (DataTemplate)GetValue(PolygonItemTemplateProperty);
        set => SetValue(PolygonItemTemplateProperty, value);
    }

    public static readonly BindableProperty PolygonItemTemplateProperty =
        BindableProperty.Create(
            nameof(PolygonItemTemplate),
            typeof(DataTemplate),
            typeof(GoogleMap));
    #endregion

    #region PolygonClickedCommand
    public ICommand PolygonClickedCommand
    {
        get => (ICommand)GetValue(PolygonClickedCommandProperty);
        set => SetValue(PolygonClickedCommandProperty, value);
    }

    public static readonly BindableProperty PolygonClickedCommandProperty =
        BindableProperty.Create(
            nameof(PolygonClickedCommand),
            typeof(ICommand),
            typeof(GoogleMap));
    #endregion
}