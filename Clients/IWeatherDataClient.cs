using Forecast.Models.Weather;

namespace Forecast.Clients;

public interface IWeatherDataClient
{
    Task<decimal> LocationCurrentTemperature(decimal latitude, decimal longitude);
    Task<List<ForecastDay>> LocationForecast(decimal latitude, decimal longitude);
}