using TrmlWeather.Models;

namespace TrmlWeather.Services;

public static class SampleWeatherService
{
    public static List<HourlyForecast> GetForecasts()
    {
        return
        [
            new(0, -1, WeatherCondition.Clear),
            new(2, -2, WeatherCondition.Clear),
            new(4, -2, WeatherCondition.Clear),
            new(6, -1, WeatherCondition.Clear),
            new(8, 2, WeatherCondition.PartlyCloudy),
            new(10, 5, WeatherCondition.PartlyCloudy),
            new(12, 7, WeatherCondition.PartlyCloudy),
            new(14, 6, WeatherCondition.Cloudy),
            new(16, 4, WeatherCondition.Cloudy),
            new(18, 2, WeatherCondition.Cloudy),
            new(20, 0, WeatherCondition.Clear),
            new(22, -1, WeatherCondition.Clear),
            new(24, -2, WeatherCondition.Clear),
        ];
    }
}
