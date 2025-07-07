namespace MPowerKit.GoogleMaps;

/// <summary>
/// Optimistically fetch clusters for adjacent zoom levels, caching them as necessary.
/// </summary>
public class PreCachingAlgorithmDecorator : AbstractAlgorithm, IAlgorithmDecorator
{
    public IAlgorithm Algorithm { get; }

    // Cache for storing clusters at different zoom levels.
    private readonly LurchTable<int, IEnumerable<Cluster>> _cache = new(5);

    /// <summary>
    /// Initializes a new instance of the <see cref="PreCachingAlgorithmDecorator"/> class.
    /// </summary>
    /// <param name="algorithm">The underlying clustering algorithm to be decorated.</param>
    public PreCachingAlgorithmDecorator(IAlgorithm algorithm)
    {
        ArgumentNullException.ThrowIfNull(algorithm, nameof(algorithm));

        Algorithm = algorithm;
    }

    /// <summary>
    /// Adds an item to the algorithm.
    /// </summary>
    /// <param name="item">The item to be added.</param>
    /// <returns>True if the algorithm contents changed as a result of the call.</returns>
    public override bool AddItem(Pin item)
    {
        var result = Algorithm.AddItem(item);
        if (result)
        {
            ClearCache();
        }
        return result;
    }

    /// <summary>
    /// Adds a collection of items to the algorithm.
    /// </summary>
    /// <param name="items">The items to be added.</param>
    /// <returns>True if the algorithm contents changed as a result of the call.</returns>
    public override bool AddItems(IEnumerable<Pin> items)
    {
        var result = Algorithm.AddItems(items);
        if (result)
        {
            ClearCache();
        }
        return result;
    }

    /// <summary>
    /// Clears all items from the algorithm.
    /// </summary>
    public override void ClearItems()
    {
        Algorithm.ClearItems();
        ClearCache();
    }

    /// <summary>
    /// Removes an item from the algorithm.
    /// </summary>
    /// <param name="item">The item to be removed.</param>
    /// <returns>True if this algorithm contained the specified element (or equivalently, if this
    /// algorithm changed as a result of the call).</returns>
    public override bool RemoveItem(Pin item)
    {
        var result = Algorithm.RemoveItem(item);
        if (result)
        {
            ClearCache();
        }
        return result;
    }

    /// <summary>
    /// Removes a collection of items from the algorithm.
    /// </summary>
    /// <param name="items">The items to be removed.</param>
    /// <returns>True if this algorithm contents changed as a result of the call.</returns>
    public override bool RemoveItems(IEnumerable<Pin> items)
    {
        var result = Algorithm.RemoveItems(items);
        if (result)
        {
            ClearCache();
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
        var result = Algorithm.UpdateItem(item);
        if (result)
        {
            ClearCache();
        }
        return result;
    }

    /// <summary>
    /// Gets the clusters for the specified zoom level.
    /// </summary>
    /// <param name="zoom">The zoom level.</param>
    /// <returns>A set of clusters.</returns>
    public override IEnumerable<Cluster> GetClusters(float zoom, CancellationToken token = default)
    {
        var discreteZoom = (int)zoom;
        var results = GetClustersInternal(discreteZoom, token);

        Precache();

        async void Precache()
        {
            try
            {
                // Precache adjacent zoom levels if not already cached.
                if (!_cache.TryGetValue(discreteZoom + 1, out var _))
                {
                    await Task.Run(() => PrecacheZoomLevel(discreteZoom + 1, token), token);
                }
                if (!_cache.TryGetValue(discreteZoom - 1, out var _))
                {
                    await Task.Run(() => PrecacheZoomLevel(discreteZoom - 1, token), token);
                }
            }
            catch { }
        }

        return results;
    }

    /// <summary>
    /// Gets the items in the algorithm.
    /// </summary>
    public override IEnumerable<Pin> Items => Algorithm.Items;

    /// <summary>
    /// Gets or sets the maximum distance between clustered items.
    /// </summary>
    public override int MaxDistanceBetweenClusteredItems
    {
        get => Algorithm.MaxDistanceBetweenClusteredItems;
        set
        {
            Algorithm.MaxDistanceBetweenClusteredItems = value;
            ClearCache();
        }
    }

    /// <summary>
    /// Clears the cache.
    /// </summary>
    protected virtual void ClearCache()
    {
        Lock();
        try
        {
            _cache.Clear();
        }
        finally
        {
            Unlock();
        }
    }

    /// <summary>
    /// Gets clusters for the specified zoom level from the cache or the underlying algorithm.
    /// </summary>
    /// <param name="discreteZoom">The discrete zoom level.</param>
    /// <returns>A set of clusters.</returns>
    protected virtual IEnumerable<Cluster> GetClustersInternal(int discreteZoom, CancellationToken token = default)
    {
        Lock();
        try
        {
            if (_cache.TryGetValue(discreteZoom, out var results))
            {
                return results;
            }

            results = Algorithm.GetClusters(discreteZoom, token);
            _cache.Add(discreteZoom, results);
            return results;
        }
        finally
        {
            Unlock();
        }
    }

    private readonly Random _precahceDelayRandom = new();
    /// <summary>
    /// Precaches clusters for the specified zoom level.
    /// </summary>
    /// <param name="zoom">The zoom level to precache.</param>
    protected virtual async Task PrecacheZoomLevel(int zoom, CancellationToken token = default)
    {
        try
        {
            // Simulate a delay between 500 - 1000 ms.
            await Task.Delay(_precahceDelayRandom.Next(500, 1000), token);

            GetClustersInternal(zoom, token);
        }
        catch (Exception)
        {
            // Ignore exceptions during precaching.
        }
    }
}