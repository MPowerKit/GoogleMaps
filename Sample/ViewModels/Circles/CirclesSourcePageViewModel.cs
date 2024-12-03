using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

namespace Sample.ViewModels;

public partial class CircleDataObject : ObservableObject
{
    [ObservableProperty]
    private Sample.Models.Location _location;


}

public partial class CirclesSourcePageViewModel : ObservableObject
{
    public CirclesSourcePageViewModel()
    {
    }

    [ObservableProperty]
    private ObservableCollection<CircleDataObject> _items = [];

    private void SetupItems()
    {

    }
}