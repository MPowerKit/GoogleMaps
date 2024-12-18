using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Controls.UserDialogs.Maui;

using MPowerKit.GoogleMaps;

namespace Sample.ViewModels;

public partial class GroundOverlaysPageViewModel : ObservableObject
{
    private GroundOverlay _groundOverlay = new()
    {
        OverlayBounds = new(new(-20, -20), new(20, 20)),
        Image = "tile.png"
    };

    public GroundOverlaysPageViewModel()
    {
        SetupGroundOverlays();
    }

    [ObservableProperty]
    private ObservableCollection<GroundOverlay> _overlays = [];

    [ObservableProperty]
    private string _selectedOverlayPosition;

    [ObservableProperty]
    private string _selectedImageSource;

    [ObservableProperty]
    private bool _isVisible;

    [ObservableProperty]
    private bool _clickable;

    [ObservableProperty]
    private bool _anchorVisible;

    [ObservableProperty]
    private double _opacity;

    [ObservableProperty]
    private double _bearing;

    [ObservableProperty]
    private double _anchorX;

    [ObservableProperty]
    private double _anchorY;

    [ObservableProperty]
    private double _width;

    [ObservableProperty]
    private double _height;

    [ObservableProperty]
    private LatLngBounds? _bounds;

    [ObservableProperty]
    private Point? _position;

    private void SetupGroundOverlays()
    {
        Opacity = _groundOverlay.Opacity;
        _groundOverlay.SetBinding(GroundOverlay.OpacityProperty, new Binding(nameof(Opacity), source: this));
        Bearing = _groundOverlay.Bearing;
        _groundOverlay.SetBinding(GroundOverlay.BearingProperty, new Binding(nameof(Bearing), source: this));
        IsVisible = _groundOverlay.IsVisible;
        _groundOverlay.SetBinding(GroundOverlay.IsVisibleProperty, new Binding(nameof(IsVisible), source: this));
        Clickable = _groundOverlay.IsEnabled;
        _groundOverlay.SetBinding(GroundOverlay.IsEnabledProperty, new Binding(nameof(Clickable), source: this));
        AnchorX = _groundOverlay.AnchorX;
        _groundOverlay.SetBinding(GroundOverlay.AnchorXProperty, new Binding(nameof(AnchorX), source: this, mode: BindingMode.TwoWay));
        AnchorY = _groundOverlay.AnchorY;
        _groundOverlay.SetBinding(GroundOverlay.AnchorYProperty, new Binding(nameof(AnchorY), source: this, mode: BindingMode.TwoWay));

        _groundOverlay.SetBinding(GroundOverlay.OverlayBoundsProperty, new Binding(nameof(Bounds), source: this, mode: BindingMode.OneWayToSource));
        _groundOverlay.SetBinding(GroundOverlay.PositionProperty, new Binding(nameof(Position), source: this, mode: BindingMode.OneWayToSource));

        _groundOverlay.SetBinding(GroundOverlay.WidthRequestProperty, new Binding(nameof(Width), source: this, mode: BindingMode.OneWayToSource));
        _groundOverlay.SetBinding(GroundOverlay.HeightRequestProperty, new Binding(nameof(Height), source: this, mode: BindingMode.OneWayToSource));

        SelectedOverlayPosition = "From Bounds";
        SelectedImageSource = "File";

        ObservableCollection<GroundOverlay> overlays = [];
        overlays.Add(_groundOverlay);
        Overlays = overlays;
    }

    [RelayCommand]
    private async Task GroundOverlayClicked(GroundOverlay overlay)
    {
        await UserDialogs.Instance.AlertAsync("Ground overlay was clicked");
    }

    [RelayCommand]
    private async Task ChangeOverlayPosition()
    {
        var res = await UserDialogs.Instance.ActionSheetAsync(null,
            "Choose overlay positioning",
            "Cancel",
            buttons:
            [
                "From Bounds",
                "From Position",
#if ANDROID
                "Change dimensions"
#endif
            ]);

        if (res == "Cancel") return;

        var rnd = new Random();
        if (res == "From Bounds")
        {
            _groundOverlay.OverlayBounds = new(new(rnd.Next(-90, 0), rnd.Next(-180, 0)), new(rnd.Next(0, 90), rnd.Next(0, 180)));
        }
        else if (res == "From Position")
        {
            _groundOverlay.Position = new(rnd.Next(-90, 90), rnd.Next(-180, 180));
        }
        else
        {
            (_groundOverlay.WidthRequest, _groundOverlay.HeightRequest) = (Distance.FromKMeters(rnd.Next(200, 5000)), Distance.FromKMeters(rnd.Next(200, 5000)));
        }

        SelectedOverlayPosition = res;
    }

    [RelayCommand]
    private async Task ChangeOverlayImage()
    {
        var res = await UserDialogs.Instance.ActionSheetAsync(null,
            "Choose overlay image source",
            "Cancel",
            buttons:
            [
                "Url",
                "File",
                "Stream",
                "View"
            ]);

        if (res == "Cancel" || SelectedImageSource == res) return;

        var img = res switch
        {
            "Url" => ImageSource.FromUri(new Uri("https://picsum.photos/200/300")),
            "File" => ImageSource.FromFile("tile.png"),
            "Stream" => ImageSource.FromStream((ct) => FileSystem.Current.OpenAppPackageFileAsync("bot.png")),
            "View" => GetImageFromView()
        };

        _groundOverlay.Image = img;

        SelectedImageSource = res;
    }

    private ViewImageSource GetImageFromView()
    {
        var view = new ContentView()
        {
            BackgroundColor = Colors.Orange,
            Padding = 20,
            Content = new Label()
            {
                TextColor = Colors.Black,
                FontSize = 18,
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                Text = $"This is overlay's image from view.\r\nIt has Bounds={_groundOverlay.Position},\r\nIsVisible={IsVisible},\r\nBearing={Bearing:F1},\r\nOpacity={Opacity:F1}"
            }
        };

        return (ViewImageSource)view;
    }
}