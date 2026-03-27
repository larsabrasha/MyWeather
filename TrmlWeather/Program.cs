using TrmlWeather.Rendering;
using TrmlWeather.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseKestrel(options =>
{
    options.ListenAnyIP(5001);
});

var weatherSource = builder.Configuration.GetValue<string>("Weather:Source") ?? "smhi";

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
