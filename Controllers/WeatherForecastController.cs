using Forecast.Clients;
using Forecast.Models.Weather;

namespace Forecast.Controllers;

public class WeatherForecastController(IWeatherDataClient client)
{
    public async Task<WeatherForecast> GetForecast(decimal latitude, decimal longitude)
    {
        throw new NotImplementedException();
    }
}