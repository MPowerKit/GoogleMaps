using GMap = global::Android.Gms.Maps.GoogleMap;
using NPin = global::Android.Gms.Maps.Model.Marker;
using VPin = MPowerKit.GoogleMaps.Pin;

namespace MPowerKit.GoogleMaps;

public class ClusterManager : PinManager
{
    protected override IEnumerable<VPin>? VirtualViewItems =>
        VirtualView!.ClusterAlgorithm is ClusterAlgorithm.None
            ? VirtualView.Pins
            : VirtualView.Clusters;

    protected override string VirtualViewItemsPropertyName =>
        VirtualView!.ClusterAlgorithm is ClusterAlgorithm.None
            ? GoogleMap.PinsProperty.PropertyName
            : GoogleMap.ClustersProperty.PropertyName;

    protected override void Init(GoogleMap virtualView, GMap platformView, GoogleMapHandler handler)
    {
        base.Init(virtualView, platformView, handler);

        platformView.SetInfoWindowAdapter(new ClusterInfoWindowAdapter(virtualView, () => Items));
    }

    protected override void VirtualViewPropertyChanged(GoogleMap virtualView, GMap platformView, string? propertyName)
    {
        base.VirtualViewPropertyChanged(virtualView, platformView, propertyName);

        if (propertyName == GoogleMap.ClusterIconTemplateProperty.PropertyName)
        {
            OnClusterIconTemplateChanged(virtualView, platformView);
        }
        else if (propertyName == GoogleMap.UseBucketsForClustersProperty.PropertyName)
        {
            OnUseBucketsForClustersChanged(virtualView, platformView);
        }
    }

    protected virtual void OnClusterIconTemplateChanged(GoogleMap virtualView, GMap platformView)
    {
        foreach (var item in Items.OfType<Cluster>())
        {
            var nPin = NativeObjectAttachedProperty.GetNativeObject(item) as NPin;
            OnIconChanged(item, nPin);
        }
    }

    protected virtual void OnUseBucketsForClustersChanged(GoogleMap virtualView, GMap platformView)
    {
        OnClusterIconTemplateChanged(virtualView, platformView);
    }

    protected override void AddItemsToPlatformView(IEnumerable<VPin> items)
    {
        var virtualView = VirtualView!;
        var algo = virtualView.ClusterAlgorithm;
        if (algo is ClusterAlgorithm.None)
        {
            base.AddItemsToPlatformView(items);
            return;
        }

        var minClusterSize = virtualView.MinClusterSize;
        foreach (var item in items.OfType<Cluster>())
        {
            if (item.Size < minClusterSize)
            {
                foreach (var vPin in item.Items)
                {
                    var nPin = base.AddItemToPlatformView(vPin);
                    AddItem(vPin, nPin);
                }
                continue;
            }
            var native = AddItemToPlatformView(item);
            AddItem(item, native);
        }
    }

    protected override void PlatformView_PinClick(object? sender, GMap.MarkerClickEventArgs e)
    {
        e.Handled = true;

        var pin = Items.SingleOrDefault(p => (NativeObjectAttachedProperty.GetNativeObject(p) as NPin)!.Id == e.Marker.Id);
        if (pin is null) return;

        if (pin is not Cluster cluster)
        {
            base.PlatformView_PinClick(sender, e);
            return;
        }

        if (!cluster.IsEnabled) return;

        VirtualView!.SendClusterClick(cluster);
    }

    protected override void PlatformView_InfoWindowClick(object? sender, GMap.InfoWindowClickEventArgs e)
    {
        var pin = Items.SingleOrDefault(p => (NativeObjectAttachedProperty.GetNativeObject(p) as NPin)!.Id == e.Marker.Id);
        if (pin is null) return;

        if (pin is not Cluster cluster)
        {
            base.PlatformView_InfoWindowClick(sender, e);
            return;
        }

        VirtualView!.SendClusterInfoWindowClick(cluster);
    }

    protected override void PlatformView_InfoWindowLongClick(object? sender, GMap.InfoWindowLongClickEventArgs e)
    {
        var pin = Items.SingleOrDefault(p => (NativeObjectAttachedProperty.GetNativeObject(p) as NPin)!.Id == e.Marker.Id);
        if (pin is null) return;

        if (pin is not Cluster cluster)
        {
            base.PlatformView_InfoWindowLongClick(sender, e);
            return;
        }

        VirtualView!.SendClusterInfoWindowLongClick(cluster);
    }

    protected override void PlatformView_InfoWindowClose(object? sender, GMap.InfoWindowCloseEventArgs e)
    {
        var pin = Items.SingleOrDefault(p => (NativeObjectAttachedProperty.GetNativeObject(p) as NPin)!.Id == e.Marker.Id);
        if (pin is null) return;

        if (pin is not Cluster cluster)
        {
            base.PlatformView_InfoWindowClose(sender, e);
            return;
        }

        VirtualView!.SendClusterInfoWindowClosed(cluster);
    }
}

public class ClusterInfoWindowAdapter : InfoWindowAdapter
{
    public ClusterInfoWindowAdapter(GoogleMap map, Func<IEnumerable<VPin>> getPins)
        : base(map, getPins)
    {
    }

    public override global::Android.Views.View? GetInfoWindow(NPin marker)
    {
        var pin = GetPins().Single(p => (NativeObjectAttachedProperty.GetNativeObject(p) as NPin)!.Id == marker.Id);

        if (pin is not Cluster cluster || Map.ClusterAlgorithm is ClusterAlgorithm.None)
            return base.GetInfoWindow(marker);

        var template = Map.ClusterInfoWindowTemplate;

        while (template is DataTemplateSelector selector)
        {
            template = selector.SelectTemplate(cluster, Map);
        }

        if (template?.CreateContent() is not View view) return null;

        view.BindingContext = cluster;

        var virtualView = Map;
        view.MaximumWidthRequest = virtualView.Width;
        view.MaximumHeightRequest = virtualView.Height / 2d;

        var platformView = view.ToNative(Map.Handler!.MauiContext!);

        return platformView;
    }
}