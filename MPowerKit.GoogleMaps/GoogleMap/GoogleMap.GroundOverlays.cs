using System.Collections;
using System.ComponentModel;
using System.Windows.Input;

namespace MPowerKit.GoogleMaps;

public partial class GoogleMap
{
    public const string GroundOverlayManagerName = nameof(GroundOverlayManagerName);

    public event Action<GroundOverlay>? GroundOverlayClick;

    protected virtual void InitGroundOverlays()
    {

    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void SendGroundOverlayClick(GroundOverlay groundOverlay)
    {
        GroundOverlayClick?.Invoke(groundOverlay);

        var parameter = groundOverlay.BindingContext ?? groundOverlay;

        if (GroundOverlayClickedCommand?.CanExecute(parameter) is true)
            GroundOverlayClickedCommand.Execute(parameter);
    }

    #region GroundOverlays
    public IEnumerable<GroundOverlay> GroundOverlays
    {
        get => (IEnumerable<GroundOverlay>)GetValue(GroundOverlaysProperty);
        set => SetValue(GroundOverlaysProperty, value);
    }

    public static readonly BindableProperty GroundOverlaysProperty =
        BindableProperty.Create(
            nameof(GroundOverlays),
            typeof(IEnumerable<GroundOverlay>),
            typeof(GoogleMap));
    #endregion

    #region GroundOverlaysSource
    public IEnumerable GroundOverlaysSource
    {
        get => (IEnumerable)GetValue(GroundOverlaysSourceProperty);
        set => SetValue(GroundOverlaysSourceProperty, value);
    }

    public static readonly BindableProperty GroundOverlaysSourceProperty =
        BindableProperty.Create(
            nameof(GroundOverlaysSource),
            typeof(IEnumerable),
            typeof(GoogleMap));
    #endregion

    #region GroundOverlayItemTemplate
    public DataTemplate GroundOverlayItemTemplate
    {
        get => (DataTemplate)GetValue(GroundOverlayItemTemplateProperty);
        set => SetValue(GroundOverlayItemTemplateProperty, value);
    }

    public static readonly BindableProperty GroundOverlayItemTemplateProperty =
        BindableProperty.Create(
            nameof(GroundOverlayItemTemplate),
            typeof(DataTemplate),
            typeof(GoogleMap));
    #endregion

    #region GroundOverlayClickedCommand
    public ICommand GroundOverlayClickedCommand
    {
        get => (ICommand)GetValue(GroundOverlayClickedCommandProperty);
        set => SetValue(GroundOverlayClickedCommandProperty, value);
    }

    public static readonly BindableProperty GroundOverlayClickedCommandProperty =
        BindableProperty.Create(
            nameof(GroundOverlayClickedCommand),
            typeof(ICommand),
            typeof(GoogleMap));
    #endregion
}