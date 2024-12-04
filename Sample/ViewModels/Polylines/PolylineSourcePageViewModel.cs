using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.ViewModels
{
    public class PolylineTemplateSelector : DataTemplateSelector
    {
        public DataTemplate PolylineStandartTemplate { get; set; }
        public DataTemplate PolylineGradientTemplate { get; set; }
        public DataTemplate PolylineDashedTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is PolylineDataObject pData)
            {
                return pData.Type switch
                {
                    PolylineTemplateType.Standart => PolylineStandartTemplate,
                    PolylineTemplateType.Gradient => PolylineGradientTemplate,
                    PolylineTemplateType.Dashed => PolylineDashedTemplate
                };
            }

            return null;
        }
    }
    public enum PolylineTemplateType
    {
        Standart,
        Gradient,
        Dashed
    }
    public partial class PolylineDataObject : ObservableObject
    {
        [ObservableProperty]
        private double _width;
        [ObservableProperty]
        private Point[] _points;

        [ObservableProperty]
        private Brush _stroke;

        [ObservableProperty]
        private DoubleCollection _dashedData;

        [ObservableProperty]
        private PolylineTemplateType _type;
    }
    public partial class PolylineSourcePageViewModel : ObservableObject
    {
        private Dictionary<int, Color> _randomColors = new Dictionary<int, Color>()
        {
            {1, Colors.Green },
            {2, Colors.DimGray },
            {3, Colors.Goldenrod },
            {4, Colors.LightSlateGray },
            {5, Colors.MintCream },
            {6, Colors.MediumBlue },
            {7, Colors.Yellow },
            {8, Colors.Violet },
            {9, Colors.Tomato },
            {10, Colors.Sienna }
        };
        [ObservableProperty]
        private ObservableCollection<PolylineDataObject> _items = [];
        public PolylineSourcePageViewModel()
        {
            SetupItems();
        }

        private void SetupItems()
        {
            ObservableCollection<PolylineDataObject> items = [];
            Random rnd = new Random();
            int first = rnd.Next(1, 5);
            int second = rnd.Next(6, 10);
            var gradientStops = new GradientStopCollection();
            gradientStops.Add(new GradientStop(_randomColors[first], 0));
            gradientStops.Add(new GradientStop(_randomColors[second], 1));

            items.Add(new PolylineDataObject()
            {
                Width = 5,
                Type = PolylineTemplateType.Standart,
                Points = [new Point(5.877659, -13.010200), new Point(2.503409, -16.481879), new Point(-3.996456, -12.922309),]
            });
            items.Add(new PolylineDataObject()
            {
                Width = 5,
                Type = PolylineTemplateType.Dashed,
                Points = [new Point(4.126610, -5.935005), new Point(-0.176458, -8.308051), new Point(-8.885740, -10.988715),],
                DashedData = new DoubleCollection([10,8])
            });
            items.Add(new PolylineDataObject()
            {
                Width = 5,
                Type = PolylineTemplateType.Gradient,
                Points = [new Point(3.556607, 3.425346), new Point(-0.132513, -1.496528), new Point(-3.294758, 4.831596),],
                Stroke = new LinearGradientBrush(gradientStops)
            });

            Items = items!;
        }
    }
}
