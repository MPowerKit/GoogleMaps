using System.Collections.Specialized;

namespace MPowerKit.GoogleMaps;

public abstract class ItemsMapFeatureManager<TVItem, TNItem, TVMap, TNMap, THandler>
    : MapFeatureManager<TVMap, TNMap, THandler>
    where TVItem : VisualElement
    where TNItem : class
    where TVMap : GoogleMap
    where TNMap : class
    where THandler :
#if ANDROID || IOS
        GoogleMapHandler
#else
        class
#endif
{
    protected List<TVItem> Items { get; set; } = [];

    protected abstract IEnumerable<TVItem> GetVirtualViewItems();
    protected abstract string GetVirtualViewItemsPropertyName();

    protected override void Init(TVMap virtualView, TNMap platformView, THandler handler)
    {
        base.Init(virtualView, platformView, handler);

        ResetItems();
    }

    protected override void Reset(TVMap virtualView, TNMap platformView, THandler handler)
    {
        ClearItems();

        base.Reset(virtualView, platformView, handler);
    }

    protected override void VirtualView_PropertyChanging(object sender, PropertyChangingEventArgs e)
    {
        base.VirtualView_PropertyChanging(sender, e);

        if (e.PropertyName == GetVirtualViewItemsPropertyName())
        {
            ClearItems();
            return;
        }
    }

    protected override void VirtualView_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        base.VirtualView_PropertyChanged(sender, e);

        if (e.PropertyName == GetVirtualViewItemsPropertyName())
        {
            InitItems();
            return;
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
        var itemCollection = GetVirtualViewItems();
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
            item.PropertyChanging -= Item_PropertyChanging;
            item.PropertyChanged -= Item_PropertyChanged;
            var native = NativeObjectAttachedProperty.GetNativeObject(item) as TNItem;

            RemoveItemFromPlatformView(native);

            Items.Remove(item);
        }
    }

    protected abstract void RemoveItemFromPlatformView(TNItem? nItem);

    protected virtual void InitItems()
    {
        var itemCollection = GetVirtualViewItems();
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
            NativeObjectAttachedProperty.SetNativeObject(item, native);

            item.PropertyChanging += Item_PropertyChanging;
            item.PropertyChanged += Item_PropertyChanged;
            Items.Add(item);
        }
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