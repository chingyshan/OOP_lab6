namespace Forecast.Clients;

public class WeatherProviderSelector(IWeatherDataClient openWeather, IWeatherDataClient google)
{
    public IWeatherDataClient GetProvider(string provider)
    {
        throw new NotImplementedException();
    }
}