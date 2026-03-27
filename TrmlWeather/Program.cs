using TrmlWeather.Rendering;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseKestrel(options =>
{
    options.ListenAnyIP(5001);
});

var app = builder.Build();

app.MapGet("/weather", () =>
{
    var imageBytes = WeatherImageRenderer.Render();
    return Results.File(imageBytes, "image/png");
});

app.Run();
