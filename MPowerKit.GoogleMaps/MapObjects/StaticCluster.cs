namespace MPowerKit.GoogleMaps;

/// <summary>
/// Represents a cluster whose center is determined upon creation.
/// </summary>
public class StaticCluster : Cluster
{
    private readonly Point _center;
    private readonly HashSet<Pin> _items = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="StaticCluster"/> class.
    /// </summary>
    /// <param name="center">The center of the cluster.</param>
    public StaticCluster(Point center)
    {
        Position = center;
    }

    /// <summary>
    /// Adds an item to the cluster.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <returns>True if the item was added; otherwise, false.</returns>
    public bool Add(Pin item)
    {
        return _items.Add(item);
    }

    /// <summary>
    /// Removes an item from the cluster.
    /// </summary>
    /// <param name="item">The item to remove.</param>
    /// <returns>True if the item was removed; otherwise, false.</returns>
    public bool Remove(Pin item)
    {
        return _items.Remove(item);
    }

    /// <summary>
    /// Gets the collection of items in the cluster.
    /// </summary>
    public override IEnumerable<Pin> Items => _items;

    /// <summary>
    /// Gets the number of items in the cluster.
    /// </summary>
    public override int Size => _items.Count;

    /// <summary>
    /// Returns a string representation of the cluster.
    /// </summary>
    /// <returns>A string representation of the cluster.</returns>
    public override string ToString()
    {
        return $"StaticCluster{{Center={_center}, Items.Count={_items.Count}}}";
    }

    /// <summary>
    /// Returns the hash code for this cluster.
    /// </summary>
    /// <returns>The hash code for this cluster.</returns>
    public override int GetHashCode()
    {
        return _center.GetHashCode() + _items.GetHashCode();
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current cluster.
    /// </summary>
    /// <param name="obj">The object to compare with the current cluster.</param>
    /// <returns>True if the specified object is equal to the current cluster; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is not StaticCluster other)
        {
            return false;
        }

        return other._center.Equals(_center) && other._items.SetEquals(_items);
    }
}