namespace Forecast.Clients;

 public interface IWeatherDataClient
{
    Task<decimal> LocationCurrentTemperature(decimal latitude, decimal longitude);
}