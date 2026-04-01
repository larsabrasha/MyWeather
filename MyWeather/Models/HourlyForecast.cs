namespace MyWeather.Models;

public record HourlyForecast(int Hour, float Temperature, WeatherCondition Condition, float WindSpeed = 0, string SymbolCode = "cloudy");

public enum WeatherCondition
{
    Clear,
    PartlyCloudy,
    Cloudy,
    Rain,
    Snow,
    Fog
}
