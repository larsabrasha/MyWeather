using MyWeather.Models;

namespace MyWeather.Services;

public interface IWeatherService
{
    Task<List<HourlyForecast>> GetForecastsAsync(double latitude, double longitude);
}
