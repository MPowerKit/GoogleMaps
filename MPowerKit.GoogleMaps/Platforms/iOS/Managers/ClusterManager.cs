using Google.Maps;

using UIKit;

using NPin = global::Google.Maps.Marker;
using VPin = MPowerKit.GoogleMaps.Pin;

namespace MPowerKit.GoogleMaps;

public class ClusterManager : PinManager
{
    protected override IEnumerable<VPin> VirtualViewItems =>
        VirtualView!.ClusterAlgorithm is ClusterAlgorithm.None
            ? VirtualView.Pins
            : VirtualView.Clusters;

    protected override string VirtualViewItemsPropertyName =>
        VirtualView!.ClusterAlgorithm is ClusterAlgorithm.None
            ? GoogleMap.PinsProperty.PropertyName
            : GoogleMap.ClustersProperty.PropertyName;

    protected override void Init(GoogleMap virtualView, MapView platformView, GoogleMapHandler handler)
    {
        base.Init(virtualView, platformView, handler);

        OnClusterAlgorithmChanged(virtualView, platformView);
    }

    protected override void VirtualViewPropertyChanged(GoogleMap virtualView, MapView platformView, string? propertyName)
    {
        base.VirtualViewPropertyChanged(virtualView, platformView, propertyName);

        if (propertyName == GoogleMap.ClusterInfoWindowTemplateProperty.PropertyName)
        {
            OnClusterInfoWindowTemplateChanged(virtualView, platformView);
        }
        else if (propertyName == GoogleMap.ClusterAlgorithmProperty.PropertyName)
        {
            OnClusterAlgorithmChanged(virtualView, platformView);
        }
        else if (propertyName == GoogleMap.ClusterIconTemplateProperty.PropertyName)
        {
            OnClusterIconTemplateChanged(virtualView, platformView);
        }
        else if (propertyName == GoogleMap.UseBucketsForClustersProperty.PropertyName)
        {
            OnUseBucketsForClustersChanged(virtualView, platformView);
        }
    }

    protected virtual void OnClusterAlgorithmChanged(GoogleMap virtualView, MapView platformView)
    {
        if (virtualView.PrevAlgorithm is ClusterAlgorithm.None
            && virtualView.ClusterAlgorithm is not ClusterAlgorithm.None)
        {
            OnClusterInfoWindowTemplateChanged(virtualView, platformView);
        }
    }

    protected virtual void OnClusterInfoWindowTemplateChanged(GoogleMap virtualView, MapView platformView)
    {
        OnInfoWindowTemplateChanged(virtualView, platformView);
    }

    protected virtual void OnClusterIconTemplateChanged(GoogleMap virtualView, MapView platformView)
    {
        foreach (var item in Items.OfType<Cluster>())
        {
            var nPin = NativeObjectAttachedProperty.GetNativeObject(item) as NPin;
            OnIconChanged(item, nPin);
        }
    }

    protected virtual void OnUseBucketsForClustersChanged(GoogleMap virtualView, MapView platformView)
    {
        OnClusterIconTemplateChanged(virtualView, platformView);
    }

    protected override void OnInfoWindowTemplateChanged(GoogleMap virtualView, MapView platformView)
    {
        platformView.MarkerInfoWindow = virtualView.ClusterInfoWindowTemplate is not null
            || virtualView.InfoWindowTemplate is not null ? GetInfoWindow : null;
    }

    protected override UIView? GetInfoWindow(MapView nMap, NPin marker)
    {
        var virtualView = VirtualView!;

        var pin = Items.Single(p => (NativeObjectAttachedProperty.GetNativeObject(p) as NPin) == marker);

        if (pin is not Cluster cluster || virtualView.ClusterAlgorithm is ClusterAlgorithm.None)
            return base.GetInfoWindow(nMap, marker);

        var template = virtualView.ClusterInfoWindowTemplate;

        while (template is DataTemplateSelector selector)
        {
            template = selector.SelectTemplate(cluster, virtualView);
        }

        if (template?.CreateContent() is not View view) return null;

        view.BindingContext = cluster;

        var platformView = view.ToNative(Handler!.MauiContext!);

        return platformView;
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
                foreach (var vpin in item.Items)
                {
                    var npin = base.AddItemToPlatformView(vpin);
                    AddItem(vpin, npin);
                }
                continue;
            }
            var native = AddItemToPlatformView(item);
            AddItem(item, native);
        }
    }

    protected override bool PlatformView_PinTapped(MapView map, NPin native)
    {
        var pin = Items.SingleOrDefault(p => NativeObjectAttachedProperty.GetNativeObject(p) == native);
        if (pin is null) return true;

        if (pin is not Cluster cluster)
        {
            return base.PlatformView_PinTapped(map, native);
        }

        VirtualView!.SendClusterClick(cluster);

        return true;
    }

    protected override void PlatformView_InfoTapped(object? sender, GMSMarkerEventEventArgs e)
    {
        var pin = Items.SingleOrDefault(p => NativeObjectAttachedProperty.GetNativeObject(p) == e.Marker);
        if (pin is null) return;

        if (pin is not Cluster cluster)
        {
            base.PlatformView_InfoTapped(sender, e);
            return;
        }

        VirtualView!.SendClusterInfoWindowClick(cluster);
    }

    protected override void PlatformView_InfoLongPressed(object? sender, GMSMarkerEventEventArgs e)
    {
        var pin = Items.SingleOrDefault(p => NativeObjectAttachedProperty.GetNativeObject(p) == e.Marker);
        if (pin is null) return;

        if (pin is not Cluster cluster)
        {
            base.PlatformView_InfoLongPressed(sender, e);
            return;
        }

        VirtualView!.SendClusterInfoWindowLongClick(cluster);
    }

    protected override void PlatformView_InfoClosed(object? sender, GMSMarkerEventEventArgs e)
    {
        var pin = Items.SingleOrDefault(p => NativeObjectAttachedProperty.GetNativeObject(p) == e.Marker);
        if (pin is null) return;

        if (pin is not Cluster cluster)
        {
            base.PlatformView_InfoClosed(sender, e);
            return;
        }

        VirtualView!.SendClusterInfoWindowClosed(cluster);
    }
}