using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;

namespace MPowerKit.GoogleMaps;

public partial class GoogleMap
{
    public static DataTemplate DefaultClusterIconTemplate { get; } = new DefaultClusterIconTemplateSelector();

    public event Action<Cluster>? ClusterClick;
    public event Action<Cluster>? ClusterInfoWindowClick;
    public event Action<Cluster>? ClusterInfoWindowLongClick;
    public event Action<Cluster>? ClusterInfoWindowClosed;

    protected CameraPosition? PreviousCameraPosition { get; set; }
    protected virtual Dictionary<int, ImageSource?> ClustersIconCache { get; set; } = [];
    protected CancellationTokenSource? ReclusterCts;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public ClusterAlgorithm PrevAlgorithm { get; protected set; }
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected IScreenBasedAlgorithm? ScreenBasedAlgorithm { get; set; }

    protected virtual void InitClusters()
    {
        this.PropertyChanging += GoogleMap_Clusters_PropertyChanging;
        this.PropertyChanged += GoogleMap_Clusters_PropertyChanged;
        this.SizeChanged += GoogleMap_Clusters_SizeChanged;
        this.CameraIdle += GoogleMap_Clusters_CameraIdle;

        OnClusterAlgorithmChanged();
    }

    protected virtual void GoogleMap_Clusters_CameraIdle(VisibleRegion obj)
    {
        ReclusterIfNeeded();
    }

    protected virtual void GoogleMap_Clusters_SizeChanged(object? sender, EventArgs e)
    {
        IAlgorithm? algo = ScreenBasedAlgorithm;
        while (algo is IAlgorithmDecorator decorator)
        {
            algo = decorator.Algorithm;
        }

        if (algo is NonHierarchicalViewBasedAlgorithm viewBasedAlgo)
        {
            viewBasedAlgo.UpdateViewSize(this.Width, this.Height);
            ReclusterIfNeeded();
        }
    }

    private void GoogleMap_Clusters_PropertyChanging(object sender, Microsoft.Maui.Controls.PropertyChangingEventArgs e)
    {
        if (e.PropertyName == ClusterAlgorithmProperty.PropertyName)
        {
            OnClusterAlgorithmChanging();
        }
        else if (e.PropertyName == PinsProperty.PropertyName)
        {
            OnClustersPinsChanging();
        }
    }

