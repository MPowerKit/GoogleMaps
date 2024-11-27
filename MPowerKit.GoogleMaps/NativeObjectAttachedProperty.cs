namespace MPowerKit.GoogleMaps;

public class NativeObjectAttachedProperty
{
    #region NativeObject
    public static readonly BindableProperty NativeObjectProperty =
        BindableProperty.CreateAttached(
            "NativeObject",
            typeof(object),
            typeof(NativeObjectAttachedProperty),
            null);

    public static object GetNativeObject(BindableObject view) => (object)view.GetValue(NativeObjectProperty);

    public static void SetNativeObject(BindableObject view, object value) => view.SetValue(NativeObjectProperty, value);
    #endregion
}