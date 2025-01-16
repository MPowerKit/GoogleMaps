namespace MPowerKit.GoogleMaps;

// <summary>
/// Base algorithm class that implements lock/unlock functionality.
/// </summary>
/// <typeparam name="T">The type of <see cref="IClusterItem"/> objects in the algorithm.</typeparam>
public abstract class AbstractAlgorithm : IAlgorithm
{
    private readonly SemaphoreSlim _lock = new(1,1);

    public abstract IEnumerable<Pin> Items { get; }

    public virtual int MaxDistanceBetweenClusteredItems { get; set; }

    /// <summary>
    /// Locks the algorithm for exclusive write access.
    /// </summary>
    public void Lock()
    {
        _lock.Wait();
    }

    /// <summary>
    /// Unlocks the algorithm after exclusive write access.
    /// </summary>
    public void Unlock()
    {
        _lock.Release();
    }

    /// <summary>
    /// Disposes the lock when the object is no longer needed.
    /// </summary>
    public void Dispose()
    {
        _lock.Dispose();
    }

    public abstract bool AddItem(Pin item);

    public abstract bool AddItems(IEnumerable<Pin> items);

    public abstract void ClearItems();

    public abstract bool RemoveItem(Pin item);

    public abstract bool UpdateItem(Pin item);

    public abstract bool RemoveItems(IEnumerable<Pin> items);

    public abstract IEnumerable<Cluster> GetClusters(float zoom, CancellationToken token = default);
}