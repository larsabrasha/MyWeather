using MyWeather.Rendering;
using MyWeather.Services;

var builder = WebApplication.CreateBuilder(args);

var weatherSource = builder.Configuration.GetValue<string>("Weather:Source") ?? "smhi";

builder.Services.AddHttpClient("Smhi", client =>
{
    client.DefaultRequestHeaders.Add("User-Agent", "MyWeather/1.0");
});

if (weatherSource.Equals("mock", StringComparison.OrdinalIgnoreCase))
    builder.Services.AddSingleton<IWeatherService, MockWeatherService>();
else
    builder.Services.AddSingleton<IWeatherService, SmhiWeatherService>();

var app = builder.Build();

var lat = app.Configuration.GetValue<double>("Weather:Latitude", 59.2753);
var lon = app.Configuration.GetValue<double>("Weather:Longitude", 15.2134);

app.MapGet("/weather", async (IWeatherService weatherService) =>
{
    var forecasts = await weatherService.GetForecastsAsync(lat, lon);
    var imageBytes = WeatherImageRenderer.Render(forecasts);
    return Results.File(imageBytes, "image/png");
});

app.Run();
