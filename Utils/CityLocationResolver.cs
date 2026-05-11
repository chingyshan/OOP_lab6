namespace Forecast.Utils;

public class CityLocationResolver
{
    private static readonly Dictionary<string, (decimal lat, decimal lon)> Cities = new()
    {
        ["minsk"] = (53.9045m, 27.5615m),
        ["london"] = (51.5074m, -0.1278m),
        ["tokyo"] = (35.6762m, 139.6503m),
        ["shanghai"] = (31.2304m, 121.4737m),
        ["warsaw"] = (52.2297m, 21.0122m)
    };

    public (decimal lat, decimal lon) Resolve(string city)
    {
        var key = city.ToLower().Trim();
        if (Cities.TryGetValue(key, out var coords))
            return coords;

        throw new ArgumentException($"Unknown city: {city}");
    }
}