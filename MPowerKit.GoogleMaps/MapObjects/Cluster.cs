namespace MPowerKit.GoogleMaps;

public class Cluster : Pin
{
    public Cluster()
    {
        AnchorX = 0.5;
        AnchorY = 0.5;
        CanBeSelected = false;
        IsEnabled = false;
    }

    public virtual IEnumerable<Pin> Items { get; }
    public virtual int Size { get; }
}