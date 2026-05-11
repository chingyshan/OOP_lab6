using Forecast.Clients;
using Forecast.Models.Weather;

namespace Forecast.Controllers;

public class CurrentWeatherController(IWeatherDataClient client)
{
    public async Task<CurrentWeather> GetCurrentWeather(decimal latitude, decimal longitude)
    {
        var temperature = await client.LocationCurrentTemperature(latitude, longitude);
        return new(temperature);
    }

    public async Task<List<CurrentWeather>> GetTemperaturesForLocations(List<(decimal lat, decimal lon)> locations)
    {
        var tasks = locations.Select(loc => client.LocationCurrentTemperature(loc.lat, loc.lon));
        var temperatures = await Task.WhenAll(tasks);
        return temperatures.Select(t => new CurrentWeather(t)).ToList();
    }
}