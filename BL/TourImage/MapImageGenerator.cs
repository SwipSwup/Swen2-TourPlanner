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

    public async Task<string?> GenerateMapImageWithLeaflet(RouteResult route)
{
    string templatePath = Path.Combine(AppContext.BaseDirectory, "BL/External/Assets/MapTemplate.html");
    Console.WriteLine(templatePath);
    if (!File.Exists(templatePath))
    {
        Console.WriteLine($"Template file not found at: {templatePath}");
        return null;
    }

    string template = await File.ReadAllTextAsync(templatePath);

    string html = template
        .Replace("{{startLat}}", route.StartLatitude.ToString(CultureInfo.InvariantCulture))
        .Replace("{{startLon}}", route.StartLongitude.ToString(CultureInfo.InvariantCulture))
        .Replace("{{endLat}}", route.EndLatitude.ToString(CultureInfo.InvariantCulture))
        .Replace("{{endLon}}", route.EndLongitude.ToString(CultureInfo.InvariantCulture));

    string tempHtmlPath = Path.Combine(Path.GetTempPath(), $"map_{Guid.NewGuid()}.html");
    await File.WriteAllTextAsync(tempHtmlPath, html);

    try
    {
        if (string.IsNullOrWhiteSpace(OutputPath))
        {
            Console.WriteLine("OutputPath is not set.");
            return null;
        }

        Directory.CreateDirectory(OutputPath); // Ensure the output directory exists

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
        //Console.WriteLine($"Opening map file: {fileUri}");

        await page.GoToAsync(fileUri, WaitUntilNavigation.Networkidle2);

        // Wait for leaflet to finish rendering (set this in your JS with: window.status = 'ready')
        try
        {
            await page.WaitForFunctionAsync("() => window.status === 'ready'", new WaitForFunctionOptions
            {
                Timeout = 5000
            });
        }
        catch (PuppeteerException ex)
        {
            Console.WriteLine("Warning: 'window.status = ready' not reached within timeout. Proceeding anyway.");
            Console.WriteLine($"PuppeteerException: {ex.Message}");
        }

        await page.ScreenshotAsync(outputImagePath, new ScreenshotOptions { FullPage = true });

        return outputImagePath;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception in GenerateMapImageWithLeaflet: {ex.Message}");
        Console.WriteLine(ex.StackTrace);
        return null;
    }
    finally
    {
        try
        {
            if (File.Exists(tempHtmlPath))
                File.Delete(tempHtmlPath);
        }
        catch (Exception cleanupEx)
        {
            Console.WriteLine($"Error deleting temp HTML: {cleanupEx.Message}");
        }
    }
}

}