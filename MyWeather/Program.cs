using MyWeather.Rendering;
using MyWeather.Services;

var builder = WebApplication.CreateBuilder(args);

var weatherSource = builder.Configuration.GetValue<string>("Weather:Source") ?? "smhi";

builder.Services.AddHttpClient("Yr", (sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var baseUrl = config.GetValue<string>("Weather:YrBaseUrl")
                  ?? "https://api.met.no/weatherapi/locationforecast/2.0/";
    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.UserAgent.ParseAdd("MyWeather/1.0 (github.com/larsabrasha/myweather)");
});

if (weatherSource.Equals("mock", StringComparison.OrdinalIgnoreCase))
    builder.Services.AddSingleton<IWeatherService, MockWeatherService>();
else
    builder.Services.AddSingleton<IWeatherService, YrWeatherService>();

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
