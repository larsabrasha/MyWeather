using SkiaSharp;
using TrmlWeather.Models;

namespace TrmlWeather.Rendering;

public static class WeatherIconRenderer
{
    public static void Draw(SKCanvas canvas, WeatherCondition condition, float cx, float cy, float size)
    {
        using var paint = new SKPaint
        {
            Color = SKColors.Black, IsAntialias = true,
            StrokeWidth = Math.Max(1.5f, size / 8f), Style = SKPaintStyle.Stroke
        };

        switch (condition)
        {
            case WeatherCondition.Clear:
                DrawSun(canvas, cx, cy, size, paint);
                break;
            case WeatherCondition.PartlyCloudy:
                DrawSun(canvas, cx - size * 0.3f, cy - size * 0.3f, size * 0.6f, paint);
                DrawCloud(canvas, cx + size * 0.2f, cy + size * 0.2f, size * 0.7f, paint);
                break;
            case WeatherCondition.Cloudy:
                DrawCloud(canvas, cx, cy, size, paint);
                break;
            case WeatherCondition.Rain:
                DrawCloud(canvas, cx, cy - size * 0.3f, size * 0.8f, paint);
                DrawRainDrops(canvas, cx, cy + size * 0.4f, size, paint);
                break;
            case WeatherCondition.Snow:
                DrawCloud(canvas, cx, cy - size * 0.3f, size * 0.8f, paint);
                DrawSnowflakes(canvas, cx, cy + size * 0.4f, size);
                break;
            case WeatherCondition.Fog:
                DrawFogLines(canvas, cx, cy, size, paint);
                break;
        }
    }

    private static void DrawSun(SKCanvas canvas, float cx, float cy, float size, SKPaint paint)
    {
        canvas.DrawCircle(cx, cy, size * 0.4f, paint);
        for (int i = 0; i < 8; i++)
        {
            float angle = i * MathF.PI * 2 / 8;
            float innerR = size * 0.55f;
            float outerR = size * 0.8f;
            canvas.DrawLine(
                cx + MathF.Cos(angle) * innerR, cy + MathF.Sin(angle) * innerR,
                cx + MathF.Cos(angle) * outerR, cy + MathF.Sin(angle) * outerR,
                paint);
        }
    }

    private static void DrawCloud(SKCanvas canvas, float cx, float cy, float size, SKPaint paint)
    {
        using var fillPaint = new SKPaint { Color = SKColors.White, IsAntialias = true, Style = SKPaintStyle.Fill };

        float r = size * 0.35f;
        var path = new SKPath();
        path.AddCircle(cx - size * 0.25f, cy, r);
        path.AddCircle(cx + size * 0.15f, cy - size * 0.1f, r * 1.2f);
        path.AddCircle(cx + size * 0.4f, cy, r * 0.9f);
        path.AddRect(new SKRect(cx - size * 0.25f - r, cy, cx + size * 0.4f + r * 0.9f, cy + r));

        var simplified = new SKPath();
        if (path.Simplify(simplified))
        {
            canvas.DrawPath(simplified, fillPaint);
            canvas.DrawPath(simplified, paint);
        }
        simplified.Dispose();
    }

    private static void DrawRainDrops(SKCanvas canvas, float cx, float cy, float size, SKPaint paint)
    {
        float spacing = size * 0.35f;
        for (int i = -1; i <= 1; i++)
        {
            float x = cx + i * spacing;
            canvas.DrawLine(x, cy, x - size * 0.1f, cy + size * 0.3f, paint);
        }
    }

    private static void DrawSnowflakes(SKCanvas canvas, float cx, float cy, float size)
    {
        float spacing = size * 0.35f;
        float r = size * 0.08f;
        using var dotPaint = new SKPaint { Color = SKColors.Black, IsAntialias = true, Style = SKPaintStyle.Fill };
        for (int i = -1; i <= 1; i++)
        {
            canvas.DrawCircle(cx + i * spacing, cy, r, dotPaint);
            canvas.DrawCircle(cx + i * spacing + spacing * 0.5f, cy + size * 0.25f, r, dotPaint);
        }
    }

    private static void DrawFogLines(SKCanvas canvas, float cx, float cy, float size, SKPaint paint)
    {
        for (int i = -1; i <= 1; i++)
        {
            float y = cy + i * size * 0.3f;
            canvas.DrawLine(cx - size * 0.6f, y, cx + size * 0.6f, y, paint);
        }
    }
}
