//using CommunityToolkit.Mvvm.ComponentModel;
//using CommunityToolkit.Mvvm.Input;
//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Sample.ViewModels
//{
//    public class PolylineTemplateSelector : DataTemplateSelector
//    {
//        public DataTemplate PolylineStandartTemplate { get; set; }
//        public DataTemplate PolylineGradientTemplate { get; set; }
//        public DataTemplate PolylineDashedTemplate { get; set; }

//        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
//        {
//            if (item is PolylineDataObject pData)
//            {
//                return pData.Type switch
//                {
//                    PolylineTemplateType.Standart => PolylineStandartTemplate,
//                    PolylineTemplateType.Gradient => PolylineGradientTemplate,
//                    PolylineTemplateType.Dashed => PolylineDashedTemplate
//                };
//            }

//            return null;
//        }
//    }
//    public enum PolylineTemplateType
//    {
//        Standart,
//        Gradient,
//        Dashed
//    }
//    public partial class PolylineDataObject : ObservableObject
//    {
//        [ObservableProperty]
//        private double _width;
//        [ObservableProperty]
//        private int _zIndex;
//        [ObservableProperty]
//        private Point[] _points;

//        [ObservableProperty]
//        private Brush _stroke;

//        [ObservableProperty]
//        private DoubleCollection _dashedData;

//        [ObservableProperty]
//        private PolylineTemplateType _type;
//    }
//    public partial class PolylineSourcePageViewModel : ObservableObject
//    {
//        private Dictionary<int, Color> _randomColors = new Dictionary<int, Color>()
//        {
//            {1, Colors.Green },
//            {2, Colors.DimGray },
//            {3, Colors.Goldenrod },
//            {4, Colors.LightSlateGray },
//            {5, Colors.MintCream },
//            {6, Colors.MediumBlue },
//            {7, Colors.Yellow },
//            {8, Colors.Violet },
//            {9, Colors.Tomato },
//            {10, Colors.Sienna }
//        };
//        [ObservableProperty]
//        private ObservableCollection<PolylineDataObject> _items = [];
//        [RelayCommand]
//        private void ChangeZIndex()
//        {
//            PolylineDataObject first = _items[0];
//            int one = first.ZIndex;
//            PolylineDataObject second = _items[1];
//            int two = second.ZIndex;
//            PolylineDataObject third = _items[2];
//            int three = third.ZIndex;
//            first.ZIndex = three;
//            second.ZIndex = one;
//            third.ZIndex = two;
//        }
//        public PolylineSourcePageViewModel()
//        {
//            SetupItems();
//        }

//        private void SetupItems()
//        {
//            ObservableCollection<PolylineDataObject> items = [];
//            Random rnd = new Random();
//            int first = rnd.Next(1, 5);
//            int second = rnd.Next(6, 10);
//            var gradientStops = new GradientStopCollection();
//            gradientStops.Add(new GradientStop(_randomColors[first], 0));
//            gradientStops.Add(new GradientStop(_randomColors[second], 1));

//            items.Add(new PolylineDataObject()
//            {
//                ZIndex = 0,
//                Width = 5,
//                Type = PolylineTemplateType.Standart,
//                Stroke = new SolidColorBrush(Colors.Green),
//                Points = [new Point(6.573118, -14.371261), new Point(0.379401, -22.353207), new Point(6.793332, -29.669991),]
//            });
//            items.Add(new PolylineDataObject()
//            {
//                ZIndex = 1,
//                Width = 5,
//                Type = PolylineTemplateType.Dashed,
//                Points = [new Point(-1.615869, -29.448270), new Point(5.397074, -23.609624), new Point(-1.246450, -16.588468),],
//                DashedData = new DoubleCollection([10,8])
//            });
//            items.Add(new PolylineDataObject()
//            {
//                Width = 5,
//                Type = PolylineTemplateType.Gradient,
//                Points = [new Point(9.062395, -23.240090), new Point(-2.502170, -14.223447), new Point(-8.019669, -25.752925),],
//                Stroke = new LinearGradientBrush(gradientStops)
//            });

//            Items = items!;
//        }
//    }
//}
