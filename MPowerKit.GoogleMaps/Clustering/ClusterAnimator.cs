namespace MPowerKit.GoogleMaps;

public class ClusterAnimator
{
    protected GoogleMap Map { get; }
    protected IClusterAnimation ClusterAnimation { get; }

    protected List<(Pin marker, Point animateTo)> MarkersToAnimate { get; set; } = [];

    public ClusterAnimator(GoogleMap map, IClusterAnimation animation)
    {
        Map = map;
        ClusterAnimation = animation;
    }

    public void AddMarker(Pin marker, Point animateTo)
    {
        MarkersToAnimate.Add((marker, animateTo));
    }

    public async Task Animate(bool zoomDirection, CancellationToken token = default)
    {
        var easing = zoomDirection ? ClusterAnimation.EasingOut : ClusterAnimation.EasingIn;
        var duration = zoomDirection ? ClusterAnimation.DurationOut : ClusterAnimation.DurationIn;

        List<Task> animations = [];
        foreach (var (marker, animateTo) in MarkersToAnimate)
        {
            TaskCompletionSource tcs = new();

            var fromPoint = marker.Position;
            var hash = $"{marker.GetHashCode()}";
            Map.Animate(hash, t =>
            {
                var lat = fromPoint.X + t * (animateTo.X - fromPoint.X);
                var lon = fromPoint.Y + t * (animateTo.Y - fromPoint.Y);
                return new Point(lat, lon);
            }, p =>
            {
                marker.Position = p;
            }, 16, (uint)duration.TotalMilliseconds, easing, (p, c) => tcs.SetResult(), () => false);

            token.Register(() =>
            {
                Map.AbortAnimation(hash);
                marker.Position = animateTo;
            });

            animations.Add(tcs.Task);
        }

        await Task.WhenAll(animations);

        MarkersToAnimate.Clear();
    }
}