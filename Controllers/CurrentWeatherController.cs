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

    public Task<List<CurrentWeather>> GetTemperaturesForLocations(List<(decimal lat, decimal lon)> locations)
    {
        throw new NotImplementedException();
    }
}