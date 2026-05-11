using DotEnv.Core;
using Forecast.Api;
using Forecast.Clients;
using Forecast.Controllers;
using Forecast.Utils;

var builder = WebApplication.CreateBuilder(args);

new EnvLoader().Load();
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "WeatherExampleAPI";
    config.Title = "Weather Example API";
    config.Version = "v1";
});

builder.Services.AddHttpClient<OpenWeatherDataClient>();
builder.Services.AddHttpClient<GoogleWeatherDataClient>();
builder.Services.AddSingleton<OpenWeatherDataClient>();
builder.Services.AddSingleton<GoogleWeatherDataClient>();
builder.Services.AddSingleton<WeatherProviderSelector>(sp => new WeatherProviderSelector(
    sp.GetRequiredService<OpenWeatherDataClient>(),
    sp.GetRequiredService<GoogleWeatherDataClient>()
));
builder.Services.AddSingleton<CurrentWeatherController>(sp =>
    new CurrentWeatherController(sp.GetRequiredService<OpenWeatherDataClient>()));
builder.Services.AddSingleton<WeatherForecastController>(sp =>
    new WeatherForecastController(sp.GetRequiredService<OpenWeatherDataClient>()));
builder.Services.AddSingleton<CityLocationResolver>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
    app.UseDeveloperExceptionPage();
}

app.MapGroup("/api/v1").MapCurrentWeatherApi();
app.Run();