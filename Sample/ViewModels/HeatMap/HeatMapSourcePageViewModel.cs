﻿using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using MPowerKit.GoogleMaps;

namespace Sample.ViewModels;

public class HeatMapTileTemplateSelector : DataTemplateSelector
{
    public DataTemplate OpaqueTemplate { get; set; }
    public DataTemplate TransparentTemplate { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        if (item is HeatMapData tile)
        {
            return tile.Opaque ? OpaqueTemplate : TransparentTemplate;
        }
        return null;
    }
}

public partial class HeatMapData : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Point> _coords = [];

    [ObservableProperty]
    private int _radius;

    [ObservableProperty]
    private float _intensity;

    [ObservableProperty]
    private Gradient _gradient;

    [ObservableProperty]
    private bool _opaque;
}

public partial class HeatMapSourcePageViewModel : ObservableObject
{
    public HeatMapSourcePageViewModel()
    {
        SetupHeatMaps();
    }

    [ObservableProperty]
    private ObservableCollection<HeatMapData> _items = [];

    private void SetupHeatMaps()
    {
        var first = new HeatMapData
        {
            Coords = new(_data),
            Opaque = true,
            Radius =
#if ANDROID
                50,
#else
                100,
#endif
        };

        Items.Add(first);

        var second = new HeatMapData()
        {
            Intensity = 3f,
            Gradient = new Gradient(_colors, _startPoints)
        };

        var minLatitude = -5d;
        var maxLatitude = 5d;
        var minLongitude = -5d;
        var maxLongitude = 5d;

        List<Point> data = [];

        var random = new Random();

        for (int i = 0; i < 1000; i++)
        {
            var latitude = minLatitude + (maxLatitude - minLatitude) * random.NextDouble();
            var longitude = minLongitude + (maxLongitude - minLongitude) * random.NextDouble();

            data.Add(new(latitude, longitude));
        }

        second.Coords = new(data);

        Items.Add(second);
    }

    private static readonly float[] _startPoints = new float[]
    {
        0.3f,
        0.32f,
        0.35f,
        0.4f,
        0.45f,
        0.5f,
        0.6f,
        0.7f,
        0.8f,
        0.9f,
        0.92f,
        0.95f,
        0.97f,
        1.0f
    };

    private static readonly Color[] _colors = new Color[]
    {
        Color.FromRgba(0, 255, 255, 0),
        Color.FromRgba(0, 255, 255, 255),
        Color.FromRgba(0, 255, 191, 255),
        Color.FromRgba(0, 255, 127, 255),
        Color.FromRgba(0, 255, 63, 255),
        Color.FromRgba(0, 255, 0, 255),
        Color.FromRgba(0, 223, 0, 255),
        Color.FromRgba(0, 191, 0, 255),
        Color.FromRgba(0, 159, 0, 255),
        Color.FromRgba(0, 127, 0, 255),
        Color.FromRgba(63, 91, 0, 255),
        Color.FromRgba(127, 63, 0, 255),
        Color.FromRgba(191, 31, 0, 255),
        Color.FromRgba(255, 0, 0, 255)
    };

