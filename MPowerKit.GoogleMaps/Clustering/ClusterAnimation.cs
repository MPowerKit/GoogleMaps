using System.ComponentModel;

using Microsoft.Maui.Converters;

namespace MPowerKit.GoogleMaps;

public interface IClusterAnimation
{
    Easing EasingIn { get; set; }
    Easing EasingOut { get; set; }
    TimeSpan DurationIn { get; set; }
    TimeSpan DurationOut { get; set; }
}

public class ClusterAnimation : BindableObject, IClusterAnimation
{
    #region EasingIn
    [TypeConverter(typeof(EasingTypeConverter))]
    public Easing EasingIn
    {
        get => (Easing)GetValue(EasingInProperty);
        set => SetValue(EasingInProperty, value);
    }

    public static readonly BindableProperty EasingInProperty =
        BindableProperty.Create(
            nameof(EasingIn),
            typeof(Easing),
            typeof(ClusterAnimation),
            Easing.SinIn
            );
    #endregion

    #region EasingOut
    [TypeConverter(typeof(EasingTypeConverter))]
    public Easing EasingOut
    {
        get => (Easing)GetValue(EasingOutProperty);
        set => SetValue(EasingOutProperty, value);
    }

    public static readonly BindableProperty EasingOutProperty =
        BindableProperty.Create(
            nameof(EasingOut),
            typeof(Easing),
            typeof(ClusterAnimation),
            Easing.SinOut
            );
    #endregion

    #region DurationIn
    public TimeSpan DurationIn
    {
        get => (TimeSpan)GetValue(DurationInProperty);
        set => SetValue(DurationInProperty, value);
    }

    public static readonly BindableProperty DurationInProperty =
        BindableProperty.Create(
            nameof(DurationIn),
            typeof(TimeSpan),
            typeof(ClusterAnimation),
            TimeSpan.FromMilliseconds(300)
            );
    #endregion

    #region DurationOut
    public TimeSpan DurationOut
    {
        get => (TimeSpan)GetValue(DurationOutProperty);
        set => SetValue(DurationOutProperty, value);
    }

    public static readonly BindableProperty DurationOutProperty =
        BindableProperty.Create(
            nameof(DurationOut),
            typeof(TimeSpan),
            typeof(ClusterAnimation),
            TimeSpan.FromMilliseconds(300)
            );
    #endregion
}