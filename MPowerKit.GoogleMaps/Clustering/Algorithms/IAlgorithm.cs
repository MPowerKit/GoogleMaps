namespace MPowerKit.GoogleMaps;

/// <summary>
/// Defines the contract for clustering algorithms.
/// </summary>
public interface IAlgorithm : IDisposable
{
    /// <summary>
    /// Adds an item to the algorithm.
    /// </summary>
    /// <param name="item">The item to be added.</param>
    /// <returns>True if the algorithm contents changed as a result of the call.</returns>
    bool AddItem(Pin item);

    /// <summary>
    /// Adds a collection of items to the algorithm.
    /// </summary>
    /// <param name="items">The items to be added.</param>
    /// <returns>True if the algorithm contents changed as a result of the call.</returns>
    bool AddItems(IEnumerable<Pin> items);

    /// <summary>
    /// Clears all items from the algorithm.
    /// </summary>
    void ClearItems();

    /// <summary>
    /// Removes an item from the algorithm.
    /// </summary>
    /// <param name="item">The item to be removed.</param>
    /// <returns>True if the algorithm contained the specified element (or equivalently, if the algorithm changed as a result of the call).</returns>
    bool RemoveItem(Pin item);

    /// <summary>
    /// Updates the provided item in the algorithm.
    /// </summary>
    /// <param name="item">The item to be updated.</param>
    /// <returns>True if the item existed in the algorithm and was updated, or false if the item did not exist in the algorithm and the algorithm contents remain unchanged.</returns>
    bool UpdateItem(Pin item);

    /// <summary>
    /// Removes a collection of items from the algorithm.
    /// </summary>
    /// <param name="items">The items to be removed.</param>
    /// <returns>True if the algorithm contents changed as a result of the call.</returns>
    bool RemoveItems(IEnumerable<Pin> items);

    /// <summary>
    /// Computes and returns the clusters for the given zoom level.
    /// </summary>
    /// <param name="zoom">The zoom level for clustering.</param>
    /// <returns>A set of clusters.</returns>
    IEnumerable<Cluster> GetClusters(float zoom, CancellationToken token = default);

    /// <summary>
    /// All the items in the algorithm.
    /// </summary>
    IEnumerable<Pin> Items { get; }

    /// <summary>
    /// The maximum distance between clustered items.
    /// </summary>
    int MaxDistanceBetweenClusteredItems { get; set; }

    /// <summary>
    /// Locks the algorithm for exclusive write access.
    /// </summary>
    void Lock();

    /// <summary>
    /// Unlocks the algorithm after exclusive write access.
    /// </summary>
    void Unlock();
}