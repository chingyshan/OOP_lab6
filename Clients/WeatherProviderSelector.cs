namespace Forecast.Clients;

public class WeatherProviderSelector(IWeatherDataClient openWeather, IWeatherDataClient google)
{
    public IWeatherDataClient GetProvider(string provider) => provider.ToLower() switch
    {
        "openweather" => openWeather,
        "google" => google,
        _ => throw new ArgumentException($"Unknown provider: {provider}")
    };
}