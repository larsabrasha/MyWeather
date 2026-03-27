using TrmlWeather.Models;

namespace TrmlWeather.Services;

/// <summary>
/// Mock weather service with seasonal scenarios typical for Örebro/central Sweden.
/// </summary>
public class MockWeatherService : IWeatherService
{
    public Task<List<HourlyForecast>> GetForecastsAsync(double latitude, double longitude)
    {
        var month = DateTime.Now.Month;
        var forecasts = GetSeasonalForecasts(month);
        return Task.FromResult(forecasts);
    }

    private static List<HourlyForecast> GetSeasonalForecasts(int month) => month switch
    {
        // January: Kall vinterdag, klart på morgonen, snö på eftermiddagen
        1 => [
            new(0, -12, WeatherCondition.Clear, 2),
            new(2, -14, WeatherCondition.Clear, 1),
            new(4, -15, WeatherCondition.Clear, 1),
            new(6, -14, WeatherCondition.Clear, 2),
            new(8, -10, WeatherCondition.PartlyCloudy, 3),
            new(10, -7, WeatherCondition.Cloudy, 3),
            new(12, -5, WeatherCondition.Snow, 4),
            new(14, -4, WeatherCondition.Snow, 4),
            new(16, -6, WeatherCondition.Snow, 3),
            new(18, -8, WeatherCondition.Cloudy, 2),
            new(20, -10, WeatherCondition.PartlyCloudy, 2),
            new(22, -11, WeatherCondition.Clear, 1),
            new(24, -13, WeatherCondition.Clear, 1),
        ],

        // February: Kall men solig vinterdag
        2 => [
            new(0, -8, WeatherCondition.Clear, 2),
            new(2, -10, WeatherCondition.Clear, 1),
            new(4, -11, WeatherCondition.Clear, 1),
            new(6, -9, WeatherCondition.Clear, 2),
            new(8, -5, WeatherCondition.Clear, 2),
            new(10, -2, WeatherCondition.PartlyCloudy, 3),
            new(12, 0, WeatherCondition.PartlyCloudy, 3),
            new(14, 1, WeatherCondition.PartlyCloudy, 3),
            new(16, -1, WeatherCondition.Clear, 2),
            new(18, -4, WeatherCondition.Clear, 2),
            new(20, -6, WeatherCondition.Clear, 1),
            new(22, -7, WeatherCondition.Clear, 1),
            new(24, -9, WeatherCondition.Clear, 1),
        ],

        // Mars: Vårvinter, plusgrader mitt på dagen
        3 => [
            new(0, -3, WeatherCondition.Clear, 3),
            new(2, -4, WeatherCondition.Clear, 2),
            new(4, -4, WeatherCondition.Clear, 2),
            new(6, -2, WeatherCondition.PartlyCloudy, 3),
            new(8, 1, WeatherCondition.PartlyCloudy, 4),
            new(10, 4, WeatherCondition.PartlyCloudy, 4),
            new(12, 6, WeatherCondition.PartlyCloudy, 5),
            new(14, 7, WeatherCondition.Cloudy, 5),
            new(16, 5, WeatherCondition.Cloudy, 4),
            new(18, 2, WeatherCondition.PartlyCloudy, 3),
            new(20, 0, WeatherCondition.Clear, 2),
            new(22, -1, WeatherCondition.Clear, 2),
            new(24, -2, WeatherCondition.Clear, 2),
        ],

        // April: Vårdag med regnskurar
        4 => [
            new(0, 2, WeatherCondition.Cloudy, 4),
            new(2, 1, WeatherCondition.Cloudy, 3),
            new(4, 1, WeatherCondition.Cloudy, 3),
            new(6, 3, WeatherCondition.PartlyCloudy, 4),
            new(8, 6, WeatherCondition.PartlyCloudy, 5),
            new(10, 9, WeatherCondition.Rain, 6),
            new(12, 10, WeatherCondition.Rain, 6),
            new(14, 11, WeatherCondition.PartlyCloudy, 5),
            new(16, 9, WeatherCondition.PartlyCloudy, 4),
            new(18, 7, WeatherCondition.Cloudy, 3),
            new(20, 5, WeatherCondition.Cloudy, 3),
            new(22, 4, WeatherCondition.Cloudy, 2),
            new(24, 3, WeatherCondition.Cloudy, 2),
        ],

        // Maj: Varm vårdag
        5 => [
            new(0, 7, WeatherCondition.Clear, 2),
            new(2, 5, WeatherCondition.Clear, 1),
            new(4, 5, WeatherCondition.Clear, 1),
            new(6, 8, WeatherCondition.Clear, 2),
            new(8, 12, WeatherCondition.PartlyCloudy, 3),
            new(10, 16, WeatherCondition.PartlyCloudy, 3),
            new(12, 19, WeatherCondition.Clear, 3),
            new(14, 20, WeatherCondition.Clear, 3),
            new(16, 18, WeatherCondition.PartlyCloudy, 3),
            new(18, 15, WeatherCondition.PartlyCloudy, 2),
            new(20, 12, WeatherCondition.Clear, 2),
            new(22, 10, WeatherCondition.Clear, 1),
            new(24, 8, WeatherCondition.Clear, 1),
        ],

        // Juni: Sommardag med sol och moln
        6 => [
            new(0, 12, WeatherCondition.Clear, 2),
            new(2, 11, WeatherCondition.Clear, 1),
            new(4, 11, WeatherCondition.Clear, 1),
            new(6, 13, WeatherCondition.Clear, 2),
            new(8, 17, WeatherCondition.PartlyCloudy, 2),
            new(10, 20, WeatherCondition.PartlyCloudy, 3),
            new(12, 23, WeatherCondition.PartlyCloudy, 3),
            new(14, 24, WeatherCondition.PartlyCloudy, 3),
            new(16, 22, WeatherCondition.Cloudy, 3),
            new(18, 19, WeatherCondition.PartlyCloudy, 2),
            new(20, 16, WeatherCondition.Clear, 2),
            new(22, 14, WeatherCondition.Clear, 1),
            new(24, 13, WeatherCondition.Clear, 1),
        ],

        // Juli: Varm sommardag
        7 => [
            new(0, 15, WeatherCondition.Clear, 1),
            new(2, 14, WeatherCondition.Clear, 1),
            new(4, 14, WeatherCondition.Clear, 1),
            new(6, 16, WeatherCondition.Clear, 1),
            new(8, 20, WeatherCondition.Clear, 2),
            new(10, 24, WeatherCondition.Clear, 2),
            new(12, 27, WeatherCondition.Clear, 2),
            new(14, 28, WeatherCondition.PartlyCloudy, 3),
            new(16, 26, WeatherCondition.PartlyCloudy, 3),
            new(18, 23, WeatherCondition.Clear, 2),
            new(20, 20, WeatherCondition.Clear, 1),
            new(22, 17, WeatherCondition.Clear, 1),
            new(24, 16, WeatherCondition.Clear, 1),
        ],

        // Augusti: Sommardag med åskregn på eftermiddagen
        8 => [
            new(0, 14, WeatherCondition.Clear, 1),
            new(2, 13, WeatherCondition.Clear, 1),
            new(4, 12, WeatherCondition.Fog, 0),
            new(6, 14, WeatherCondition.PartlyCloudy, 1),
            new(8, 18, WeatherCondition.PartlyCloudy, 2),
            new(10, 22, WeatherCondition.Cloudy, 3),
            new(12, 24, WeatherCondition.Cloudy, 4),
            new(14, 23, WeatherCondition.Rain, 6),
            new(16, 20, WeatherCondition.Rain, 5),
            new(18, 18, WeatherCondition.Cloudy, 3),
            new(20, 16, WeatherCondition.PartlyCloudy, 2),
            new(22, 15, WeatherCondition.Clear, 1),
            new(24, 14, WeatherCondition.Clear, 1),
        ],

        // September: Tidig höst, svalt och mulet
        9 => [
            new(0, 8, WeatherCondition.Cloudy, 3),
            new(2, 7, WeatherCondition.Cloudy, 3),
            new(4, 6, WeatherCondition.Fog, 2),
            new(6, 7, WeatherCondition.Fog, 2),
            new(8, 9, WeatherCondition.Cloudy, 3),
            new(10, 12, WeatherCondition.Cloudy, 4),
            new(12, 14, WeatherCondition.PartlyCloudy, 4),
            new(14, 14, WeatherCondition.PartlyCloudy, 4),
            new(16, 12, WeatherCondition.Cloudy, 3),
            new(18, 10, WeatherCondition.Cloudy, 3),
            new(20, 9, WeatherCondition.Cloudy, 2),
            new(22, 8, WeatherCondition.Cloudy, 2),
            new(24, 8, WeatherCondition.Cloudy, 2),
        ],

        // Oktober: Regnig höstdag
        10 => [
            new(0, 5, WeatherCondition.Rain, 5),
            new(2, 5, WeatherCondition.Rain, 5),
            new(4, 4, WeatherCondition.Cloudy, 4),
            new(6, 4, WeatherCondition.Cloudy, 5),
            new(8, 5, WeatherCondition.Rain, 6),
            new(10, 7, WeatherCondition.Rain, 7),
            new(12, 8, WeatherCondition.Rain, 6),
            new(14, 8, WeatherCondition.Cloudy, 5),
            new(16, 7, WeatherCondition.Cloudy, 5),
            new(18, 6, WeatherCondition.Cloudy, 4),
            new(20, 5, WeatherCondition.Cloudy, 4),
            new(22, 5, WeatherCondition.Rain, 5),
            new(24, 4, WeatherCondition.Rain, 5),
        ],

        // November: Grått och rått
        11 => [
            new(0, 1, WeatherCondition.Cloudy, 4),
            new(2, 0, WeatherCondition.Cloudy, 4),
            new(4, 0, WeatherCondition.Fog, 3),
            new(6, 0, WeatherCondition.Fog, 3),
            new(8, 1, WeatherCondition.Cloudy, 4),
            new(10, 3, WeatherCondition.Cloudy, 5),
            new(12, 4, WeatherCondition.Rain, 5),
            new(14, 4, WeatherCondition.Rain, 5),
            new(16, 3, WeatherCondition.Cloudy, 4),
            new(18, 2, WeatherCondition.Cloudy, 4),
            new(20, 1, WeatherCondition.Cloudy, 3),
            new(22, 1, WeatherCondition.Cloudy, 3),
            new(24, 0, WeatherCondition.Cloudy, 3),
        ],

        // December: Snöig vinterdag
        12 => [
            new(0, -5, WeatherCondition.Cloudy, 3),
            new(2, -6, WeatherCondition.Cloudy, 3),
            new(4, -7, WeatherCondition.Cloudy, 2),
            new(6, -6, WeatherCondition.Snow, 3),
            new(8, -4, WeatherCondition.Snow, 4),
            new(10, -2, WeatherCondition.Snow, 4),
            new(12, -1, WeatherCondition.Snow, 5),
            new(14, -1, WeatherCondition.Cloudy, 4),
            new(16, -2, WeatherCondition.Cloudy, 3),
            new(18, -4, WeatherCondition.Cloudy, 3),
            new(20, -5, WeatherCondition.PartlyCloudy, 2),
            new(22, -6, WeatherCondition.Clear, 2),
            new(24, -7, WeatherCondition.Clear, 1),
        ],

        _ => throw new ArgumentOutOfRangeException(nameof(month))
    };
}
