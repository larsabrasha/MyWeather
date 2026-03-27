using SkiaSharp;
using TrmlWeather.Helpers;
using TrmlWeather.Models;

namespace TrmlWeather.Rendering;

public static class WeatherImageRenderer
{
    private const int Width = 1024;
    private const int Height = 768;
    private const float TimelineY = Height * 0.5f;
    private const float LeftMargin = 40f;
    private const float RightMargin = 40f;
    private const float TimelineWidth = Width - LeftMargin - RightMargin;
    private const float PixelsPerDegree = 15f;

    public static byte[] Render(List<HourlyForecast> forecasts)
    {
        var now = DateTime.Now;

        using var surface = SKSurface.Create(new SKImageInfo(Width, Height));
        var canvas = surface.Canvas;
        canvas.Clear(SKColors.White);

        DrawHeader(canvas, forecasts, now);
        DrawTimeline(canvas);
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
        using var dateFont = new SKFont(SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold), 32);
        using var dateSmallFont = new SKFont(SKTypeface.FromFamilyName("Arial"), 28);

        WeatherIconRenderer.Draw(canvas, current.Condition, 80, 90, 50);

        canvas.DrawText($"{current.Temperature:+0;-0;0}°C", 160, 100, SKTextAlign.Left, bigFont, paint);
        canvas.DrawText($"{current.WindSpeed:0} m/s", 160, 135, SKTextAlign.Left, medFont, paint);
        canvas.DrawText(SwedishDateHelper.GetConditionText(current.Condition), 160, 165, SKTextAlign.Left, medFont, paint);

        var dayName = SwedishDateHelper.GetDayName(now.DayOfWeek);
        var monthName = SwedishDateHelper.GetMonthName(now.Month);
        canvas.DrawText(dayName, Width - 50, 55, SKTextAlign.Right, dateFont, paint);
        canvas.DrawText($"{now.Day} {monthName}", Width - 50, 90, SKTextAlign.Right, dateSmallFont, paint);
        canvas.DrawText($"{now.Year}", Width - 50, 125, SKTextAlign.Right, dateSmallFont, paint);
    }

    private static void DrawTimeline(SKCanvas canvas)
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
        using var hourFont = new SKFont(SKTypeface.FromFamilyName("Arial"), 16);

        canvas.DrawLine(LeftMargin, TimelineY, Width - RightMargin, TimelineY, linePaint);

        for (int h = 0; h <= 24; h += 2)
        {
            float x = HourToX(h);
            canvas.DrawLine(x, TimelineY - 8, x, TimelineY + 8, tickPaint);
            canvas.DrawText(h.ToString(), x, TimelineY + 25, SKTextAlign.Center, hourFont, textPaint);
        }
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

        foreach (var f in forecasts.Where(f => f.Hour % 2 == 0))
        {
            float x = HourToX(f.Hour);
            float y = TempToY(f.Temperature);

            bool aboveLine = f.Temperature >= 0;
            float iconY = aboveLine ? y - 75 : y + 50;
            WeatherIconRenderer.Draw(canvas, f.Condition, x, iconY, 24);

            float tempLabelY = aboveLine ? y - 30 : y + 100;
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

    private static float HourToX(float hour) => LeftMargin + (hour / 24f) * TimelineWidth;

    private static float TempToY(float temp) => TimelineY - (temp * PixelsPerDegree);
}
