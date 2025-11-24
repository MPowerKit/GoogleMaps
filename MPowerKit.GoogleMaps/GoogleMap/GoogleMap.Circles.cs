using System.Collections;
using System.ComponentModel;
using System.Windows.Input;

namespace MPowerKit.GoogleMaps;

public partial class GoogleMap
{
    public const string CircleManagerName = nameof(CircleManagerName);

    public event Action<Circle>? CircleClick;

    protected virtual void InitCircles()
    {

    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void SendCircleClick(Circle circle)
    {
        CircleClick?.Invoke(circle);

        var parameter = circle.BindingContext ?? circle;

        if (CircleClickedCommand?.CanExecute(parameter) is true)
            CircleClickedCommand.Execute(parameter);
    }

    #region Circles
    public IEnumerable<Circle>? Circles
    {
        get => (IEnumerable<Circle>?)GetValue(CirclesProperty);
        set => SetValue(CirclesProperty, value);
    }

    public static readonly BindableProperty CirclesProperty =
        BindableProperty.Create(
            nameof(Circles),
            typeof(IEnumerable<Circle>),
            typeof(GoogleMap));
    #endregion

    #region CirclesSource
    public IEnumerable? CirclesSource
    {
        get => (IEnumerable?)GetValue(CirclesSourceProperty);
        set => SetValue(CirclesSourceProperty, value);
    }

    public static readonly BindableProperty CirclesSourceProperty =
        BindableProperty.Create(
            nameof(CirclesSource),
            typeof(IEnumerable),
            typeof(GoogleMap));
    #endregion

    #region CircleItemTemplate
    public DataTemplate? CircleItemTemplate
    {
        get => (DataTemplate?)GetValue(CircleItemTemplateProperty);
        set => SetValue(CircleItemTemplateProperty, value);
    }

    public static readonly BindableProperty CircleItemTemplateProperty =
        BindableProperty.Create(
            nameof(CircleItemTemplate),
            typeof(DataTemplate),
            typeof(GoogleMap));
    #endregion

    #region CircleClickedCommand
    public ICommand? CircleClickedCommand
    {
        get => (ICommand?)GetValue(CircleClickedCommandProperty);
        set => SetValue(CircleClickedCommandProperty, value);
    }

    public static readonly BindableProperty CircleClickedCommandProperty =
        BindableProperty.Create(
            nameof(CircleClickedCommand),
            typeof(ICommand),
            typeof(GoogleMap));
    #endregion
}