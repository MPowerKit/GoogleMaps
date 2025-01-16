using System.Collections;
using System.ComponentModel;
using System.Windows.Input;

using Microsoft.Maui.Controls.Shapes;

namespace MPowerKit.GoogleMaps;

public partial class GoogleMap
{
    public const string PolylineManagerName = nameof(PolylineManagerName);

    public event Action<Polyline>? PolylineClick;

    protected virtual void InitPolylines()
    {

    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void SendPolylineClick(Polyline polyline)
    {
        PolylineClick?.Invoke(polyline);

        var parameter = polyline.BindingContext ?? polyline;

        if (PolylineClickedCommand?.CanExecute(parameter) is true)
            PolylineClickedCommand.Execute(parameter);
    }

    #region Polylines
    public IEnumerable<Polyline> Polylines
    {
        get => (IEnumerable<Polyline>)GetValue(PolylinesProperty);
        set => SetValue(PolylinesProperty, value);
    }

    public static readonly BindableProperty PolylinesProperty =
        BindableProperty.Create(
            nameof(Polylines),
            typeof(IEnumerable<Polyline>),
            typeof(GoogleMap));
    #endregion

    #region PolylinesSource
    public IEnumerable PolylinesSource
    {
        get => (IEnumerable)GetValue(PolylinesSourceProperty);
        set => SetValue(PolylinesSourceProperty, value);
    }

    public static readonly BindableProperty PolylinesSourceProperty =
        BindableProperty.Create(
            nameof(PolylinesSource),
            typeof(IEnumerable),
            typeof(GoogleMap));
    #endregion

    #region PolylineItemTemplate
    public DataTemplate PolylineItemTemplate
    {
        get => (DataTemplate)GetValue(PolylineItemTemplateProperty);
        set => SetValue(PolylineItemTemplateProperty, value);
    }

    public static readonly BindableProperty PolylineItemTemplateProperty =
        BindableProperty.Create(
            nameof(PolylineItemTemplate),
            typeof(DataTemplate),
            typeof(GoogleMap));
    #endregion

    #region PolylineClickedCommand
    public ICommand PolylineClickedCommand
    {
        get => (ICommand)GetValue(PolylineClickedCommandProperty);
        set => SetValue(PolylineClickedCommandProperty, value);
    }

    public static readonly BindableProperty PolylineClickedCommandProperty =
        BindableProperty.Create(
            nameof(PolylineClickedCommand),
            typeof(ICommand),
            typeof(GoogleMap));
    #endregion
}