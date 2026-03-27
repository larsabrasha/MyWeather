namespace TrmlWeather.Models;

public record HourlyForecast(int Hour, float Temperature, WeatherCondition Condition);

public enum WeatherCondition
{
    Clear,
    PartlyCloudy,
    Cloudy,
    Rain,
    Snow,
    Fog
}
