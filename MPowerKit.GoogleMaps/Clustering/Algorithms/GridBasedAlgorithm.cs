namespace MPowerKit.GoogleMaps;

/// <summary>
/// Groups markers into a grid for clustering. This algorithm organizes items into a two-dimensional grid,
/// facilitating the formation of clusters based on proximity within each grid cell. The grid size determines
/// the spatial granularity of clustering, and clusters are created by aggregating items within the same grid cell.
/// <para>
/// The effectiveness of clustering is influenced by the specified grid size, which determines the spatial resolution of the grid.
/// Smaller grid sizes result in more localized clusters, whereas larger grid sizes lead to broader clusters covering larger areas.
/// </para>
/// </summary>
/// <typeparam name="T">The type of <see cref="IClusterItem"/> to be clustered.</typeparam>
public class GridBasedAlgorithm : AbstractAlgorithm
{
    private readonly HashSet<Pin> _items = [];

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
            return _items.Add(item);
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
        var changed = false;
        foreach (var item in items)
        {
            if (AddItem(item))
            {
                changed = true;
            }
        }
        return changed;
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
            return _items.Remove(item);
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
        var changed = false;
        foreach (var item in items)
        {
            if (RemoveItem(item))
            {
                changed = true;
            }
        }
        return changed;
    }

    /// <summary>
    /// Updates the provided item in the algorithm.
    /// </summary>
    /// <param name="item">The item to be updated.</param>
    /// <returns>True if the item existed in the algorithm and was updated, or false if the item did
    /// not exist in the algorithm and the algorithm contents remain unchanged.</returns>
    public override bool UpdateItem(Pin item)
    {
        var result = RemoveItem(item);
        if (result)
        {
            result = AddItem(item);
        }
        return result;
    }

    /// <summary>
    /// Gets or sets the maximum distance between clustered items.
    /// </summary>
    public override int MaxDistanceBetweenClusteredItems { get; set; } = 100;

    /// <summary>
    /// Gets the clusters for the specified zoom level.
    /// </summary>
    /// <param name="zoom">The zoom level.</param>
    /// <returns>A set of clusters.</returns>
    public override IEnumerable<Cluster> GetClusters(float zoom, CancellationToken token = default)
    {
        var numCells = Math.Ceiling(256d * Math.Pow(2d, zoom) / MaxDistanceBetweenClusteredItems);
        var proj = new SphericalMercatorProjection(numCells);

        HashSet<Cluster> clusters = [];
        Dictionary<long, StaticCluster> sparseArray = [];

        Lock();
        try
        {
            foreach (var item in _items)
            {
                if (token.IsCancellationRequested) return clusters;

                var p = proj.ToPoint(item.Position);

                var coord = GetCoord((long)numCells, p.X, p.Y);

                if (!sparseArray.TryGetValue(coord, out var cluster))
                {
                    cluster = new(proj.ToLatLng(new(Math.Floor(p.X) + 0.5, Math.Floor(p.Y) + 0.5)));
                    sparseArray[coord] = cluster;
                    clusters.Add(cluster);
                }
                cluster.Add(item);
            }

            return clusters;
        }
        finally
        {
            Unlock();
        }
    }

    /// <summary>
    /// Gets the items in the algorithm.
    /// </summary>
    public override IEnumerable<Pin> Items => _items.ToList();

    private static long GetCoord(long numCells, double x, double y)
    {
        return (long)(numCells * Math.Floor(x) + Math.Floor(y));
    }
}