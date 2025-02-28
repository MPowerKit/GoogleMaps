﻿using System.Runtime.CompilerServices;

namespace MPowerKit.GoogleMaps;

public class TileOverlay : VisualElement
{
    public virtual void ClearTileCache()
    {
        var native = NativeObjectAttachedProperty.GetNativeObject(this);
#if ANDROID
        (native as Android.Gms.Maps.Model.TileOverlay)?.ClearTileCache();
#elif IOS
        (native as Google.Maps.TileLayer)?.ClearTileCache();
#endif
    }

    protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName == TileSizeProperty.PropertyName
            || propertyName == TileTemplateProperty.PropertyName
            || propertyName == TileProviderProperty.PropertyName)
        {
            ClearTileCache();
        }
    }

    #region FadeIn
    public bool FadeIn
    {
        get { return (bool)GetValue(FadeInProperty); }
        set { SetValue(FadeInProperty, value); }
    }

    public static readonly BindableProperty FadeInProperty =
        BindableProperty.Create(
            nameof(FadeIn),
            typeof(bool),
            typeof(TileOverlay),
            true);
    #endregion

    #region TileSize
    public int TileSize
    {
        get { return (int)GetValue(TileSizeProperty); }
        set { SetValue(TileSizeProperty, value); }
    }

    public static readonly BindableProperty TileSizeProperty =
        BindableProperty.Create(
            nameof(TileSize),
            typeof(int),
            typeof(TileOverlay),
            256);
    #endregion

    #region TileProvider
    public Func<Point, int, int, ImageSource?> TileProvider
    {
        get { return (Func<Point, int, int, ImageSource?>)GetValue(TileProviderProperty); }
        set { SetValue(TileProviderProperty, value); }
    }

    public static readonly BindableProperty TileProviderProperty =
        BindableProperty.Create(
            nameof(TileProvider),
            typeof(Func<Point, int, int, ImageSource?>),
            typeof(TileOverlay));
    #endregion

    #region TileTemplate
    public DataTemplate TileTemplate
    {
        get { return (DataTemplate)GetValue(TileTemplateProperty); }
        set { SetValue(TileTemplateProperty, value); }
    }

    public static readonly BindableProperty TileTemplateProperty =
        BindableProperty.Create(
            nameof(TileTemplate),
            typeof(DataTemplate),
            typeof(TileOverlay));
    #endregion
}