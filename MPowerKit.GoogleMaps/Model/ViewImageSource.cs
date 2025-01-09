using System.Runtime.CompilerServices;

namespace MPowerKit.GoogleMaps;

public interface IViewImageSource : IImageSource
{
    View? View { get; }
}

[ContentProperty("View")]
public class ViewImageSource : ImageSource, IViewImageSource
{
    public override bool IsEmpty => View is null;

    public static implicit operator ViewImageSource(View view)
    {
        return new ViewImageSource() { View = view };
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        if (!IsEmpty && View.BindingContext is null)
        {
            View.BindingContext = this.BindingContext;
        }
    }

    protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName == ViewProperty.PropertyName
            && !IsEmpty && View.BindingContext is null)
        {
            View.BindingContext = this.BindingContext;
        }
    }

    #region View
    public View View
    {
        get { return (View)GetValue(ViewProperty); }
        set { SetValue(ViewProperty, value); }
    }

    public static readonly BindableProperty ViewProperty =
        BindableProperty.Create(
            nameof(View),
            typeof(View),
            typeof(ViewImageSource)
            );
    #endregion
}