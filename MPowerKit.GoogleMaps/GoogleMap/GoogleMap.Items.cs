using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

using Microsoft.Maui.Controls.Shapes;

namespace MPowerKit.GoogleMaps;

public partial class GoogleMap
{
    protected List<Element> AllChildren { get; } = [];

    public virtual IEnumerable<View> MapObjects => new ReadOnlyCollection<View>(AllChildren.OfType<View>().ToList());

    protected virtual void InitItems()
    {
        this.PropertyChanging += GoogleMap_Items_PropertyChanging;
        this.PropertyChanged += GoogleMap_Items_PropertyChanged;
    }

    protected virtual void GoogleMap_Items_PropertyChanging(object sender, PropertyChangingEventArgs e)
    {
        if (e.PropertyName == PolylinesSourceProperty.PropertyName
            || e.PropertyName == PolylineItemTemplateProperty.PropertyName)
        {
            ClearSource<Polyline>(PolylinesSource, PolylinesProperty);
        }
        else if (e.PropertyName == PolygonsSourceProperty.PropertyName
            || e.PropertyName == PolygonItemTemplateProperty.PropertyName)
        {
            ClearSource<Polygon>(PolygonsSource, PolygonsProperty);
        }
        else if (e.PropertyName == CirclesSourceProperty.PropertyName
            || e.PropertyName == CircleItemTemplateProperty.PropertyName)
        {
            ClearSource<Circle>(CirclesSource, CirclesProperty);
        }
        else if (e.PropertyName == TileOverlaysSourceProperty.PropertyName
            || e.PropertyName == TileOverlayItemTemplateProperty.PropertyName)
        {
            ClearSource<TileOverlay>(TileOverlaysSource, TileOverlaysProperty);
        }
        else if (e.PropertyName == GroundOverlaysSourceProperty.PropertyName
            || e.PropertyName == GroundOverlayItemTemplateProperty.PropertyName)
        {
            ClearSource<GroundOverlay>(GroundOverlaysSource, GroundOverlaysProperty);
        }
        else if (e.PropertyName == PinsSourceProperty.PropertyName
            || e.PropertyName == PinItemTemplateProperty.PropertyName)
        {
            ClearSource<Pin>(PinsSource, PinsProperty);
        }
    }

