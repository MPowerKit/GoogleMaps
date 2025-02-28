﻿using System.Runtime.CompilerServices;

using NPin =
#if ANDROID
    Android.Gms.Maps.Model.Marker;
#elif IOS
    Google.Maps.Marker;
#else
    System.Object;
#endif

namespace MPowerKit.GoogleMaps;

public class Pin : VisualElement
{
    public bool InfoWindowShown { get; protected set; }

    public Pin()
    {
        AnchorX = 0.5;
        AnchorY = 1d;
    }

    public virtual void ShowInfoWindow()
    {
        if (!ShowInfoWindowOnPinSelection) return;

        if (NativeObjectAttachedProperty.GetNativeObject(this) is not NPin native) return;

#if ANDROID
        InfoWindowShown = true;
        native.ShowInfoWindow();
#endif

#if IOS
        if (native.Map is not null)
        {
            InfoWindowShown = true;
            native.Map.SelectedMarker = native;
        }
#endif
    }

    public virtual void HideInfoWindow()
    {
        if (!InfoWindowShown) return;

        InfoWindowShown = false;

        var native = NativeObjectAttachedProperty.GetNativeObject(this) as NPin;
#if ANDROID
        native?.HideInfoWindow();
#endif

#if IOS
        if (native?.Map?.SelectedMarker is not null)
        {
            native.Map.SelectedMarker = null;
        }
#endif
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        if (Icon is not null && Icon.BindingContext is null)
        {
            Icon.BindingContext = this.BindingContext;
        }

        if (InfoWindow is not null && InfoWindow.BindingContext is null)
        {
            InfoWindow.BindingContext = this.BindingContext;
        }
    }

    protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName == IconProperty.PropertyName
            && Icon is not null && Icon.BindingContext is null)
        {
            Icon.BindingContext = this.BindingContext;
        }
        else if (propertyName == InfoWindowProperty.PropertyName
            && InfoWindow is not null && InfoWindow.BindingContext is null)
        {
            InfoWindow.BindingContext = this.BindingContext;
        }
    }

    #region Position
    public Point Position
    {
        get { return (Point)GetValue(PositionProperty); }
        set { SetValue(PositionProperty, value); }
    }

    public static readonly BindableProperty PositionProperty =
        BindableProperty.Create(
            nameof(Position),
            typeof(Point),
            typeof(Pin)
            );
    #endregion

    #region Title
    public string Title
    {
        get { return (string)GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(
            nameof(Title),
            typeof(string),
            typeof(Pin)
            );
    #endregion

    #region Snippet
    public string Snippet
    {
        get { return (string)GetValue(SnippetProperty); }
        set { SetValue(SnippetProperty, value); }
    }

    public static readonly BindableProperty SnippetProperty =
        BindableProperty.Create(
            nameof(Snippet),
            typeof(string),
            typeof(Pin)
            );
    #endregion

    #region Draggable
    public bool Draggable
    {
        get { return (bool)GetValue(DraggableProperty); }
        set { SetValue(DraggableProperty, value); }
    }

    public static readonly BindableProperty DraggableProperty =
        BindableProperty.Create(
            nameof(Draggable),
            typeof(bool),
            typeof(Pin)
            );
    #endregion

    #region ShowInfoWindowOnPinSelection
    public bool ShowInfoWindowOnPinSelection
    {
        get { return (bool)GetValue(ShowInfoWindowOnPinSelectionProperty); }
        set { SetValue(ShowInfoWindowOnPinSelectionProperty, value); }
    }

    public static readonly BindableProperty ShowInfoWindowOnPinSelectionProperty =
        BindableProperty.Create(
            nameof(ShowInfoWindowOnPinSelection),
            typeof(bool),
            typeof(Pin),
            true
            );
    #endregion

    #region CanBeSelected
    public bool CanBeSelected
    {
        get { return (bool)GetValue(CanBeSelectedProperty); }
        set { SetValue(CanBeSelectedProperty, value); }
    }

    public static readonly BindableProperty CanBeSelectedProperty =
        BindableProperty.Create(
            nameof(CanBeSelected),
            typeof(bool),
            typeof(Pin),
            true
            );
    #endregion

    #region InfoWindowAnchor
    public Point InfoWindowAnchor
    {
        get { return (Point)GetValue(InfoWindowAnchorProperty); }
        set { SetValue(InfoWindowAnchorProperty, value); }
    }

    public static readonly BindableProperty InfoWindowAnchorProperty =
        BindableProperty.Create(
            nameof(InfoWindowAnchor),
            typeof(Point),
            typeof(Pin),
            new Point(0.5, 0d)
            );
    #endregion

    #region IsFlat
    public bool IsFlat
    {
        get { return (bool)GetValue(IsFlatProperty); }
        set { SetValue(IsFlatProperty, value); }
    }

    public static readonly BindableProperty IsFlatProperty =
        BindableProperty.Create(
            nameof(IsFlat),
            typeof(bool),
            typeof(Pin)
            );
    #endregion

    #region Icon
    public ImageSource Icon
    {
        get { return (ImageSource)GetValue(IconProperty); }
        set { SetValue(IconProperty, value); }
    }

    public static readonly BindableProperty IconProperty =
        BindableProperty.Create(
            nameof(Icon),
            typeof(ImageSource),
            typeof(Pin)
            );
    #endregion

    #region DefaultIconColor
    public Color DefaultIconColor
    {
        get { return (Color)GetValue(DefaultIconColorProperty); }
        set { SetValue(DefaultIconColorProperty, value); }
    }

    public static readonly BindableProperty DefaultIconColorProperty =
        BindableProperty.Create(
            nameof(DefaultIconColor),
            typeof(Color),
            typeof(Pin)
            );
    #endregion

    #region InfoWindow
    public View InfoWindow
    {
        get { return (View)GetValue(InfoWindowProperty); }
        set { SetValue(InfoWindowProperty, value); }
    }

    public static readonly BindableProperty InfoWindowProperty =
        BindableProperty.Create(
            nameof(InfoWindow),
            typeof(View),
            typeof(Pin)
            );
    #endregion
}