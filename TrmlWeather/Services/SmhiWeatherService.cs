using System.Text.Json;
using TrmlWeather.Models;

namespace TrmlWeather.Services;

public class SmhiWeatherService : IWeatherService
{
    private static readonly HttpClient HttpClient = new()
    {
        DefaultRequestHeaders = { { "User-Agent", "TrmlWeather/1.0" } }
    };

    public async Task<List<HourlyForecast>> GetForecastsAsync(double latitude, double longitude)
    {
        var lonStr = longitude.ToString("F6", System.Globalization.CultureInfo.InvariantCulture);
        var latStr = latitude.ToString("F6", System.Globalization.CultureInfo.InvariantCulture);
        var url = $"https://opendata-download-metfcst.smhi.se/api/category/pmp3g/version/2/geotype/point/lon/{lonStr}/lat/{latStr}/data.json";

        var json = await HttpClient.GetStringAsync(url);
        var doc = JsonDocument.Parse(json);

        var today = DateTime.Now.Date;
        var forecasts = new List<HourlyForecast>();

        foreach (var timeSeries in doc.RootElement.GetProperty("timeSeries").EnumerateArray())
        {
            var validTime = DateTime.Parse(timeSeries.GetProperty("validTime").GetString()!).ToLocalTime();

            int hour;
            if (validTime.Date == today)
            {
                hour = validTime.Hour;
            }
            else if (validTime.Date == today.AddDays(1) && validTime.Hour == 0)
            {
                hour = 24; // Midnatt = slutet av dagens tidslinje
            }
            else
            {
                continue;
            }
            float temperature = 0;
            int wsymb2 = 1;
            float windSpeed = 0;

            foreach (var param in timeSeries.GetProperty("parameters").EnumerateArray())
            {
                var name = param.GetProperty("name").GetString();
                var value = param.GetProperty("values")[0].GetSingle();

                switch (name)
                {
                    case "t":
                        temperature = value;
                        break;
                    case "Wsymb2":
                        wsymb2 = (int)value;
                        break;
                    case "ws":
                        windSpeed = value;
                        break;
                }
            }

            var condition = MapWsymb2(wsymb2);
            forecasts.Add(new HourlyForecast(hour, temperature, condition, windSpeed));
        }

        // If we have no data for today (e.g. late at night), take tomorrow
        if (forecasts.Count < 4)
        {
            var tomorrow = today.AddDays(1);
            foreach (var timeSeries in doc.RootElement.GetProperty("timeSeries").EnumerateArray())
            {
                var validTime = DateTime.Parse(timeSeries.GetProperty("validTime").GetString()!).ToLocalTime();
                if (validTime.Date != tomorrow) continue;

                var hour = validTime.Hour;
                float temperature = 0;
                int wsymb2 = 1;
                float windSpeed = 0;

                foreach (var param in timeSeries.GetProperty("parameters").EnumerateArray())
                {
                    var name = param.GetProperty("name").GetString();
                    var value = param.GetProperty("values")[0].GetSingle();

                    switch (name)
                    {
                        case "t":
                            temperature = value;
                            break;
                        case "Wsymb2":
                            wsymb2 = (int)value;
                            break;
                        case "ws":
                            windSpeed = value;
                            break;
                    }
                }

                forecasts.Add(new HourlyForecast(hour, temperature, MapWsymb2(wsymb2), windSpeed));
            }
        }

        return forecasts.OrderBy(f => f.Hour).ToList();
    }

    private static WeatherCondition MapWsymb2(int wsymb2) => wsymb2 switch
    {
        1 or 2 => WeatherCondition.Clear,
        3 or 4 => WeatherCondition.PartlyCloudy,
        5 or 6 => WeatherCondition.Cloudy,
        7 => WeatherCondition.Fog,
        8 or 9 or 10 or 11 or 18 or 19 or 20 or 21 => WeatherCondition.Rain,
        12 or 13 or 14 or 22 or 23 or 24 => WeatherCondition.Rain, // sleet → rain
        15 or 16 or 17 or 25 or 26 or 27 => WeatherCondition.Snow,
        _ => WeatherCondition.Cloudy
    };
}
