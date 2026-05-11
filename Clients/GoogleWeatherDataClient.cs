using Forecast.Utils;

namespace Forecast.Clients;

public class GoogleWeatherDataClient : IWeatherDataClient
{
    public GoogleWeatherDataClient(IConfiguration config, HttpClient httpClient)
    {
        throw new NotImplementedException();
    }

    public Task<decimal> LocationCurrentTemperature(decimal latitude, decimal longitude)
    {
        throw new NotImplementedException();
    }
}