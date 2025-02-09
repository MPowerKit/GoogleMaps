using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Controls.UserDialogs.Maui;

using Microsoft.Maui.Controls.Shapes;

using MPowerKit.GoogleMaps;

namespace Sample.ViewModels;

public partial class GeoJsonPageViewModel : ObservableObject
{
    public GeoJsonPageViewModel()
    {
        ChangeGeoJsonSource();
    }

    [ObservableProperty]
    private IEnumerable<Pin> _pins;

    [ObservableProperty]
    private IEnumerable<Polygon> _polygons;

    [ObservableProperty]
    private IEnumerable<Polyline> _polylines;

    [ObservableProperty]
    private Func<CameraUpdate, int, Task> _animateCameraFunc;

    [ObservableProperty]
    private string _geoJson;

    [RelayCommand]
    private async Task ChangeGeoJsonSource()
    {
        await Task.Yield();
        var res = await UserDialogs.Instance.ActionSheetAsync(null,
            "Choose GeoJSON source",
            "Cancel",
            buttons:
            [
                "From Url",
                "From File",
                "Raw JSON"
            ]);

        if (res == "Cancel") return;

        UserDialogs.Instance.ShowLoading();

        GeoJson = res switch
        {
            "From Url" => "https://raw.githubusercontent.com/googlemaps/android-maps-utils/refs/heads/main/demo/src/main/res/raw/earthquakes_with_usa.json",
            "From File" => "usa.json",
            "Raw JSON" => "{\n  \"type\": \"FeatureCollection\",\n  \"features\": [\n    {\n      \"type\": \"Feature\",\n      \"properties\": {\n        \"title\": \"This is titile\",\n        \"snippet\": \"This is snippet\",\n        \"icon\": \"https://picsum.photos/60/90\"\n      },\n      \"geometry\": {\n        \"coordinates\": [\n          14.987881318903788,\n          43.52412755667953\n        ],\n        \"type\": \"Point\"\n      },\n      \"id\": 0\n    },\n    {\n      \"type\": \"Feature\",\n      \"properties\": {\n        \"marker-color\": \"#05ff50\"\n      },\n      \"geometry\": {\n        \"coordinates\": [\n          10.183666282451469,\n          44.55391045584892\n        ],\n        \"type\": \"Point\"\n      },\n      \"id\": 1\n    }\n  ]\n}"
        };
    }

    [RelayCommand]
    private void GeoJsonParsed()
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

        AnimateCameraFunc(CameraUpdateFactory.NewLatLngBounds(new LatLngBounds.Builder().Include(points).Build(), 100), 1000);
    }
}