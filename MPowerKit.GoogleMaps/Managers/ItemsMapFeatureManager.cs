using System.Collections.Specialized;

namespace MPowerKit.GoogleMaps;

public abstract class ItemsMapFeatureManager<TVItem, TNItem, TNMap>
    : MapFeatureManager<TNMap>
    where TVItem : VisualElement
    where TNItem : class
{
    protected List<TVItem> Items { get; set; } = [];

    protected abstract IEnumerable<TVItem> VirtualViewItems { get; }
    protected abstract string VirtualViewItemsPropertyName { get; }

    protected override void Init(GoogleMap virtualView, TNMap platformView, GoogleMapHandler handler)
    {
        base.Init(virtualView, platformView, handler);

        InitItems();
    }

    protected override void Reset(GoogleMap virtualView, TNMap platformView, GoogleMapHandler handler)
    {
        ClearItems();

        base.Reset(virtualView, platformView, handler);
    }

    protected override void VirtualView_PropertyChanging(object sender, PropertyChangingEventArgs e)
    {
        base.VirtualView_PropertyChanging(sender, e);

        if (e.PropertyName == VirtualViewItemsPropertyName)
        {
            ClearItems();
        }
    }

    protected override void VirtualView_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        base.VirtualView_PropertyChanged(sender, e);

        if (e.PropertyName == VirtualViewItemsPropertyName)
        {
            InitItems();
        }
    }

    protected virtual void Items_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                AddItems(e);
                break;
            case NotifyCollectionChangedAction.Remove:
                RemoveItems(e);
                break;
            case NotifyCollectionChangedAction.Replace:
                ReplaceItems(e);
                break;
            case NotifyCollectionChangedAction.Move:
                break;
            case NotifyCollectionChangedAction.Reset:
            default:
                ResetItems();
                break;
        }
    }

    protected virtual void AddItems(NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems?.Count is null or 0 || PlatformView is null) return;

        AddItemsToPlatformView(e.NewItems.Cast<TVItem>());
    }

    protected virtual void RemoveItems(NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems?.Count is null or 0 || PlatformView is null) return;

        RemoveItemsFromPlatformView(e.OldItems.Cast<TVItem>());
    }

    protected virtual void ReplaceItems(NotifyCollectionChangedEventArgs e)
    {
        RemoveItems(e);
        AddItems(e);
    }

    protected virtual void ResetItems()
    {
        ClearItems();

        InitItems();
    }

    protected virtual void ClearItems()
    {
        var itemCollection = VirtualViewItems;
        if (itemCollection is INotifyCollectionChanged items)
        {
            items.CollectionChanged -= Items_CollectionChanged;
        }

        RemoveItemsFromPlatformView([..Items]);
    }

    protected virtual void RemoveItemsFromPlatformView(IEnumerable<TVItem> items)
    {
        foreach (var item in items)
        {
            var native = RemoveItem(item);
            RemoveItemFromPlatformView(native);
        }
    }

    protected virtual TNItem? RemoveItem(TVItem item)
    {
        item.PropertyChanging -= Item_PropertyChanging;
        item.PropertyChanged -= Item_PropertyChanged;

        VirtualView!.RemoveLogicalChild(item);
        Items.Remove(item);

        return NativeObjectAttachedProperty.GetNativeObject(item) as TNItem;
    }

    protected abstract void RemoveItemFromPlatformView(TNItem? nItem);

    protected virtual void InitItems()
    {
        var itemCollection = VirtualViewItems;
        if (itemCollection is null) return;

        if (itemCollection.Any())
        {
            var itemsList = itemCollection.ToList();

            Items = new(itemsList.Count);

            AddItemsToPlatformView(itemsList);
        }

        if (itemCollection is INotifyCollectionChanged items)
        {
            items.CollectionChanged += Items_CollectionChanged;
        }
    }

    protected virtual void AddItemsToPlatformView(IEnumerable<TVItem> items)
    {
        foreach (var item in items)
        {
            var native = AddItemToPlatformView(item);
            AddItem(item, native);
        }
    }

    protected virtual void AddItem(TVItem vItem, TNItem nItem)
    {
        NativeObjectAttachedProperty.SetNativeObject(vItem, nItem);

        vItem.PropertyChanging += Item_PropertyChanging;
        vItem.PropertyChanged += Item_PropertyChanged;
        Items.Add(vItem);
        VirtualView!.AddLogicalChild(vItem);
    }

    protected abstract TNItem AddItemToPlatformView(TVItem vItem);

    protected virtual void Item_PropertyChanging(object sender, PropertyChangingEventArgs e)
    {
        var vItem = (sender as TVItem)!;

        if (NativeObjectAttachedProperty.GetNativeObject(vItem) is not TNItem nItem) return;

        ItemPropertyChanging(vItem, nItem, e.PropertyName);
    }

    protected virtual void ItemPropertyChanging(TVItem vItem, TNItem nItem, string? propertyName)
    {

    }

    protected virtual void Item_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        var vItem = (sender as TVItem)!;

        if (NativeObjectAttachedProperty.GetNativeObject(vItem) is not TNItem nItem) return;

        ItemPropertyChanged(vItem, nItem, e.PropertyName);
    }

    protected virtual void ItemPropertyChanged(TVItem vItem, TNItem nItem, string? propertyName)
    {

    }
}