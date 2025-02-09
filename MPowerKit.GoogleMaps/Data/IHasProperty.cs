namespace MPowerKit.GoogleMaps.Data;

public interface IHasProperty : IVisible
{
    string? GetProperty(string property);
    bool HasProperty(string property);
}