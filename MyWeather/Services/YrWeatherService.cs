using System.Text.Json;
using MyWeather.Models;

namespace MyWeather.Services;

public class YrWeatherService : IWeatherService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public YrWeatherService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<List<HourlyForecast>> GetForecastsAsync(double latitude, double longitude)
    {
        var latStr = latitude.ToString("F4", System.Globalization.CultureInfo.InvariantCulture);
        var lonStr = longitude.ToString("F4", System.Globalization.CultureInfo.InvariantCulture);

        var httpClient = _httpClientFactory.CreateClient("Yr");
        var json = await httpClient.GetStringAsync($"compact?lat={latStr}&lon={lonStr}");
        var doc = JsonDocument.Parse(json);

        var today = DateTime.Now.Date;
        var tomorrow = today.AddDays(1);
        var forecasts = new List<HourlyForecast>();

        foreach (var timeseries in doc.RootElement
                     .GetProperty("properties")
                     .GetProperty("timeseries")
                     .EnumerateArray())
        {
            var time = DateTime.Parse(timeseries.GetProperty("time").GetString()!).ToLocalTime();

            int hour;
            if (time.Date == today)
            {
                hour = time.Hour;
            }
            else if (time.Date == tomorrow)
            {
                hour = 24 + time.Hour;
            }
            else if (time.Date == tomorrow.AddDays(1) && time.Hour == 0)
            {
                hour = 48;
            }
            else
            {
                continue;
            }

            var instant = timeseries.GetProperty("data")
                .GetProperty("instant")
                .GetProperty("details");

            var temperature = instant.GetProperty("air_temperature").GetSingle();
            var windSpeed = instant.GetProperty("wind_speed").GetSingle();

            var symbolCode = "cloudy";
            if (timeseries.GetProperty("data").TryGetProperty("next_1_hours", out var next1))
            {
                symbolCode = next1.GetProperty("summary").GetProperty("symbol_code").GetString()!;
            }
            else if (timeseries.GetProperty("data").TryGetProperty("next_6_hours", out var next6))
            {
                symbolCode = next6.GetProperty("summary").GetProperty("symbol_code").GetString()!;
            }

            forecasts.Add(new HourlyForecast(hour, temperature, MapSymbolCode(symbolCode), windSpeed, symbolCode));
        }

        return forecasts.OrderBy(f => f.Hour).ToList();
    }

    private static WeatherCondition MapSymbolCode(string symbolCode)
    {
        // Yr symbol codes use a base name, optionally followed by _day/_night/_polartwilight
        var baseName = symbolCode.Split('_')[0];

        return baseName switch
        {
            "clearsky" => WeatherCondition.Clear,
            "fair" => WeatherCondition.Clear,
            "partlycloudy" => WeatherCondition.PartlyCloudy,
            "cloudy" => WeatherCondition.Cloudy,
            "fog" => WeatherCondition.Fog,
            "lightrain" or "rain" or "heavyrain" => WeatherCondition.Rain,
            "lightrainshowers" or "rainshowers" or "heavyrainshowers" => WeatherCondition.Rain,
            "lightrainandthunder" or "rainandthunder" or "heavyrainandthunder" => WeatherCondition.Rain,
            "lightrainshowersandthunder" or "rainshowersandthunder" or "heavyrainshowersandthunder" => WeatherCondition.Rain,
            "lightsleet" or "sleet" or "heavysleet" => WeatherCondition.Rain,
            "lightsleetshowers" or "sleetshowers" or "heavysleetshowers" => WeatherCondition.Rain,
            "lightsleetandthunder" or "sleetandthunder" or "heavysleetandthunder" => WeatherCondition.Rain,
            "lightsleetshowersandthunder" or "sleetshowersandthunder" or "heavysleetshowersandthunder" => WeatherCondition.Rain,
            "lightsnow" or "snow" or "heavysnow" => WeatherCondition.Snow,
            "lightsnowshowers" or "snowshowers" or "heavysnowshowers" => WeatherCondition.Snow,
            "lightsnowandthunder" or "snowandthunder" or "heavysnowandthunder" => WeatherCondition.Snow,
            "lightsnowshowersandthunder" or "snowshowersandthunder" or "heavysnowshowersandthunder" => WeatherCondition.Snow,
            _ => WeatherCondition.Cloudy
        };
    }
}
