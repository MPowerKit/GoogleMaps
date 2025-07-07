namespace MPowerKit.GoogleMaps;

public interface IPointItem
{
    Point Point { get; }
}

public class PointQuadTree<T> where T : IPointItem
{
    private const int MaxElements = 50;
    private const int MaxDepth = 40;

    private readonly Bounds _bounds;
    private readonly int _depth;
    private HashSet<T>? _items;
    private List<PointQuadTree<T>>? _children;

    public PointQuadTree(double minX, double maxX, double minY, double maxY)
        : this(new Bounds(minX, maxX, minY, maxY), 0) { }

    public PointQuadTree(Bounds bounds)
        : this(bounds, 0) { }

    private PointQuadTree(double minX, double maxX, double minY, double maxY, int depth)
        : this(new Bounds(minX, maxX, minY, maxY), depth) { }

    private PointQuadTree(Bounds bounds, int depth)
    {
        _bounds = bounds;
        _depth = depth;
    }

    public void Add(T item)
    {
        var point = item.Point;
        if (_bounds.Contains((float)point.X, (float)point.Y))
        {
            Insert(point.X, point.Y, item);
        }
    }

    private void Insert(double x, double y, T item)
    {
        if (_children is not null)
        {
            if (y < _bounds.MidY)
            {
                if (x < _bounds.MidX) // top left
                    _children[0].Insert(x, y, item);
                else // top right
                    _children[1].Insert(x, y, item);
            }
            else
            {
                if (x < _bounds.MidX) // bottom left
                    _children[2].Insert(x, y, item);
                else // bottom right
                    _children[3].Insert(x, y, item);
            }
            return;
        }

        _items ??= [];

        _items.Add(item);

        if (_items.Count > MaxElements && _depth < MaxDepth)
        {
            Split();
        }
    }

    private void Split()
    {
        _children =
        [
            new(_bounds.MinX, _bounds.MidX, _bounds.MinY, _bounds.MidY, _depth + 1),
            new(_bounds.MidX, _bounds.MaxX, _bounds.MinY, _bounds.MidY, _depth + 1),
            new(_bounds.MinX, _bounds.MidX, _bounds.MidY, _bounds.MaxY, _depth + 1),
            new(_bounds.MidX, _bounds.MaxX, _bounds.MidY, _bounds.MaxY, _depth + 1)
        ];

        var items = _items!;
        _items = null;

        foreach (var item in items)
        {
            Insert(item.Point.X, item.Point.Y, item);
        }
    }

    public bool Remove(T item)
    {
        var point = item.Point;
        if (_bounds.Contains((float)point.X, (float)point.Y))
        {
            return Remove(point.X, point.Y, item);
        }
        return false;
    }

    private bool Remove(double x, double y, T item)
    {
        if (_children is null) return _items?.Remove(item) ?? false;

        if (y < _bounds.MidY)
        {
            if (x < _bounds.MidX) // top left
                return _children[0].Remove(x, y, item);
            else // top right
                return _children[1].Remove(x, y, item);
        }

        if (x < _bounds.MidX) // bottom left
            return _children[2].Remove(x, y, item);
        else // bottom right
            return _children[3].Remove(x, y, item);
    }

    public void Clear()
    {
        _children = null;
        _items?.Clear();
    }

    public List<T> Search(Bounds searchBounds)
    {
        var results = new List<T>();
        Search(searchBounds, results);
        return [.. results];
    }

    private void Search(Bounds searchBounds, List<T> results)
    {
        if (!_bounds.Intersects(searchBounds))
        {
            return;
        }

        if (_children is not null)
        {
            foreach (var quad in _children)
            {
                quad.Search(searchBounds, results);
            }
        }
        else if (_items is not null)
        {
            if (searchBounds.Contains(_bounds))
            {
                results.AddRange(_items);
            }
            else
            {
                foreach (var item in _items)
                {
                    if (searchBounds.Contains(item.Point))
                    {
                        results.Add(item);
                    }
                }
            }
        }
    }
}

public readonly struct Bounds
{
    public double MinX { get; }
    public double MaxX { get; }
    public double MinY { get; }
    public double MaxY { get; }
    public readonly double MidX => (MinX + MaxX) / 2f;
    public readonly double MidY => (MinY + MaxY) / 2f;

    public Bounds(double minX, double maxX, double minY, double maxY)
    {
        MinX = minX;
        MaxX = maxX;
        MinY = minY;
        MaxY = maxY;
    }

    public bool Contains(Point point)
    {
        return Contains(point.X, point.Y);
    }

    public bool Contains(double x, double y)
    {
        return x >= MinX && x <= MaxX && y >= MinY && y <= MaxY;
    }

    public bool Intersects(Bounds other)
    {
        return !(MaxX < other.MinX || MinX > other.MaxX || MaxY < other.MinY || MinY > other.MaxY);
    }

    public bool Contains(Bounds other)
    {
        return Contains(other.MinX, other.MinY) && Contains(other.MaxX, other.MaxY);
    }
}