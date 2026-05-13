using System.Text.Json.Serialization;
using Forecast.Models.Weather;
using Forecast.Utils;

namespace Forecast.Clients;

public class OpenWeatherDataClient : IWeatherDataClient
{
    private readonly HttpClient client;
    private readonly string apiKey;

    public OpenWeatherDataClient(IConfiguration config, HttpClient httpClient)
    {
        client = httpClient;
        client.BaseAddress = new Uri(config.GetValue<string>("OPENWEATHER_BASE_URL") ?? "");
        apiKey = config.GetValue<string>("OPENWEATHER_API_KEY") ?? "";
    }

    public async Task<decimal> LocationCurrentTemperature(decimal latitude, decimal longitude)
    {
        try
        {
            var response = await client.GetAsync(
                $"?lat={latitude}&lon={longitude}&appid={apiKey}&units=metric"
            );
            if (!response.IsSuccessStatusCode)
                throw new ApiCallException($"openweather returned bad status: {(ushort)response.StatusCode}");

            var data = await response.Content.ReadFromJsonAsync<OpenWeatherResponse>();
            return data?.Main?.Temp ?? throw new ApiCallException("failed to decode response");
        }
        catch (HttpRequestException e)
        {
            throw new ApiCallException($"failed to call openweather: {e.Message}", e);
        }
    }

    public async Task<List<ForecastDay>> LocationForecast(decimal latitude, decimal longitude)
    {
        try
        {
            var response = await client.GetAsync(
                $"forecast?lat={latitude}&lon={longitude}&appid={apiKey}&units=metric&cnt=5"
            );
            if (!response.IsSuccessStatusCode)
                throw new ApiCallException($"openweather returned bad status: {(ushort)response.StatusCode}");

            var data = await response.Content.ReadFromJsonAsync<OpenWeatherForecastResponse>();
            return data?.List?.Select(item => new ForecastDay(
                DateTimeOffset.FromUnixTimeSeconds(item.Dt).DateTime,
                item.Main.Temp
            )).ToList() ?? throw new ApiCallException("failed to decode response");
        }
        catch (HttpRequestException e)
        {
            throw new ApiCallException($"failed to call openweather: {e.Message}", e);
        }
    }
}

class OpenWeatherResponse
{
    [JsonPropertyName("main")]
    public required Nested Main { get; set; }

    public class Nested
    {
        [JsonPropertyName("temp")]
        public decimal Temp { get; set; }
    }
}

class OpenWeatherForecastResponse
{
    [JsonPropertyName("list")]
    public required List<OpenWeatherForecastItem> List { get; set; }
}

class OpenWeatherForecastItem
{
    [JsonPropertyName("dt")]
    public long Dt { get; set; }

    [JsonPropertyName("main")]
    public required OpenWeatherResponse.Nested Main { get; set; }
}