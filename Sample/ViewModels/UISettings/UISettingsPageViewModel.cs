using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.ViewModels
{
    public partial class UISettingsPageViewModel : ObservableObject
    {
        public UISettingsPageViewModel()
        {
            
        }

        [ObservableProperty]
        private double _compassEnabled;
        [ObservableProperty]
        private double _mapToolbarEnabled;
        [ObservableProperty]
        private double _zoomControlsEnabled;
        [ObservableProperty]
        private double _zoomGesturesEnabled;
        [ObservableProperty]
        private double _scrollGesturesEnabled;
        [ObservableProperty]
        private double _tiltGesturesEnabled;
        [ObservableProperty]
        private double _rotateGesturesEnabled;
        [ObservableProperty]
        private double _myLocationButtonEnabled;
        [ObservableProperty]
        private double _indoorLevelPickerEnabled;
    }
}
