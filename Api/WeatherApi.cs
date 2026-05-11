using System.ComponentModel;
using Forecast.Clients;
using Forecast.Controllers;
using Forecast.Models.Weather;
using Forecast.Shared.Responses;
using Forecast.Utils;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Forecast.Api;

public static class WeatherApi
{
    public static RouteGroupBuilder MapCurrentWeatherApi(this RouteGroupBuilder groups)
    {
        groups
            .MapGet("weather", HandleGetCurrentWeather)
            .WithName("GetCurrentWeather")
            .WithTags("weather")
            .WithDescription("Returns current weather for given coordinates or city name");

        groups
            .MapGet("weather/cities", HandleGetWeatherForCities)
            .WithName("GetWeatherForCities")
            .WithTags("weather")
            .WithDescription("Returns current temperature for multiple cities");

        groups
            .MapGet("forecast", HandleGetForecast)
            .WithName("GetForecast")
            .WithTags("forecast")
            .WithDescription("Returns weather forecast for given coordinates or city name");

        return groups;
    }

    private static async Task<Results<Ok<Success<CurrentWeather>>, BadRequest<Status>, InternalServerError<Status>>>
        HandleGetCurrentWeather(
            [FromServices] CurrentWeatherController controller,
            [FromServices] WeatherProviderSelector selector,
            [FromServices] CityLocationResolver resolver,
            [DefaultValue("openweather")] string provider,
            string? city,
            [DefaultValue("18.300231990440125")] string? lat,
            [DefaultValue("-64.8251590359234")] string? lon)
    {
        try
        {
            var (latitude, longitude) = ResolveLocation(city, lat, lon, resolver);
            var client = selector.GetProvider(provider);
            var weatherController = new CurrentWeatherController(client);
            var weather = await weatherController.GetCurrentWeather(latitude, longitude);
            return TypedResults.Ok(Success.Create(200, "success", weather));
        }
        catch (FormatException)
        {
            return TypedResults.BadRequest(Status.Create(400, "invalid coordinates"));
        }
        catch (ArgumentException e)
        {
            return TypedResults.BadRequest(Status.Create(400, e.Message));
        }
        catch (ApiCallException e)
        {
            return TypedResults.InternalServerError(Status.Create(500, e.Message));
        }
    }

    private static async Task<Results<Ok<Success<List<CurrentWeather>>>, BadRequest<Status>, InternalServerError<Status>>>
        HandleGetWeatherForCities(
            [FromServices] CurrentWeatherController controller,
            [FromServices] CityLocationResolver resolver,
            [DefaultValue("london,tokyo,warsaw")] string cities)
    {
        try
        {
            var locations = cities.Split(',')
                .Select(c => resolver.Resolve(c.Trim()))
                .ToList();
            var result = await controller.GetTemperaturesForLocations(locations);
            return TypedResults.Ok(Success.Create(200, "success", result));
        }
        catch (ArgumentException e)
        {
            return TypedResults.BadRequest(Status.Create(400, e.Message));
        }
        catch (ApiCallException e)
        {
            return TypedResults.InternalServerError(Status.Create(500, e.Message));
        }
    }

    private static async Task<Results<Ok<Success<WeatherForecast>>, BadRequest<Status>, InternalServerError<Status>>>
        HandleGetForecast(
            [FromServices] WeatherForecastController controller,
            [FromServices] WeatherProviderSelector selector,
            [FromServices] CityLocationResolver resolver,
            [DefaultValue("openweather")] string provider,
            string? city,
            [DefaultValue("18.300231990440125")] string? lat,
            [DefaultValue("-64.8251590359234")] string? lon)
    {
        try
        {
            var (latitude, longitude) = ResolveLocation(city, lat, lon, resolver);
            var client = selector.GetProvider(provider);
            var forecastController = new WeatherForecastController(client);
            var forecast = await forecastController.GetForecast(latitude, longitude);
            return TypedResults.Ok(Success.Create(200, "success", forecast));
        }
        catch (FormatException)
        {
            return TypedResults.BadRequest(Status.Create(400, "invalid coordinates"));
        }
        catch (ArgumentException e)
        {
            return TypedResults.BadRequest(Status.Create(400, e.Message));
        }
        catch (ApiCallException e)
        {
            return TypedResults.InternalServerError(Status.Create(500, e.Message));
        }
    }

    private static (decimal lat, decimal lon) ResolveLocation(
        string? city, string? lat, string? lon, CityLocationResolver resolver)
    {
        if (!string.IsNullOrEmpty(city))
            return resolver.Resolve(city);

        return (decimal.Parse(lat ?? "0"), decimal.Parse(lon ?? "0"));
    }
}