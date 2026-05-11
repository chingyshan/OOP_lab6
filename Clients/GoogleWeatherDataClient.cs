using System.Text.Json.Serialization;
using Forecast.Models.Weather;
using Forecast.Utils;

namespace Forecast.Clients;

public class GoogleWeatherDataClient : IWeatherDataClient
{
    private readonly HttpClient client;
    private readonly string apiKey;

    public GoogleWeatherDataClient(IConfiguration config, HttpClient httpClient)
    {
        client = httpClient;
        client.BaseAddress = new Uri(config.GetValue<string>("GOOGLE_WEATHER_BASE_URL") ?? "");
        apiKey = config.GetValue<string>("GOOGLE_WEATHER_API_KEY") ?? "";
    }

    public async Task<decimal> LocationCurrentTemperature(decimal latitude, decimal longitude)
    {
        try
        {
            var response = await client.GetAsync(
                $"forecast?location.latitude={latitude}&location.longitude={longitude}&key={apiKey}&pageSize=1"
            );
            if (!response.IsSuccessStatusCode)
                throw new ApiCallException($"google weather returned bad status: {(ushort)response.StatusCode}");

            var data = await response.Content.ReadFromJsonAsync<GoogleWeatherResponse>();
            return data?.CurrentConditions?.Temperature?.Degrees
                ?? throw new ApiCallException("failed to decode response");
        }
        catch (HttpRequestException e)
        {
            throw new ApiCallException($"failed to call google weather: {e.Message}", e);
        }
    }

    public async Task<List<ForecastDay>> LocationForecast(decimal latitude, decimal longitude)
    {
        try
        {
            var response = await client.GetAsync(
                $"forecast?location.latitude={latitude}&location.longitude={longitude}&key={apiKey}&pageSize=5"
            );
            if (!response.IsSuccessStatusCode)
                throw new ApiCallException($"google weather returned bad status: {(ushort)response.StatusCode}");

            var data = await response.Content.ReadFromJsonAsync<GoogleWeatherForecastResponse>();
            return data?.ForecastDays?.Select(item => new ForecastDay(
                DateTime.Parse(item.Interval.StartTime),
                item.MaxTemperature.Degrees
            )).ToList() ?? throw new ApiCallException("failed to decode response");
        }
        catch (HttpRequestException e)
        {
            throw new ApiCallException($"failed to call google weather: {e.Message}", e);
        }
    }
}

class GoogleWeatherResponse
{
    [JsonPropertyName("currentConditions")]
    public CurrentConditions? CurrentConditions { get; set; }
}

class CurrentConditions
{
    [JsonPropertyName("temperature")]
    public Temperature? Temperature { get; set; }
}

class Temperature
{
    [JsonPropertyName("degrees")]
    public decimal Degrees { get; set; }
}

class GoogleWeatherForecastResponse
{
    [JsonPropertyName("forecastDays")]
    public List<GoogleForecastDay>? ForecastDays { get; set; }
}

class GoogleForecastDay
{
    [JsonPropertyName("interval")]
    public required GoogleInterval Interval { get; set; }

    [JsonPropertyName("maxTemperature")]
    public required Temperature MaxTemperature { get; set; }
}

class GoogleInterval
{
    [JsonPropertyName("startTime")]
    public required string StartTime { get; set; }
}