    protected virtual void GoogleMap_Items_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == PolylinesSourceProperty.PropertyName
            || e.PropertyName == PolylineItemTemplateProperty.PropertyName)
        {
            InitSource<Polyline>(PolylinesSource, PolylineItemTemplate, PolylinesProperty);
        }
        else if (e.PropertyName == PolygonsSourceProperty.PropertyName
            || e.PropertyName == PolygonItemTemplateProperty.PropertyName)
        {
            InitSource<Polygon>(PolygonsSource, PolygonItemTemplate, PolygonsProperty);
        }
        else if (e.PropertyName == CirclesSourceProperty.PropertyName
            || e.PropertyName == CircleItemTemplateProperty.PropertyName)
        {
            InitSource<Circle>(CirclesSource, CircleItemTemplate, CirclesProperty);
        }
        else if (e.PropertyName == TileOverlaysSourceProperty.PropertyName
            || e.PropertyName == TileOverlayItemTemplateProperty.PropertyName)
        {
            InitSource<TileOverlay>(TileOverlaysSource, TileOverlayItemTemplate, TileOverlaysProperty);
        }
        else if (e.PropertyName == GroundOverlaysSourceProperty.PropertyName
            || e.PropertyName == GroundOverlayItemTemplateProperty.PropertyName)
        {
            InitSource<GroundOverlay>(GroundOverlaysSource, GroundOverlayItemTemplate, GroundOverlaysProperty);
        }
        else if (e.PropertyName == PinsSourceProperty.PropertyName
            || e.PropertyName == PinItemTemplateProperty.PropertyName)
        {
            InitSource<Pin>(PinsSource, PinItemTemplate, PinsProperty);
        }
    }

    protected override void OnChildAdded(Element child)
    {
        base.OnChildAdded(child);

        AllChildren.Add(child);
    }

    protected override void OnChildRemoved(Element child, int oldLogicalIndex)
    {
        AllChildren.Remove(child);

        base.OnChildRemoved(child, oldLogicalIndex);
    }

    protected virtual void ClearSource<T>(IEnumerable source, BindableProperty property)
        where T : VisualElement
    {
        if (source is INotifyCollectionChanged collectionChanged)
        {
            collectionChanged.CollectionChanged -= Source_CollectionChanged<T>;
        }

        if (property is null) return;

        var mapObjects = GetValue(property) as ObservableCollection<T>;

        mapObjects?.Clear();
        SetValue(property, null);
    }

    protected virtual void InitSource<T>(IEnumerable source, DataTemplate itemTemplate, BindableProperty property)
        where T : VisualElement
    {
        if (source is null || itemTemplate is null || property is null) return;

        if (source is INotifyCollectionChanged collectionChanged)
        {
            collectionChanged.CollectionChanged += Source_CollectionChanged<T>;
        }

        ObservableCollection<T> mapObjects = [];

        SetValue(property, mapObjects);

        AddMapObjects(source, mapObjects, 0, itemTemplate);
    }

    protected Dictionary<Type, (BindableProperty items, BindableProperty itemsSource, BindableProperty template)> MapObjectsAndTemplateProperties { get; } = new()
    {
        { typeof(Polyline), (PolylinesProperty, PolylinesSourceProperty, PolylineItemTemplateProperty) },
        { typeof(Polygon), (PolygonsProperty, PolygonsSourceProperty, PolygonItemTemplateProperty) },
        { typeof(Circle), (CirclesProperty, CirclesSourceProperty, CircleItemTemplateProperty) },
        { typeof(TileOverlay), (TileOverlaysProperty, TileOverlaysSourceProperty, TileOverlayItemTemplateProperty) },
        { typeof(GroundOverlay), (GroundOverlaysProperty, GroundOverlaysSourceProperty, GroundOverlayItemTemplateProperty) },
        { typeof(Pin), (PinsProperty, PinsSourceProperty, PinItemTemplateProperty) },
    };

    protected virtual (ObservableCollection<T>?, IEnumerable?, DataTemplate?) GetMapObjectsAndTemplate<T>()
        where T : VisualElement
    {
        var key = typeof(T);
        if (MapObjectsAndTemplateProperties.TryGetValue(key, out var properties))
        {
            var items = GetValue(properties.items) as ObservableCollection<T>;
            var itemsSource = GetValue(properties.items) as IEnumerable;
            var template = GetValue(properties.template) as DataTemplate;
            return (items, itemsSource, template);
        }

        return (null, null, null);
    }

    protected virtual void Source_CollectionChanged<T>(object? sender, NotifyCollectionChangedEventArgs e)
        where T : VisualElement
    {
        var (mapObjects, itemsSource, itemTemplate) = GetMapObjectsAndTemplate<T>();

        if (itemTemplate is null || mapObjects is null) return;

        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                AddMapObjects(e.NewItems!, mapObjects!, e.NewStartingIndex, itemTemplate!);
                break;
            case NotifyCollectionChangedAction.Remove:
                RemoveMapObjects(e.OldItems!, mapObjects!, e.OldStartingIndex);
                break;
            case NotifyCollectionChangedAction.Replace:
                RemoveMapObjects(e.OldItems!, mapObjects!, e.OldStartingIndex);
                AddMapObjects(e.NewItems!, mapObjects!, e.NewStartingIndex, itemTemplate!);
                break;
            case NotifyCollectionChangedAction.Move:
                break;
            case NotifyCollectionChangedAction.Reset:
                ResetMapObjects(itemsSource, mapObjects, itemTemplate);
                break;
        }
    }

    protected virtual void AddMapObjects<T>(IEnumerable source, IList<T> dest, int fromIndex, DataTemplate itemTemplate)
        where T : VisualElement
    {
        var index = fromIndex;

        foreach (var item in source)
        {
            var template = itemTemplate;
            while (template is DataTemplateSelector selector)
            {
                template = selector.SelectTemplate(item, this);
            }

            var @object = template?.CreateContent();

            var typeName = typeof(T).Name;

            if (@object is not T mo)
                throw new InvalidOperationException($"{typeName}ItemTemplate must return a {typeName}");
            mo.BindingContext = item;

            dest.Insert(index++, mo);
        }
    }

    protected virtual void RemoveMapObjects<T>(IEnumerable source, IList<T> dest, int fromIndex)
        where T : VisualElement
    {
        var index = fromIndex;

        foreach (var item in source)
        {
            dest.RemoveAt(index);
        }
    }

    protected virtual void ResetMapObjects<T>(IEnumerable? source, IList<T>? dest, DataTemplate? itemTemplate)
        where T : VisualElement
    {
        if (source is null || dest is null || itemTemplate is null) return;

        dest.Clear();

        AddMapObjects(source, dest, 0, itemTemplate);
    }
}