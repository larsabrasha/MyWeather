using TrmlWeather.Models;

namespace TrmlWeather.Services;

public interface IWeatherService
{
    Task<List<HourlyForecast>> GetForecastsAsync(double latitude, double longitude);
}
