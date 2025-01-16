namespace MPowerKit.GoogleMaps;

/// <summary>
/// A simple clustering algorithm with O(n log n) performance. Resulting clusters are not hierarchical.
/// </summary>
/// <typeparam name="T">The type of <see cref="IClusterItem"/> to be clustered.</typeparam>
public class NonHierarchicalDistanceBasedAlgorithm : AbstractAlgorithm
{
    /// <summary>
    /// Any modifications should be synchronized on _quadTree.
    /// </summary>
    private readonly HashSet<QuadPinItem> _items = [];

    /// <summary>
    /// Any modifications should be synchronized on _quadTree.
    /// </summary>
    private readonly PointQuadTree<QuadPinItem> _quadTree = new(0d, 1d, 0d, 1d);

    protected static readonly SphericalMercatorProjection Projection = new(1d);

    /// <summary>
    /// Adds an item to the algorithm.
    /// </summary>
    /// <param name="item">The item to be added.</param>
    /// <returns>True if the algorithm contents changed as a result of the call.</returns>
    public override bool AddItem(Pin item)
    {
        Lock();
        try
        {
            QuadPinItem quadItem = new(item);
            var result = _items.Add(quadItem);
            if (result)
            {
                _quadTree.Add(quadItem);
            }
            return result;
        }
        finally
        {
            Unlock();
        }
    }

    /// <summary>
    /// Adds a collection of items to the algorithm.
    /// </summary>
    /// <param name="items">The items to be added.</param>
    /// <returns>True if the algorithm contents changed as a result of the call.</returns>
    public override bool AddItems(IEnumerable<Pin> items)
    {
        var result = false;
        foreach (var item in items)
        {
            var individualResult = AddItem(item);
            if (individualResult)
            {
                result = true;
            }
        }
        return result;
    }

    /// <summary>
    /// Clears all items from the algorithm.
    /// </summary>
    public override void ClearItems()
    {
        Lock();
        try
        {
            _items.Clear();
            _quadTree.Clear();
        }
        finally
        {
            Unlock();
        }
    }

    /// <summary>
    /// Removes an item from the algorithm.
    /// </summary>
    /// <param name="item">The item to be removed.</param>
    /// <returns>True if this algorithm contained the specified element (or equivalently, if this
    /// algorithm changed as a result of the call).</returns>
    public override bool RemoveItem(Pin item)
    {
        Lock();
        try
        {
            QuadPinItem quadItem = new(item);
            var result = _items.Remove(quadItem);
            if (result)
            {
                _quadTree.Remove(quadItem);
            }

            return result;
        }
        finally
        {
            Unlock();
        }
    }

    /// <summary>
    /// Removes a collection of items from the algorithm.
    /// </summary>
    /// <param name="items">The items to be removed.</param>
    /// <returns>True if this algorithm contents changed as a result of the call.</returns>
    public override bool RemoveItems(IEnumerable<Pin> items)
    {
        var result = false;
        foreach (var item in items)
        {
            var individualResult = RemoveItem(item);
            if (individualResult)
            {
                result = true;
            }
        }
        return result;
    }

    /// <summary>
    /// Updates the provided item in the algorithm.
    /// </summary>
    /// <param name="item">The item to be updated.</param>
    /// <returns>True if the item existed in the algorithm and was updated, or false if the item did
    /// not exist in the algorithm and the algorithm contents remain unchanged.</returns>
    public override bool UpdateItem(Pin item)
    {
        Lock();
        try
        {
            var result = RemoveItem(item);
            if (result)
            {
                // Only add the item if it was removed (to help prevent accidental duplicates on map)
                result = AddItem(item);
            }

            return result;
        }
        finally
        {
            Unlock();
        }
    }

