using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Controls.UserDialogs.Maui;

using MPowerKit.GoogleMaps;

namespace Sample.ViewModels;

public partial class PinsPageViewModel : ObservableObject
{
    private readonly Pin _pin = new() { Title = "Default pin", Snippet = "Default pin description" };

    public PinsPageViewModel()
    {
        SetupPins();
    }

    [ObservableProperty]
    private bool _defaultIcon = true;
    [ObservableProperty]
    private string _snippet;
    [ObservableProperty]
    private string _title;
    [ObservableProperty]
    private bool _draggable;
    [ObservableProperty]
    private bool _canBeSelected;
    [ObservableProperty]
    private bool _showInfoWindowOnPinSelection;
    [ObservableProperty]
    private bool _isFlat;
    [ObservableProperty]
    private double _rotation;
    [ObservableProperty]
    private bool _clickable;
    [ObservableProperty]
    private bool _isVisible;
    [ObservableProperty]
    private double _opacity;

    [ObservableProperty]
    private double _positionX;
    partial void OnPositionXChanged(double oldValue, double newValue)
    {
        _pin.Position = new Point(newValue, _pin.Position.Y);
    }

    [ObservableProperty]
    private double _positionY;
    partial void OnPositionYChanged(double oldValue, double newValue)
    {
        _pin.Position = new Point(_pin.Position.X, newValue);
    }

    [ObservableProperty]
    private double _anchorX;

    [ObservableProperty]
    private double _anchorY;

    [ObservableProperty]
    private double _infoWindowAnchorX;
    partial void OnInfoWindowAnchorXChanged(double oldValue, double newValue)
    {
        _pin.InfoWindowAnchor = new Point(newValue, _pin.InfoWindowAnchor.Y);
    }

    [ObservableProperty]
    private double _infoWindowAnchorY;
    partial void OnInfoWindowAnchorYChanged(double oldValue, double newValue)
    {
        _pin.InfoWindowAnchor = new Point(_pin.InfoWindowAnchor.X, newValue);
    }

    [ObservableProperty]
    private ObservableCollection<Pin> _pins = [];

    [RelayCommand]
    private void RandomizeIconColor()
    {
        var rnd = new Random();
        var red = (float)rnd.NextDouble();
        var green = (float)rnd.NextDouble();
        var blue = (float)rnd.NextDouble();

        _pin.DefaultIconColor = Color.FromRgb(red, green, blue);
    }

    [RelayCommand]
    private async Task ChangeIcon()
    {
        var res = await UserDialogs.Instance.ActionSheetAsync(null,
            "Choose pin icon",
            "Cancel",
            buttons:
            [
                "Default",
                "From images (MauiImage)",
                "From view",
                "From url"
            ]);

        switch (res)
        {
            case "Default":
                _pin.Icon = null;
                break;
            case "From images (MauiImage)":
                _pin.Icon = "map_pin.png";
                break;
            case "From view":
                _pin.Icon = (ViewImageSource)new ContentView() { Content = new Label() { Text = "This is pin from view", TextColor = Colors.Red } };
                break;
            case "From url":
                _pin.Icon = "https://picsum.photos/60/90";
                break;
        }

        DefaultIcon = _pin.Icon is null;
    }

    private void SetupPins()
    {
        Snippet = _pin.Snippet;
        _pin.SetBinding(Pin.SnippetProperty, new Binding(nameof(Snippet), source: this));
        Title = _pin.Title;
        _pin.SetBinding(Pin.TitleProperty, new Binding(nameof(Title), source: this));
        Draggable = _pin.Draggable;
        _pin.SetBinding(Pin.DraggableProperty, new Binding(nameof(Draggable), source: this));
        CanBeSelected = _pin.CanBeSelected;
        _pin.SetBinding(Pin.CanBeSelectedProperty, new Binding(nameof(CanBeSelected), source: this));
        ShowInfoWindowOnPinSelection = _pin.ShowInfoWindowOnPinSelection;
        _pin.SetBinding(Pin.ShowInfoWindowOnPinSelectionProperty, new Binding(nameof(ShowInfoWindowOnPinSelection), source: this));
        IsFlat = _pin.IsFlat;
        _pin.SetBinding(Pin.IsFlatProperty, new Binding(nameof(IsFlat), source: this));
        Rotation = _pin.Rotation;
        _pin.SetBinding(Pin.RotationProperty, new Binding(nameof(Rotation), source: this));
        Clickable = _pin.IsEnabled;
        _pin.SetBinding(Pin.IsEnabledProperty, new Binding(nameof(Clickable), source: this));
        IsVisible = _pin.IsVisible;
        _pin.SetBinding(Pin.IsVisibleProperty, new Binding(nameof(IsVisible), source: this));
        Opacity = _pin.Opacity;
        _pin.SetBinding(Pin.OpacityProperty, new Binding(nameof(Opacity), source: this));
        AnchorX = _pin.AnchorX;
        _pin.SetBinding(Pin.AnchorXProperty, new Binding(nameof(AnchorX), source: this, mode: BindingMode.TwoWay));
        AnchorY = _pin.AnchorY;
        _pin.SetBinding(Pin.AnchorYProperty, new Binding(nameof(AnchorY), source: this, mode: BindingMode.TwoWay));
        PositionX = _pin.Position.X;
        PositionY = _pin.Position.Y;
        InfoWindowAnchorX = _pin.InfoWindowAnchor.X;
        InfoWindowAnchorY = _pin.InfoWindowAnchor.Y;

        _pin.PropertyChanged += _pin_PropertyChanged;

        ObservableCollection<Pin> pins = [];
        pins.Add(_pin);
        Pins = pins;
    }

    private void _pin_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == Pin.PositionProperty.PropertyName)
        {
            PositionX = _pin.Position.X;
            PositionY = _pin.Position.Y;
        }
        else if (e.PropertyName == Pin.InfoWindowAnchorProperty.PropertyName)
        {
            InfoWindowAnchorX = _pin.InfoWindowAnchor.X;
            InfoWindowAnchorY = _pin.InfoWindowAnchor.Y;
        }
    }
}