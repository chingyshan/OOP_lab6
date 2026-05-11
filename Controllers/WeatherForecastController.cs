using Forecast.Clients;
using Forecast.Models.Weather;

namespace Forecast.Controllers;

public class WeatherForecastController(IWeatherDataClient client)
{
    public async Task<WeatherForecast> GetForecast(decimal latitude, decimal longitude)
    {
        var days = await client.LocationForecast(latitude, longitude);
        return new WeatherForecast(days);
    }
}