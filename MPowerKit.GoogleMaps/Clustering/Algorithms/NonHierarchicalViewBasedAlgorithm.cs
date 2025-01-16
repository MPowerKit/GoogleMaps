namespace MPowerKit.GoogleMaps;

/// <summary>
/// Algorithm that can be used for managing large numbers of items (>1000 markers). This algorithm works the same way as
/// <see cref="NonHierarchicalDistanceBasedAlgorithm{T}"/> but operates only in the visible area. It requires
/// <see cref="ShouldReclusterOnMapMovement"/> to be true in order to re-render clustering when camera movement changes the visible area.
/// </summary>
/// <typeparam name="T">The <see cref="IClusterItem"/> type.</typeparam>
public class NonHierarchicalViewBasedAlgorithm : NonHierarchicalDistanceBasedAlgorithm, IScreenBasedAlgorithm
{
    protected double MapWidth { get; set; }
    protected double MapHeight { get; set; }

    protected Point? MapCenter { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NonHierarchicalViewBasedAlgorithm{T}"/> class.
    /// </summary>
    /// <param name="screenWidth">Map width in dp.</param>
    /// <param name="screenHeight">Map height in dp.</param>
    public NonHierarchicalViewBasedAlgorithm(double screenWidth, double screenHeight)
    {
        MapWidth = screenWidth;
        MapHeight = screenHeight;
    }

    /// <summary>
    /// Handles camera change events.
    /// </summary>
    /// <param name="cameraPosition">The new camera position.</param>
    public void OnCameraChange(CameraPosition cameraPosition)
    {
        MapCenter = cameraPosition.Target;
    }

    /// <summary>
    /// Gets the clustering items for the specified zoom level.
    /// </summary>
    /// <param name="quadTree">The quad tree.</param>
    /// <param name="zoom">The zoom level.</param>
    /// <returns>A collection of clustering items.</returns>
    protected override IEnumerable<QuadPinItem> GetClusteringItems(PointQuadTree<QuadPinItem> quadTree, float zoom)
    {
        var visibleBounds = GetVisibleBounds(zoom);
        List<QuadPinItem> items = [];

        // Handle wrapping around international date line
        if (visibleBounds.MinX < 0d)
        {
            Bounds wrappedBounds = new(visibleBounds.MinX + 1d, 1d, visibleBounds.MinY, visibleBounds.MaxY);
            items.AddRange(quadTree.Search(wrappedBounds));
            visibleBounds = new(0d, visibleBounds.MaxX, visibleBounds.MinY, visibleBounds.MaxY);
        }
        if (visibleBounds.MaxX > 1d)
        {
            Bounds wrappedBounds = new(0d, visibleBounds.MaxX - 1d, visibleBounds.MinY, visibleBounds.MaxY);
            items.AddRange(quadTree.Search(wrappedBounds));
            visibleBounds = new(visibleBounds.MinX, 1d, visibleBounds.MinY, visibleBounds.MaxY);
        }
        items.AddRange(quadTree.Search(visibleBounds));

        return items;
    }

    /// <summary>
    /// Determines whether reclustering should occur on map movement.
    /// </summary>
    public bool ShouldReclusterOnMapMovement => true;

    /// <summary>
    /// Updates the view width and height in case the map size was changed.
    /// You need to recluster all the clusters to update the view state after view size changes.
    /// </summary>
    /// <param name="width">Map width in dp.</param>
    /// <param name="height">Map height in dp.</param>
    public void UpdateViewSize(double width, double height)
    {
        MapWidth = width;
        MapHeight = height;
    }

    protected virtual Bounds GetVisibleBounds(float zoom)
    {
        if (MapCenter is null) return new(0d, 0d, 0d, 0d);

        var p = Projection.ToPoint(MapCenter.Value);

        var halfWidthSpan = MapWidth / Math.Pow(2d, zoom) / 256d / 2d;
        var halfHeightSpan = MapHeight / Math.Pow(2d, zoom) / 256d / 2d;

        return new(
            p.X - halfWidthSpan, p.X + halfWidthSpan,
            p.Y - halfHeightSpan, p.Y + halfHeightSpan);
    }
}