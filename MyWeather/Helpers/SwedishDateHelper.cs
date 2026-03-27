using MyWeather.Models;

namespace MyWeather.Helpers;

public static class SwedishDateHelper
{
    public static string GetDayName(DayOfWeek day) => day switch
    {
        DayOfWeek.Monday => "Måndag",
        DayOfWeek.Tuesday => "Tisdag",
        DayOfWeek.Wednesday => "Onsdag",
        DayOfWeek.Thursday => "Torsdag",
        DayOfWeek.Friday => "Fredag",
        DayOfWeek.Saturday => "Lördag",
        DayOfWeek.Sunday => "Söndag",
        _ => ""
    };

    public static string GetMonthName(int month) => month switch
    {
        1 => "januari", 2 => "februari", 3 => "mars", 4 => "april",
        5 => "maj", 6 => "juni", 7 => "juli", 8 => "augusti",
        9 => "september", 10 => "oktober", 11 => "november", 12 => "december",
        _ => ""
    };

    public static string GetConditionText(WeatherCondition condition) => condition switch
    {
        WeatherCondition.Clear => "Klart",
        WeatherCondition.PartlyCloudy => "Halvklart",
        WeatherCondition.Cloudy => "Mulet",
        WeatherCondition.Rain => "Regn",
        WeatherCondition.Snow => "Snö",
        WeatherCondition.Fog => "Dimma",
        _ => ""
    };
}