    /// <summary>
    /// Gets the clusters for the specified zoom level.
    /// </summary>
    /// <param name="zoom">The zoom level.</param>
    /// <returns>A set of clusters.</returns>
    public override IEnumerable<Cluster> GetClusters(float zoom, CancellationToken token = default)
    {
        var discreteZoom = (int)zoom;
        var zoomSpecificSpan = MaxDistanceBetweenClusteredItems / Math.Pow(2d, discreteZoom) / 256d;

        HashSet<QuadPinItem> visitedCandidates = [];
        HashSet<Cluster> results = [];
        Dictionary<QuadPinItem, double> distanceToCluster = [];
        Dictionary<QuadPinItem, StaticCluster> itemToCluster = [];

        Lock();

        try
        {
            foreach (var candidate in GetClusteringItems(_quadTree, zoom))
            {
                if (token.IsCancellationRequested) return results;

                if (visitedCandidates.Contains(candidate))
                {
                    // Candidate is already part of another cluster.
                    continue;
                }

                var searchBounds = CreateBoundsFromSpan(candidate.Point, zoomSpecificSpan);
                var clusterItems = _quadTree.Search(searchBounds);
                if (clusterItems.Count == 1)
                {
                    // Only the current marker is in range. Just add the single item to the results.
                    results.Add(candidate);
                    visitedCandidates.Add(candidate);
                    distanceToCluster[candidate] = 0d;
                    continue;
                }

                StaticCluster cluster = new(candidate.ClusterItem.Position);
                results.Add(cluster);

                for (int i = 0; i < clusterItems.Count; i++)
                {
                    if (token.IsCancellationRequested) return results;

                    var clusterItem = clusterItems[i];

                    var distance = DistanceSquared(clusterItem.Point, candidate.Point);
                    if (distanceToCluster.TryGetValue(clusterItem, out var existingDistance))
                    {
                        // Item already belongs to another cluster. Check if it's closer to this cluster.
                        if (existingDistance < distance) continue;

                        // Move item to the closer cluster.
                        itemToCluster[clusterItem].Remove(clusterItem.ClusterItem);
                    }
                    distanceToCluster[clusterItem] = distance;
                    cluster.Add(clusterItem.ClusterItem);
                    itemToCluster[clusterItem] = cluster;
                }
                visitedCandidates.UnionWith(clusterItems);
            }

            return results;
        }
        finally
        {
            Unlock();
        }
    }

    /// <summary>
    /// Gets the clustering items for the specified zoom level.
    /// </summary>
    /// <param name="quadTree">The quad tree.</param>
    /// <param name="zoom">The zoom level.</param>
    /// <returns>A collection of clustering items.</returns>
    protected virtual IEnumerable<QuadPinItem> GetClusteringItems(PointQuadTree<QuadPinItem> quadTree, float zoom)
    {
        return _items;
    }

    /// <summary>
    /// Gets the items in the algorithm.
    /// </summary>
    public override IEnumerable<Pin> Items
    {
        get
        {
            Lock();
            try
            {
                return _items.Select(quadItem => quadItem.ClusterItem).ToList();
            }
            finally
            {
                Unlock();
            }
        }
    }

    /// <summary>
    /// Gets or sets the maximum distance between clustered items.
    /// </summary>
    public override int MaxDistanceBetweenClusteredItems { get; set; } = 100;

    private static double DistanceSquared(Point a, Point b)
    {
        return (a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y);
    }

    private static Bounds CreateBoundsFromSpan(Point p, double span)
    {
        var halfSpan = span / 2d;
        return new(
            p.X - halfSpan, p.X + halfSpan,
            p.Y - halfSpan, p.Y + halfSpan);
    }
}

/// <summary>
/// Represents an item in the quad tree.
/// </summary>
/// <typeparam name="TT">The type of <see cref="IClusterItem"/>.</typeparam>
public class QuadPinItem : Cluster, IPointItem
{
    public Pin ClusterItem { get; }
    public Point Point { get; }
    private readonly HashSet<Pin> _singletonSet;

    public QuadPinItem(Pin item)
    {
        ClusterItem = item;
        Position = item.Position;
        Point = new SphericalMercatorProjection(1d).ToPoint(Position);
        _singletonSet = [ClusterItem];
    }

    public override IEnumerable<Pin> Items => _singletonSet;

    public override int Size => 1;

    public override int GetHashCode()
    {
        return ClusterItem.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        if (obj is QuadPinItem other)
        {
            return ClusterItem.Equals(other.ClusterItem);
        }
        return false;
    }
}