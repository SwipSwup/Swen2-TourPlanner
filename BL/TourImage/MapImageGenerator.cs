using System.Globalization;
using Microsoft.Extensions.Configuration;
using PuppeteerSharp;

namespace BL.TourImage;

public class MapImageGenerator(IConfiguration config) : IMapImageGenerator
{
    private string OutputPath
    {
        get
        {
            string path = Path.Combine(AppContext.BaseDirectory,
                config["TourImages:OutputDirectory"] ?? throw new Exception("Missing ReportSettings:OutputDirectory"));
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }
    }

    public async Task<string> GenerateMapImageWithLeaflet(RouteResult route)
    {
        string template = await File.ReadAllTextAsync("Assets/MapTemplate.html");

        string html = template
            .Replace("{{startLat}}", route.StartLatitude.ToString(CultureInfo.InvariantCulture))
            .Replace("{{startLon}}", route.StartLongitude.ToString(CultureInfo.InvariantCulture))
            .Replace("{{endLat}}", route.EndLatitude.ToString(CultureInfo.InvariantCulture))
            .Replace("{{endLon}}", route.EndLongitude.ToString(CultureInfo.InvariantCulture));

        string tempHtmlPath = Path.Combine(Path.GetTempPath(), $"map_{Guid.NewGuid()}.html");
        await File.WriteAllTextAsync(tempHtmlPath, html);

        try
        {
            string imageFileName = $"map_{Guid.NewGuid()}.png";
            string outputImagePath = Path.Combine(OutputPath, imageFileName);

            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();

            await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                Args = ["--no-sandbox", "--disable-setuid-sandbox"]
            });

            await using var page = await browser.NewPageAsync();

            string fileUri = new Uri(tempHtmlPath).AbsoluteUri;
            await page.GoToAsync(fileUri);

            await page.WaitForFunctionAsync("() => window.status === 'ready'", new WaitForFunctionOptions
            {
                Timeout = 5000
            });

            await page.ScreenshotAsync(outputImagePath, new ScreenshotOptions { FullPage = true });

            return outputImagePath;
        }
        finally
        {
            if (File.Exists(tempHtmlPath))
            {
                File.Delete(tempHtmlPath);
            }
        }
    }
}