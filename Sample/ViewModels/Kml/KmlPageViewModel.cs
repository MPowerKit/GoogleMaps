using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Controls.UserDialogs.Maui;

using Microsoft.Maui.Controls.Shapes;

using MPowerKit.GoogleMaps;

namespace Sample.ViewModels;

public partial class KmlPageViewModel : ObservableObject
{
    public KmlPageViewModel()
    {
        ChangeKmlSource();
    }

    [ObservableProperty]
    private IEnumerable<Pin> _pins;

    [ObservableProperty]
    private IEnumerable<GroundOverlay> _groundOverlays;

    [ObservableProperty]
    private IEnumerable<Polygon> _polygons;

    [ObservableProperty]
    private IEnumerable<Polyline> _polylines;

    [ObservableProperty]
    private Func<CameraUpdate, int, Task> _animateCameraFunc;

    [ObservableProperty]
    private string _kml;

    [RelayCommand]
    private async Task ChangeKmlSource()
    {
        await Task.Yield();
        var res = await UserDialogs.Instance.ActionSheetAsync(null,
            "Choose KML source",
            "Cancel",
            buttons:
            [
                "From Url",
                "From File",
                "From KMZ (zip)",
                "Raw XML"
            ]);

        if (res == "Cancel") return;

        UserDialogs.Instance.ShowLoading();

        Kml = res switch
        {
            "From Url" => "https://raw.githubusercontent.com/googlemaps/android-maps-utils/refs/heads/main/demo/src/main/res/raw/kmlgeometrytest.kml",
            "From File" => "sample.kml",
            "From KMZ (zip)" => "Africa.kmz",
            "Raw XML" => "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<kml xmlns=\"http://www.opengis.net/kml/2.2\">\n<GroundOverlay>\n<name>GroundOverlay.kml</name>\n<color>7fffffff</color>\n<drawOrder>1</drawOrder>\n<Icon>\n<href>http://www.google.com/intl/en/images/logo.gif</href>\n<refreshMode>onInterval</refreshMode>\n<refreshInterval>86400</refreshInterval>\n<viewBoundScale>0.75</viewBoundScale>\n</Icon>\n<LatLonBox>\n<north>37.83234</north>\n<south>37.832122</south>\n<east>-122.373033</east>\n<west>-122.373724</west>\n<rotation>45</rotation>\n</LatLonBox>\n</GroundOverlay>\n</kml>"
        };
    }

    [RelayCommand]
    private void KmlParsed()
    {
        UserDialogs.Instance.HideHud();

        List<Point> points = [];

        if (Pins?.Count() > 0)
        {
            points.AddRange(Pins.Select(p => p.Position));
        }

        if (Polygons?.Count() > 0)
        {
            points.AddRange(Polygons.SelectMany(p => p.Points));
        }

        if (Polylines?.Count() > 0)
        {
            points.AddRange(Polylines.SelectMany(p => p.Points));
        }

        if (GroundOverlays?.Count() > 0)
        {
            points.AddRange(GroundOverlays.SelectMany(p => p.OverlayBounds?.ToPoints() ?? []));
        }

        AnimateCameraFunc(CameraUpdateFactory.NewLatLngBounds(new LatLngBounds.Builder().Include(points).Build(), 100), 1000);
    }
}