namespace MPowerKit.GoogleMaps;

/// <summary>
/// Adapter for screen-based clustering algorithms. This class wraps an existing clustering algorithm
/// and provides additional functionality for handling map movements and camera changes.
/// </summary>
/// <typeparam name="T">The type of <see cref="IClusterItem"/> to be clustered.</typeparam>
public class ScreenBasedAlgorithmAdapter : AbstractAlgorithm, IAlgorithmDecorator, IScreenBasedAlgorithm
{
    public IAlgorithm Algorithm { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScreenBasedAlgorithmAdapter{T}"/> class.
    /// </summary>
    /// <param name="algorithm">The underlying clustering algorithm to be adapted.</param>
    public ScreenBasedAlgorithmAdapter(IAlgorithm algorithm)
    {
        Algorithm = algorithm;
    }

    /// <summary>
    /// Determines whether reclustering should occur on map movement.
    /// </summary>
    public bool ShouldReclusterOnMapMovement => false;

    /// <summary>
    /// Adds an item to the algorithm.
    /// </summary>
    /// <param name="item">The item to be added.</param>
    /// <returns>True if the algorithm contents changed as a result of the call.</returns>
    public override bool AddItem(Pin item)
    {
        return Algorithm.AddItem(item);
    }

    /// <summary>
    /// Adds a collection of items to the algorithm.
    /// </summary>
    /// <param name="items">The items to be added.</param>
    /// <returns>True if the algorithm contents changed as a result of the call.</returns>
    public override bool AddItems(IEnumerable<Pin> items)
    {
        return Algorithm.AddItems(items);
    }

    /// <summary>
    /// Clears all items from the algorithm.
    /// </summary>
    public override void ClearItems()
    {
        Algorithm.ClearItems();
    }

    /// <summary>
    /// Removes an item from the algorithm.
    /// </summary>
    /// <param name="item">The item to be removed.</param>
    /// <returns>True if this algorithm contained the specified element (or equivalently, if this
    /// algorithm changed as a result of the call).</returns>
    public override bool RemoveItem(Pin item)
    {
        return Algorithm.RemoveItem(item);
    }

    /// <summary>
    /// Removes a collection of items from the algorithm.
    /// </summary>
    /// <param name="items">The items to be removed.</param>
    /// <returns>True if this algorithm contents changed as a result of the call.</returns>
    public override bool RemoveItems(IEnumerable<Pin> items)
    {
        return Algorithm.RemoveItems(items);
    }

    /// <summary>
    /// Updates the provided item in the algorithm.
    /// </summary>
    /// <param name="item">The item to be updated.</param>
    /// <returns>True if the item existed in the algorithm and was updated, or false if the item did
    /// not exist in the algorithm and the algorithm contents remain unchanged.</returns>
    public override bool UpdateItem(Pin item)
    {
        return Algorithm.UpdateItem(item);
    }

    /// <summary>
    /// Gets the clusters for the specified zoom level.
    /// </summary>
    /// <param name="zoom">The zoom level.</param>
    /// <returns>A set of clusters.</returns>
    public override IEnumerable<Cluster> GetClusters(float zoom, CancellationToken token = default)
    {
        return Algorithm.GetClusters(zoom, token);
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
        set => Algorithm.MaxDistanceBetweenClusteredItems = value;
    }

    /// <summary>
    /// Handles camera change events.
    /// </summary>
    /// <param name="cameraPosition">The new camera position.</param>
    public virtual void OnCameraChange(CameraPosition cameraPosition)
    {
        if (Algorithm is IScreenBasedAlgorithm screenBased)
            screenBased.OnCameraChange(cameraPosition);
    }
}