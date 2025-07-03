using System.Globalization;
using PuppeteerSharp;

namespace BL.TourImage;

public class MapImageGenerator : IMapImageGenerator
{
    private readonly string _outputPath = config["ReportSettings:OutputDirectory"]
                                          ?? throw new Exception("Missing ReportSettings:OutputDirectory");
    
    public async Task<string> GenerateMapImageWithLeaflet(RouteResult route, string outputPath)
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
            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();

            await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                Args = new[] { "--no-sandbox", "--disable-setuid-sandbox" }
            });

            await using var page = await browser.NewPageAsync();

            var fileUri = new Uri(tempHtmlPath).AbsoluteUri;
            await page.GoToAsync(fileUri);

            await page.WaitForFunctionAsync("() => window.status === 'ready'", new WaitForFunctionOptions
            {
                Timeout = 5000
            });

            await page.ScreenshotAsync(outputPath, new ScreenshotOptions { FullPage = true });

            return outputPath;
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