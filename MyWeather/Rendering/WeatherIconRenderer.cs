using System.Reflection;
using SkiaSharp;
using Svg.Skia;

namespace MyWeather.Rendering;

public static class WeatherIconRenderer
{
    private static readonly Assembly Assembly = typeof(WeatherIconRenderer).Assembly;
    private static readonly Dictionary<string, SKPicture?> Cache = new();

    public static void Draw(SKCanvas canvas, string symbolCode, float cx, float cy, float size)
    {
        var picture = GetIcon(symbolCode);
        if (picture == null) return;

        var bounds = picture.CullRect;
        float scale = size * 2 / Math.Max(bounds.Width, bounds.Height);

        canvas.Save();
        canvas.Translate(cx - bounds.Width * scale / 2, cy - bounds.Height * scale / 2);
        canvas.Scale(scale);
        canvas.DrawPicture(picture);
        canvas.Restore();
    }

    private static SKPicture? GetIcon(string symbolCode)
    {
        if (Cache.TryGetValue(symbolCode, out var cached))
            return cached;

        var picture = LoadIcon(symbolCode);
        Cache[symbolCode] = picture;
        return picture;
    }

    private static SKPicture? LoadIcon(string symbolCode)
    {
        var stream = Assembly.GetManifestResourceStream($"MyWeather.Resources.WeatherIcons.{symbolCode}.svg");
        if (stream == null) return null;

        var svg = new SKSvg();
        svg.Load(stream);
        return svg.Picture;
    }
}