    private static readonly List<Point> _data = new()
    {
        new (10.782551, -5.445368),
        new (10.782745, -5.444586),
        new (10.782842, -5.443688),
        new (10.782919, -5.442815),
        new (10.782992, -5.442112),
        new (10.7831, -5.441461),
        new (10.783206, -5.440829),
        new (10.783273, -5.440324),
        new (10.783316, -5.440023),
        new (10.783357, -5.439794),
        new (10.783371, -5.439687),
        new (10.783368, -5.439666),
        new (10.783383, -5.439594),
        new (10.783508, -5.439525),
        new (10.783842, -5.439591),
        new (10.784147, -5.439668),
        new (10.784206, -5.439686),
        new (10.784386, -5.43979),
        new (10.784701, -5.439902),
        new (10.784965, -5.439938),
        new (10.78501, -5.439947),
        new (10.78536, -5.439952),
        new (10.785715, -5.44003),
        new (10.786117, -5.440119),
        new (10.786564, -5.440209),
        new (10.786905, -5.44027),
        new (10.786956, -5.440279),
        new (10.800224, -5.43352),
        new (10.800155, -5.434101),
        new (10.80016, -5.43443),
        new (10.800378, -5.434527),
        new (10.800738, -5.434598),
        new (10.800938, -5.43465),
        new (10.801024, -5.434889),
        new (10.800955, -5.435392),
        new (10.800886, -5.435959),
        new (10.800811, -5.436275),
        new (10.800788, -5.436299),
        new (10.800719, -5.436302),
        new (10.800702, -5.436298),
        new (10.800661, -5.436273),
        new (10.800395, -5.436172),
        new (10.800228, -5.436116),
        new (10.800169, -5.43613),
        new (10.800066, -5.436167),
        new (10.784345, -5.422922),
        new (10.784389, -5.422926),
        new (10.784437, -5.422924),
        new (10.784746, -5.422818),
        new (10.785436, -5.422959),
        new (10.78612, -5.423112),
        new (10.786433, -5.423029),
        new (10.786631, -5.421213),
        new (10.78666, -5.421033),
        new (10.786801, -5.420141),
        new (10.786823, -5.420034),
        new (10.786831, -5.419916),
        new (10.787034, -5.418208),
        new (10.787056, -5.418034),
        new (10.787169, -5.417145),
        new (10.787217, -5.416715),
        new (10.786144, -5.416403),
        new (10.785292, -5.416257),
        new (10.780666, -5.390374),
        new (10.780501, -5.391281),
        new (10.780148, -5.392052),
        new (10.780173, -5.391148),
        new (10.780693, -5.390592),
        new (10.781261, -5.391142),
        new (10.781808, -5.39173),
        new (10.78234, -5.392341),
        new (10.782812, -5.393022),
        new (10.7833, -5.393672),
        new (10.783809, -5.394275),
        new (10.784246, -5.394979),
        new (10.784791, -5.395958),
        new (10.785675, -5.396746),
        new (10.786262, -5.39578),
        new (10.786776, -5.395093),
        new (10.787282, -5.394426),
        new (10.787783, -5.393767),
        new (10.788343, -5.393184),
        new (10.788895, -5.392506),
        new (10.789371, -5.391701),
        new (10.789722, -5.390952),
        new (10.790315, -5.390305),
        new (10.790738, -5.389616),
        new (10.779448, -5.438702),
        new (10.779023, -5.438585),
        new (10.778542, -5.438492),
        new (10.7781, -5.438411),
        new (10.777986, -5.438376),
        new (10.77768, -5.438313),
        new (10.777316, -5.438273),
        new (10.777135, -5.438254),
        new (10.776987, -5.438303),
        new (10.776946, -5.438404),
        new (10.776944, -5.438467),
        new (10.776892, -5.438459),
        new (10.776842, -5.438442),
        new (10.776822, -5.438391),
        new (10.776814, -5.438412),
        new (10.776787, -5.438628),
        new (10.776729, -5.43865),
        new (10.776759, -5.438677),
        new (10.776772, -5.438498),
        new (10.776787, -5.438389),
        new (10.776848, -5.438283),
        new (10.77687, -5.438239),
        new (10.777015, -5.438198),
        new (10.777333, -5.438256),
        new (10.777595, -5.438308),
        new (10.777797, -5.438344),
        new (10.77816, -5.438442),
        new (10.778414, -5.438508),
        new (10.778445, -5.438516),
        new (10.778503, -5.438529),
        new (10.778607, -5.438549),
        new (10.77867, -5.438644),
        new (10.778847, -5.438706),
        new (10.77924, -5.438744),
        new (10.779738, -5.438822),
        new (10.780201, -5.438882),
        new (10.7804, -5.438905),
        new (10.780501, -5.438921),
        new (10.780892, -5.438986),
        new (10.781446, -5.439087),
        new (10.781985, -5.439199),
        new (10.782239, -5.439249),
        new (10.782286, -5.439266),
        new (10.797847, -5.429388),
        new (10.797874, -5.42918),
        new (10.797885, -5.429069),
        new (10.797887, -5.42905),
        new (10.797933, -5.428954),
        new (10.798242, -5.42899),
        new (10.798617, -5.429075),
        new (10.798719, -5.429092),
        new (10.798944, -5.429145),
        new (10.79932, -5.429251),
        new (10.79959, -5.429309),
        new (10.799677, -5.429324),
        new (10.799966, -5.42936),
        new (10.800288, -5.42943),
        new (10.800443, -5.429461),
        new (10.800465, -5.429474),
        new (10.800644, -5.42954),
        new (10.800948, -5.42962),
        new (10.801242, -5.429685),
        new (10.801375, -5.429702),
        new (10.8014, -5.429703),
        new (10.801453, -5.429707),
        new (10.801473, -5.429709),
        new (10.801532, -5.429707),
        new (10.801852, -5.429729),
        new (10.802173, -5.429789),
        new (10.802459, -5.429847),
        new (10.802554, -5.429825),
        new (10.802647, -5.429549),
        new (10.802693, -5.429179),
        new (10.802729, -5.428751),
        new (10.766104, -5.409291),
        new (10.766103, -5.409268),
        new (10.766138, -5.409229),
        new (10.766183, -5.409231),
        new (10.766153, -5.409276),
        new (10.766005, -5.409365),
        new (10.765897, -5.40957),
        new (10.765767, -5.409739),
        new (10.765693, -5.410389),
        new (10.765615, -5.411201),
        new (10.765533, -5.412121),
        new (10.765467, -5.412939),
        new (10.765444, -5.414821),
        new (10.765444, -5.414964),
        new (10.765318, -5.415424),
        new (10.763961, -5.415296),
        new (10.763115, -5.415196),
        new (10.762967, -5.415183),
        new (10.762278, -5.415127),
        new (10.761675, -5.415055),
        new (10.760932, -5.414988),
        new (10.759337, -5.414862),
        new (10.773187, -5.421922),
        new (10.773043, -5.422118),
        new (10.773007, -5.422165),
        new (10.772979, -5.422219),
        new (10.772865, -5.422394),
        new (10.772779, -5.422503),
        new (10.772676, -5.422701),
        new (10.772606, -5.422806),
        new (10.772566, -5.42284),
        new (10.772508, -5.422852),
        new (10.772387, -5.423011),
        new (10.772099, -5.423328),
        new (10.771704, -5.423783),
        new (10.771481, -5.424081),
        new (10.7714, -5.424179),
        new (10.771352, -5.42422),
        new (10.771248, -5.424327),
        new (10.770904, -5.424781),
        new (10.77052, -5.425283),
        new (10.770337, -5.425553),
        new (10.770128, -5.425832),
        new (10.769756, -5.426331),
        new (10.7693, -5.426902),
        new (10.769132, -5.427065),
        new (10.769092, -5.427103),
        new (10.768979, -5.427172),
        new (10.768595, -5.427634),
        new (10.768372, -5.427913),
        new (10.768337, -5.427961),
        new (10.768244, -5.428138),
        new (10.767942, -5.428581),
        new (10.767482, -5.429094),
        new (10.767031, -5.429606),
        new (10.766732, -5.429986),
        new (10.76668, -5.430058),
        new (10.766633, -5.430109),
        new (10.76658, -5.430211),
        new (10.766367, -5.430594),
        new (10.76591, -5.431137),
        new (10.765353, -5.431806),
        new (10.764962, -5.432298),
        new (10.764868, -5.432486),
        new (10.764518, -5.432913),
        new (10.763435, -5.434173),
        new (10.762847, -5.434953),
        new (10.762291, -5.435935),
        new (10.762224, -5.436074),
        new (10.761957, -5.436892),
        new (10.761652, -5.438886),
        new (10.761284, -5.439955),
        new (10.76121, -5.440068),
        new (10.761064, -5.44072),
        new (10.76104, -5.441411),
        new (10.761048, -5.442324),
        new (10.760851, -5.443118),
        new (10.759977, -5.444591),
        new (10.759913, -5.444698),
        new (10.759623, -5.445065),
        new (10.758902, -5.445158),
        new (10.758428, -5.44457),
        new (10.757687, -5.44334),
        new (10.757583, -5.44324),
        new (10.757019, -5.442787),
        new (10.756603, -5.442322),
        new (10.75638, -5.441602),
        new (10.75579, -5.441382),
        new (10.754493, -5.442133),
        new (10.754361, -5.442206),
        new (10.753719, -5.44265),
        new (10.753096, -5.442915),
        new (10.751617, -5.443211),
        new (10.751496, -5.443246),
        new (10.750733, -5.443428),
        new (10.750126, -5.443536),
        new (10.750103, -5.443784),
        new (10.75039, -5.44401),
        new (10.750448, -5.444013),
        new (10.750536, -5.44404),
        new (10.750493, -5.444141),
        new (10.790859, -5.402808),
        new (10.790864, -5.402768),
        new (10.790995, -5.402539),
        new (10.791148, -5.402172),
        new (10.791385, -5.401312),
        new (10.791405, -5.400776),
        new (10.791288, -5.400528),
        new (10.791113, -5.400441),
        new (10.791027, -5.400395),
        new (10.791094, -5.400311),
        new (10.791211, -5.400183),
        new (10.79106, -5.399334),
        new (10.790538, -5.398718),
        new (10.790095, -5.398086),
        new (10.789644, -5.39736),
        new (10.789254, -5.396844),
        new (10.788855, -5.396397),
        new (10.788483, -5.395963),
        new (10.788015, -5.395365),
        new (10.787558, -5.394735),
        new (10.787472, -5.394323),
        new (10.78763, -5.394025),
        new (10.787767, -5.393987),
        new (10.787486, -5.394452),
        new (10.786977, -5.395043),
        new (10.786583, -5.395552),
        new (10.78654, -5.39561),
        new (10.786516, -5.395659),
        new (10.786378, -5.395707),
        new (10.786044, -5.395362),
        new (10.785598, -5.394715),
        new (10.785321, -5.394361),
        new (10.785207, -5.394236),
        new (10.785751, -5.394062),
        new (10.785996, -5.393881),
        new (10.786092, -5.39383),
        new (10.785998, -5.393899),
        new (10.785114, -5.394365),
        new (10.785022, -5.394441),
        new (10.784823, -5.394635),
        new (10.784719, -5.394629),
        new (10.785069, -5.394176),
        new (10.7855, -5.39365),
        new (10.78577, -5.393291),
        new (10.785839, -5.393159),
        new (10.782651, -5.400628),
        new (10.782616, -5.400599),
        new (10.782702, -5.40047),
        new (10.782915, -5.400192),
        new (10.783137, -5.399887),
        new (10.783414, -5.399519),
        new (10.783629, -5.399237),
        new (10.783688, -5.399157),
        new (10.783716, -5.399106),
        new (10.783798, -5.399072),
        new (10.783997, -5.399186),
        new (10.784271, -5.399538),
        new (10.784577, -5.399948),
        new (10.784828, -5.40026),
        new (10.784999, -5.400477),
        new (10.785113, -5.400651),
        new (10.785155, -5.400703),
        new (10.785192, -5.400749),
        new (10.785278, -5.400839),
        new (10.785387, -5.400857),
        new (10.785478, -5.40089),
        new (10.785526, -5.401022),
        new (10.785598, -5.401148),
        new (10.785631, -5.401202),
        new (10.78566, -5.401267),
        new (10.803986, -5.426035),
        new (10.804102, -5.425089),
        new (10.804211, -5.424156),
        new (10.803861, -5.423385),
        new (10.803151, -5.423214),
        new (10.802439, -5.423077),
        new (10.80174, -5.422905),
        new (10.801069, -5.422785),
        new (10.800345, -5.422649),
        new (10.799633, -5.422603),
        new (10.79975, -5.4217),
        new (10.799885, -5.420854),
        new (10.799209, -5.420607),
        new (10.795656, -5.400395),
        new (10.795203, -5.400304),
        new (10.778738, -5.415584),
        new (10.778812, -5.415189),
        new (10.778824, -5.415092),
        new (10.778833, -5.414932),
        new (10.778834, -5.414898),
        new (10.77874, -5.414757),
        new (10.778501, -5.414433),
        new (10.778182, -5.414026),
        new (10.777851, -5.413623),
        new (10.777486, -5.413166),
        new (10.777109, -5.412674),
        new (10.776743, -5.412186),
        new (10.77644, -5.4118),
        new (10.776295, -5.411614),
        new (10.776158, -5.41144),
        new (10.775806, -5.410997),
        new (10.775422, -5.410484),
        new (10.775126, -5.410087),
        new (10.775012, -5.409854),
        new (10.775164, -5.409573),
        new (10.775498, -5.40918),
        new (10.775868, -5.40873),
        new (10.776256, -5.40824),
        new (10.776519, -5.407928),
        new (10.776539, -5.407904),
        new (10.776595, -5.407854),
        new (10.776853, -5.407547),
        new (10.777234, -5.407087),
        new (10.777644, -5.406558),
        new (10.778066, -5.406017),
        new (10.778468, -5.405499),
        new (10.778866, -5.404995),
        new (10.779295, -5.404455),
        new (10.779695, -5.40395),
        new (10.779982, -5.403584),
        new (10.780295, -5.403223),
        new (10.780664, -5.402766),
        new (10.781043, -5.402288),
        new (10.781399, -5.401823),
        new (10.781727, -5.401407),
        new (10.781853, -5.401247),
        new (10.781894, -5.401195),
        new (10.782076, -5.400977),
        new (10.782338, -5.400603),
        new (10.782666, -5.400133),
        new (10.783048, -5.399634),
        new (10.78345, -5.399198),
        new (10.783791, -5.398998),
        new (10.784177, -5.398959),
        new (10.784388, -5.398971),
        new (10.784404, -5.399128),
        new (10.784586, -5.399524),
        new (10.784835, -5.399927),
        new (10.785116, -5.400307),
        new (10.785282, -5.400539),
        new (10.785346, -5.400692),
        new (10.765769, -5.407201),
        new (10.76579, -5.407414),
        new (10.765802, -5.407755),
        new (10.765791, -5.408219),
        new (10.765763, -5.408759),
        new (10.765726, -5.409348),
        new (10.765716, -5.409882),
        new (10.765708, -5.410202),
        new (10.765705, -5.410253),
        new (10.765707, -5.410369),
        new (10.765692, -5.41072),
        new (10.765699, -5.411215),
        new (10.765687, -5.411789),
        new (10.765666, -5.412373),
        new (10.765598, -5.412883),
        new (10.765543, -5.413039),
        new (10.765532, -5.413125),
        new (10.7655, -5.413553),
        new (10.765448, -5.414053),
        new (10.765388, -5.414645),
        new (10.765323, -5.41525),
        new (10.765303, -5.415847),
        new (10.765251, -5.416439),
        new (10.765204, -5.41702),
        new (10.765172, -5.417556),
        new (10.765164, -5.418075),
        new (10.765153, -5.418618),
        new (10.765136, -5.419112),
        new (10.765129, -5.419378),
        new (10.765119, -5.419481),
        new (10.7651, -5.419852),
        new (10.765083, -5.420349),
        new (10.765045, -5.42093),
        new (10.764992, -5.421481),
        new (10.76498, -5.421695),
        new (10.764993, -5.421843),
        new (10.764986, -5.422255),
        new (10.764975, -5.422823),
        new (10.764939, -5.423411),
        new (10.764902, -5.424014),
        new (10.764853, -5.424576),
        new (10.764826, -5.424922),
        new (10.764796, -5.425375),
        new (10.764782, -5.425869),
        new (10.764768, -5.426089),
        new (10.764766, -5.426117),
        new (10.764723, -5.426276),
        new (10.764681, -5.426649),
        new (10.782012, -5.4042),
        new (10.781574, -5.404911),
        new (10.781055, -5.405597),
        new (10.780479, -5.406341),
        new (10.779996, -5.406939),
        new (10.779459, -5.407613),
        new (10.778953, -5.408228),
        new (10.778409, -5.408839),
        new (10.777842, -5.409501),
        new (10.777334, -5.410181),
        new (10.776809, -5.410836),
        new (10.77624, -5.411514),
        new (10.775725, -5.412145),
        new (10.77519, -5.412805),
        new (10.774672, -5.413464),
        new (10.774084, -5.414186),
        new (10.773533, -5.413636),
        new (10.773021, -5.413009),
        new (10.772501, -5.412371),
        new (10.771964, -5.411681),
        new (10.771479, -5.411078),
        new (10.770992, -5.410477),
        new (10.770467, -5.409801),
        new (10.77009, -5.408904),
        new (10.769657, -5.408103),
        new (10.769132, -5.407276),
        new (10.768564, -5.406469),
        new (10.76798, -5.405745),
        new (10.76738, -5.405299),
        new (10.766604, -5.405297),
        new (10.765838, -5.4052),
        new (10.765139, -5.405139),
        new (10.764457, -5.405094),
        new (10.763716, -5.405142),
        new (10.762932, -5.405398),
        new (10.762126, -5.405813),
        new (10.761344, -5.406215),
        new (10.760556, -5.406495),
        new (10.759732, -5.406484),
        new (10.75891, -5.406228),
        new (10.758182, -5.405695),
        new (10.757676, -5.405118),
        new (10.757039, -5.404346),
        new (10.756335, -5.403719),
        new (10.755503, -5.403406),
        new (10.754665, -5.403242),
        new (10.753837, -5.403172),
        new (10.752986, -5.403112),
        new (10.751266, -5.403355),
    };
}