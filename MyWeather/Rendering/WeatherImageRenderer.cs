using SkiaSharp;
using MyWeather.Helpers;
using MyWeather.Models;

namespace MyWeather.Rendering;

public static class WeatherImageRenderer
{
    private const int Width = 1024;
    private const int Height = 768;
    private const float LeftMargin = 40f;
    private const float RightMargin = 40f;
    private const float TimelineWidth = Width - LeftMargin - RightMargin;
    private const float MinTemp = -15f;
    private const float MaxTemp = 35f;
    private const float GraphTop = 210f;
    private const float GraphBottom = 700f;
    private const float TimelineY = GraphBottom - (-MinTemp / (MaxTemp - MinTemp)) * (GraphBottom - GraphTop);

    public static byte[] Render(List<HourlyForecast> forecasts)
    {
        var now = DateTime.Now;

        using var surface = SKSurface.Create(new SKImageInfo(Width, Height));
        var canvas = surface.Canvas;
        canvas.Clear(SKColors.White);

        DrawHeader(canvas, forecasts, now);
        DrawTimeline(canvas, now);
        DrawTemperatureCurve(canvas, forecasts);
        DrawHourlyMarkers(canvas, forecasts);
        DrawCurrentTimeLine(canvas, now);

        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }

    private static void DrawHeader(SKCanvas canvas, List<HourlyForecast> forecasts, DateTime now)
    {
        var currentHour = now.Hour;
        var current = forecasts.MinBy(f => Math.Abs(f.Hour - currentHour)) ?? forecasts[0];

        using var paint = new SKPaint { Color = SKColors.Black, IsAntialias = true };

        using var bigFont = new SKFont(SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold), 64);
        using var medFont = new SKFont(SKTypeface.FromFamilyName("Arial"), 28);

        WeatherIconRenderer.Draw(canvas, current.SymbolCode, 80, 90, 50);

        canvas.DrawText($"{current.Temperature:+0;-0;0}°C", 160, 100, SKTextAlign.Left, bigFont, paint);
        canvas.DrawText($"{current.WindSpeed:0} m/s", 160, 135, SKTextAlign.Left, medFont, paint);
        canvas.DrawText(SwedishDateHelper.GetConditionText(current.Condition), 160, 165, SKTextAlign.Left, medFont, paint);
    }

    private static void DrawTimeline(SKCanvas canvas, DateTime now)
    {
        using var linePaint = new SKPaint
        {
            Color = SKColors.Black, IsAntialias = true, StrokeWidth = 2, Style = SKPaintStyle.Stroke
        };
        using var tickPaint = new SKPaint
        {
            Color = SKColors.Black, IsAntialias = true, StrokeWidth = 1.5f, Style = SKPaintStyle.Stroke
        };
        using var textPaint = new SKPaint { Color = SKColors.Black, IsAntialias = true };
        using var hourFont = new SKFont(SKTypeface.FromFamilyName("Arial"), 14);
        using var dayFont = new SKFont(SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold), 32);

        canvas.DrawLine(LeftMargin, TimelineY, Width - RightMargin, TimelineY, linePaint);

        for (int h = 0; h <= 48; h += 4)
        {
            float x = HourToX(h);
            var usedPaint = (h % 24 == 0) ? linePaint : tickPaint;
            canvas.DrawLine(x, TimelineY - 8, x, TimelineY + 8, usedPaint);
            canvas.DrawText((h % 24).ToString("D2"), x, TimelineY + 25, SKTextAlign.Center, hourFont, textPaint);
        }

        // Day labels
        using var dayLabelPaint = new SKPaint { Color = SKColors.Black, IsAntialias = true };
        float todayX = (HourToX(0) + HourToX(24)) / 2;
        float tomorrowX = (HourToX(24) + HourToX(48)) / 2;
        var today = now.Date;
        var tomorrow = today.AddDays(1);
        var todayLabel = $"Idag, {SwedishDateHelper.GetDayName(now.DayOfWeek).ToLower()} {today.Day} {SwedishDateHelper.GetMonthName(today.Month)}";
        var tomorrowLabel = $"Imorgon, {SwedishDateHelper.GetDayName(tomorrow.DayOfWeek).ToLower()} {tomorrow.Day} {SwedishDateHelper.GetMonthName(tomorrow.Month)}";
        canvas.DrawText(todayLabel, todayX, GraphBottom + 50, SKTextAlign.Center, dayFont, dayLabelPaint);
        canvas.DrawText(tomorrowLabel, tomorrowX, GraphBottom + 50, SKTextAlign.Center, dayFont, dayLabelPaint);
    }

    private static void DrawTemperatureCurve(SKCanvas canvas, List<HourlyForecast> forecasts)
    {
        using var curvePaint = new SKPaint
        {
            Color = SKColors.Black, IsAntialias = true, StrokeWidth = 3, Style = SKPaintStyle.Stroke
        };

        var points = forecasts.OrderBy(f => f.Hour)
            .Select(f => new SKPoint(HourToX(f.Hour), TempToY(f.Temperature)))
            .ToList();

        if (points.Count < 2) return;

        var path = new SKPath();
        path.MoveTo(points[0]);

        for (int i = 0; i < points.Count - 1; i++)
        {
            var p0 = i > 0 ? points[i - 1] : points[i];
            var p1 = points[i];
            var p2 = points[i + 1];
            var p3 = i + 2 < points.Count ? points[i + 2] : points[i + 1];

            var cp1 = new SKPoint(
                p1.X + (p2.X - p0.X) / 6f,
                p1.Y + (p2.Y - p0.Y) / 6f);
            var cp2 = new SKPoint(
                p2.X - (p3.X - p1.X) / 6f,
                p2.Y - (p3.Y - p1.Y) / 6f);

            path.CubicTo(cp1, cp2, p2);
        }

        canvas.DrawPath(path, curvePaint);
    }

    private static void DrawHourlyMarkers(SKCanvas canvas, List<HourlyForecast> forecasts)
    {
        using var paint = new SKPaint { Color = SKColors.Black, IsAntialias = true };
        using var tempFont = new SKFont(SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold), 28);

        foreach (var f in forecasts.Where(f => f.Hour % 4 == 0))
        {
            float x = HourToX(f.Hour);
            float y = TempToY(f.Temperature);

            bool aboveLine = f.Temperature >= 0;
            float iconY = aboveLine ? y - 60 : y + 40;
            WeatherIconRenderer.Draw(canvas, f.SymbolCode, x, iconY, 18);

            float tempLabelY = aboveLine ? y - 20 : y + 85;
            canvas.DrawText($"{f.Temperature:0}°", x, tempLabelY, SKTextAlign.Center, tempFont, paint);
        }
    }

    private static void DrawCurrentTimeLine(SKCanvas canvas, DateTime now)
    {
        float currentHourFraction = now.Hour + now.Minute / 60f;
        float x = HourToX(currentHourFraction);

        using var paint = new SKPaint
        {
            Color = SKColors.Black, IsAntialias = true, StrokeWidth = 1.5f,
            Style = SKPaintStyle.Stroke,
            PathEffect = SKPathEffect.CreateDash([8, 4], 0)
        };

        canvas.DrawLine(x, 40, x, Height - 40, paint);
    }

    private static float HourToX(float hour) => LeftMargin + (hour / 48f) * TimelineWidth;

    private static float TempToY(float temp) => GraphBottom - ((temp - MinTemp) / (MaxTemp - MinTemp)) * (GraphBottom - GraphTop);
}