    private void GoogleMap_Clusters_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == ClusterAlgorithmProperty.PropertyName)
        {
            OnClusterAlgorithmChanged();
        }
        else if (e.PropertyName == PinsProperty.PropertyName)
        {
            OnClustersPinsChanged();
        }
    }

    protected virtual void OnClusterAlgorithmChanging()
    {
        PrevAlgorithm = ClusterAlgorithm;

        if (ScreenBasedAlgorithm is not null)
        {
            ScreenBasedAlgorithm.ClearItems();
            ScreenBasedAlgorithm.Dispose();
        }
    }

    protected virtual void OnClusterAlgorithmChanged()
    {
        ScreenBasedAlgorithm = ClusterAlgorithm switch
        {
            ClusterAlgorithm.Grid => new ScreenBasedAlgorithmAdapter(new GridBasedAlgorithm()),
            ClusterAlgorithm.GridPreCaching => new ScreenBasedAlgorithmAdapter(new PreCachingAlgorithmDecorator(new GridBasedAlgorithm())),
            ClusterAlgorithm.NonHierarchicalDistance => new ScreenBasedAlgorithmAdapter(new NonHierarchicalDistanceBasedAlgorithm()),
            ClusterAlgorithm.NonHierarchicalDistancePreCaching => new ScreenBasedAlgorithmAdapter(new PreCachingAlgorithmDecorator(new NonHierarchicalDistanceBasedAlgorithm())),
            ClusterAlgorithm.NonHierarchicalView => new NonHierarchicalViewBasedAlgorithm(Width, Height),
            _ => null
        };

        Recluster();
    }

    protected virtual void OnClustersPinsChanging()
    {
        if (Pins is INotifyCollectionChanged collectionChanged)
        {
            collectionChanged.CollectionChanged -= Clusters_Pins_CollectionChanged;
        }
    }

    protected virtual void OnClustersPinsChanged()
    {
        if (Pins is INotifyCollectionChanged collectionChanged)
        {
            collectionChanged.CollectionChanged += Clusters_Pins_CollectionChanged;
        }

        Recluster();
    }

    protected virtual void Clusters_Pins_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Recluster();
    }

    protected virtual void ReclusterIfNeeded()
    {
        if (ScreenBasedAlgorithm is null || CameraPosition is null) return;

        var newPosition = CameraPosition;

        // delegate clustering to the algorithm
        if (ScreenBasedAlgorithm.ShouldReclusterOnMapMovement)
        {
            Recluster();
        }
        // Don't re-compute clusters if the map has just been panned/tilted/rotated.
        else if (PreviousCameraPosition?.Zoom != newPosition.Zoom)
        {
            PreviousCameraPosition = newPosition;
            Recluster();
        }
    }

    public virtual async void Recluster()
    {
        if (PrevAlgorithm is not ClusterAlgorithm.None && ClusterAlgorithm is ClusterAlgorithm.None)
        {
            OnPropertyChanged(PinsProperty.PropertyName);
            return;
        }

        if (ClusterAlgorithm is ClusterAlgorithm.None || CameraPosition is null) return;

        try
        {
            ReclusterCts?.Cancel();
            ReclusterCts?.Dispose();
        }
        catch { }

        ReclusterCts = new();

        var token = ReclusterCts.Token;

        var clusters = await GetClusters(token);
        if (clusters is null)
        {
            Clusters = null;
            return;
        }
        if (token.IsCancellationRequested) return;

        InflateClustersMarkers(clusters, token);

        if (token.IsCancellationRequested) return;

        await AnimateClusters(clusters, token);

        if (token.IsCancellationRequested) return;

        var pin = clusters.Where(c => c.Size < MinClusterSize)
                          .SelectMany(c => c.Items)
                          .FirstOrDefault(p => p == SelectedPin);
        if (pin?.InfoWindowShown is true)
        {
            pin.ShowInfoWindow();
        }
    }

    protected virtual async Task AnimateClusters(IEnumerable<Cluster> newClusters, CancellationToken token = default)
    {
        var newPosition = CameraPosition;
        if (Clusters is null
            || ClusterAnimation is null
            || PreviousCameraPosition is null
            || newPosition.Zoom == PreviousCameraPosition.Zoom)
        {
            PreviousCameraPosition = newPosition;
            Clusters = newClusters;
            return;
        }

        var direction = newPosition.Zoom > PreviousCameraPosition.Zoom;
        var visibleBounds = VisibleRegion.Bounds;
        SphericalMercatorProjection projection = new(256d * Math.Pow(2d, Math.Min(newPosition.Zoom, PreviousCameraPosition.Zoom)));

        PreviousCameraPosition = newPosition;

        ClusterAnimator animator = new(this, ClusterAnimation);

        var oldClusters = Clusters.ToList();

        List<Point> animationAnchorPoints = [];
        foreach (var cluster in direction ? oldClusters : newClusters)
        {
            if (token.IsCancellationRequested) return;
            if (cluster.Size < MinClusterSize || !visibleBounds.Contains(cluster.Position)) continue;

            var point = projection.ToPoint(cluster.Position);
            animationAnchorPoints.Add(point);
        }

        foreach (var cluster in direction ? newClusters : oldClusters)
        {
            if (token.IsCancellationRequested) return;

            if (!visibleBounds.Contains(cluster.Position)) continue;

            var point = projection.ToPoint(cluster.Position);
            var closest = FindClosestPoint(animationAnchorPoints, point);
            if (closest is null) continue;

            Point animateTo;
            if (direction)
            {
                var animateFrom = projection.ToLatLng(closest.Value);
                animateTo = cluster.Position;
                cluster.Position = animateFrom;
            }
            else
            {
                animateTo = projection.ToLatLng(closest.Value);
            }
            animator.AddMarker(cluster, animateTo);
        }

        if (token.IsCancellationRequested) return;

        if (direction) Clusters = newClusters;

        if (token.IsCancellationRequested) return;

        await animator.Animate(direction, token);

        if (token.IsCancellationRequested) return;

        if (!direction) Clusters = newClusters;
    }

    protected virtual Point? FindClosestPoint(List<Point> markers, Point point)
    {
        if (markers?.Count is null or 0) return null;

        var maxDistance = ScreenBasedAlgorithm!.MaxDistanceBetweenClusteredItems;
        double minDistSquared = maxDistance * maxDistance;

        Point? closest = null;
        foreach (var candidate in markers)
        {
            var dist = DistanceSquared(candidate, point);
            if (dist < minDistSquared)
            {
                closest = candidate;
                minDistSquared = dist;
            }
        }
        return closest;
    }

    protected static double DistanceSquared(Point a, Point b)
    {
        var dx = a.X - b.X;
        var dy = a.Y - b.Y;
        return dx * dx + dy * dy;
    }

    protected virtual async Task<IEnumerable<Cluster>?> GetClusters(CancellationToken token = default)
    {
        var algo = ScreenBasedAlgorithm!;
        algo.OnCameraChange(CameraPosition);

        if (Pins?.Count() is null or 0)
        {
            algo.ClearItems();
            return null;
        }

        try
        {
            return await Task.Run(() =>
            {
                algo.ClearItems();
                algo.AddItems(Pins);
                return algo.GetClusters(CameraPosition.Zoom, token);
            }, token);
        }
        catch (Exception ex)
        {
            return Enumerable.Empty<Cluster>();
        }
    }

    protected virtual void InflateClustersMarkers(IEnumerable<Cluster> clusters, CancellationToken token = default)
    {
        var shouldCache = ClusterIconTemplate is null;
        var iconTemplate = ClusterIconTemplate ?? DefaultClusterIconTemplate;

        var minClusterSize = MinClusterSize;
        foreach (var cluster in clusters.Where(c => c.Size >= minClusterSize))
        {
            var template = iconTemplate;
            if (token.IsCancellationRequested) return;

            cluster.Title = $"Cluster size {cluster.Size}";
            cluster.Snippet = $"Cluster center Lat={cluster.Position.X}, Lon={cluster.Position.Y}";

            ImageSource? source = null;
            if (shouldCache)
            {
                ClustersIconCache.TryGetValue(cluster.Size, out source);
            }

            if (source is null)
            {
                while (template is DataTemplateSelector selector)
                {
                    if (token.IsCancellationRequested) return;

                    template = selector.SelectTemplate(cluster, this);
                }

                source = template?.CreateContent() as ImageSource;

                if (shouldCache) ClustersIconCache[cluster.Size] = source;
            }

            cluster.Icon = source;
        }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void SendClusterClick(Cluster cluster)
    {
        SelectedPin = null;

        cluster.ShowInfoWindow();

        ClusterClick?.Invoke(cluster);

        var parameter = cluster.BindingContext ?? cluster;

        if (ClusterClickedCommand?.CanExecute(parameter) is true)
            ClusterClickedCommand.Execute(parameter);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void SendClusterInfoWindowClick(Cluster cluster)
    {
        ClusterInfoWindowClick?.Invoke(cluster);

        var parameter = cluster.BindingContext ?? cluster;

        if (ClusterInfoWindowClickedCommand?.CanExecute(parameter) is true)
            ClusterInfoWindowClickedCommand.Execute(parameter);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void SendClusterInfoWindowLongClick(Cluster cluster)
    {
        ClusterInfoWindowLongClick?.Invoke(cluster);

        var parameter = cluster.BindingContext ?? cluster;

        if (ClusterInfoWindowLongClickedCommand?.CanExecute(parameter) is true)
            ClusterInfoWindowLongClickedCommand.Execute(parameter);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void SendClusterInfoWindowClosed(Cluster cluster)
    {
        ClusterInfoWindowClosed?.Invoke(cluster);

        var parameter = cluster.BindingContext ?? cluster;

        if (ClusterInfoWindowClosedCommand?.CanExecute(parameter) is true)
            ClusterInfoWindowClosedCommand.Execute(parameter);
    }

    #region MinClusterSize
    public int MinClusterSize
    {
        get => (int)GetValue(MinClusterSizeProperty);
        set => SetValue(MinClusterSizeProperty, value);
    }

    public static readonly BindableProperty MinClusterSizeProperty =
        BindableProperty.Create(
            nameof(MinClusterSize),
            typeof(int),
            typeof(GoogleMap),
            4
            );
    #endregion

    #region UseBucketsForClusters
    public bool UseBucketsForClusters
    {
        get => (bool)GetValue(UseBucketsForClustersProperty);
        set => SetValue(UseBucketsForClustersProperty, value);
    }

    public static readonly BindableProperty UseBucketsForClustersProperty =
        BindableProperty.Create(
            nameof(UseBucketsForClusters),
            typeof(bool),
            typeof(GoogleMap),
            true
            );
    #endregion

    #region ClusterAnimation
    public IClusterAnimation ClusterAnimation
    {
        get => (IClusterAnimation)GetValue(ClusterAnimationProperty);
        set => SetValue(ClusterAnimationProperty, value);
    }

    public static readonly BindableProperty ClusterAnimationProperty =
        BindableProperty.Create(
            nameof(ClusterAnimation),
            typeof(IClusterAnimation),
            typeof(GoogleMap),
            new ClusterAnimation()
            );
    #endregion

    #region ClusterAlgorithm
    public ClusterAlgorithm ClusterAlgorithm
    {
        get => (ClusterAlgorithm)GetValue(ClusterAlgorithmProperty);
        set => SetValue(ClusterAlgorithmProperty, value);
    }

    public static readonly BindableProperty ClusterAlgorithmProperty =
        BindableProperty.Create(
            nameof(ClusterAlgorithm),
            typeof(ClusterAlgorithm),
            typeof(GoogleMap),
            ClusterAlgorithm.NonHierarchicalView
            );
    #endregion

    #region ClusterIconTemplate
    public DataTemplate ClusterIconTemplate
    {
        get => (DataTemplate)GetValue(ClusterIconTemplateProperty);
        set => SetValue(ClusterIconTemplateProperty, value);
    }

    public static readonly BindableProperty ClusterIconTemplateProperty =
        BindableProperty.Create(
            nameof(ClusterIconTemplate),
            typeof(DataTemplate),
            typeof(GoogleMap)
            );
    #endregion

    #region ClusterInfoWindowTemplate
    public DataTemplate ClusterInfoWindowTemplate
    {
        get => (DataTemplate)GetValue(ClusterInfoWindowTemplateProperty);
        set => SetValue(ClusterInfoWindowTemplateProperty, value);
    }

    public static readonly BindableProperty ClusterInfoWindowTemplateProperty =
        BindableProperty.Create(
            nameof(ClusterInfoWindowTemplate),
            typeof(DataTemplate),
            typeof(GoogleMap)
            );
    #endregion

    #region Clusters
    public IEnumerable<Cluster> Clusters
    {
        get => (IEnumerable<Cluster>)GetValue(ClustersProperty);
        protected set => SetValue(ClustersProperty, value);
    }

    public static readonly BindableProperty ClustersProperty =
        BindableProperty.Create(
            nameof(Clusters),
            typeof(IEnumerable<Cluster>),
            typeof(GoogleMap)
            );
    #endregion

    #region ClusterClickedCommand
    public ICommand ClusterClickedCommand
    {
        get => (ICommand)GetValue(ClusterClickedCommandProperty);
        set => SetValue(ClusterClickedCommandProperty, value);
    }

    public static readonly BindableProperty ClusterClickedCommandProperty =
        BindableProperty.Create(
            nameof(ClusterClickedCommand),
            typeof(ICommand),
            typeof(GoogleMap));
    #endregion

    #region ClusterInfoWindowClickedCommand
    public ICommand ClusterInfoWindowClickedCommand
    {
        get => (ICommand)GetValue(ClusterInfoWindowClickedCommandProperty);
        set => SetValue(ClusterInfoWindowClickedCommandProperty, value);
    }

    public static readonly BindableProperty ClusterInfoWindowClickedCommandProperty =
        BindableProperty.Create(
            nameof(ClusterInfoWindowClickedCommand),
            typeof(ICommand),
            typeof(GoogleMap));
    #endregion

    #region ClusterInfoWindowLongClickedCommand
    public ICommand ClusterInfoWindowLongClickedCommand
    {
        get => (ICommand)GetValue(ClusterInfoWindowLongClickedCommandProperty);
        set => SetValue(ClusterInfoWindowLongClickedCommandProperty, value);
    }

    public static readonly BindableProperty ClusterInfoWindowLongClickedCommandProperty =
        BindableProperty.Create(
            nameof(ClusterInfoWindowLongClickedCommand),
            typeof(ICommand),
            typeof(GoogleMap));
    #endregion

    #region ClusterInfoWindowClosedCommand
    public ICommand ClusterInfoWindowClosedCommand
    {
        get => (ICommand)GetValue(ClusterInfoWindowClosedCommandProperty);
        set => SetValue(ClusterInfoWindowClosedCommandProperty, value);
    }

    public static readonly BindableProperty ClusterInfoWindowClosedCommandProperty =
        BindableProperty.Create(
            nameof(ClusterInfoWindowClosedCommand),
            typeof(ICommand),
            typeof(GoogleMap));
    #endregion
}