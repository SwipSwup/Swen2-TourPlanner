using System.Globalization;
using Microsoft.Extensions.Configuration;
using PuppeteerSharp;

namespace BL.TourImage;

public class MapImageGenerator(IConfiguration config) : IMapImageGenerator
{
    private const string Template =
        "<!DOCTYPE html>\n<html>\n<head>\n    <title>Leaflet Map with Driving Route</title>\n    <meta charset=\"utf-8\" />\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\n    <link rel=\"stylesheet\" href=\"https://unpkg.com/leaflet/dist/leaflet.css\" />\n    <style>\n        #map { height: 100vh; width: 100%; }\n    </style>\n</head>\n<body>\n\n<div id=\"map\"></div>\n\n<!-- Leaflet JS -->\n<script src=\"https://unpkg.com/leaflet/dist/leaflet.js\"></script>\n<script>\n    const startLat = {{startLat}}; \n    const startLon = {{startLon}};\n    const endLat = {{endLat}};     \n    const endLon = {{endLon}};     \n    const map = L.map('map').setView([(startLat + endLat) / 2, (startLon + endLon) / 2], 5);\n\n    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {\n        attribution: '&copy; OpenStreetMap contributors'\n    }).addTo(map);\n\n    L.marker([startLat, startLon]).addTo(map);\n    L.marker([endLat, endLon]).addTo(map);\n\n    const url = `https://router.project-osrm.org/route/v1/driving/${startLon},${startLat};${endLon},${endLat}?overview=full&geometries=geojson`;\n\n    fetch(url)\n        .then(response => response.json())\n        .then(data => {\n            if (!data.routes || data.routes.length === 0) {\n                throw new Error(\"No route found.\");\n            }\n\n            const route = data.routes[0].geometry;\n\n            const routeLine = L.geoJSON(route, {\n                style: {\n                    color: 'blue',\n                    weight: 4,\n                    opacity: 0.8\n                }\n            }).addTo(map);\n\n            map.fitBounds(routeLine.getBounds(), { padding: [50, 50] });\n\n            window.status = \"ready\"\n        })\n        .catch(error => {\n            console.error('OSRM routing failed:', error);\n\n            // Fall back to straight line\n            const fallbackLine = L.polyline([\n                [startLat, startLon],\n                [endLat, endLon]\n            ], {\n                color: 'red',\n                weight: 2,\n                opacity: 0.5,\n                dashArray: '5, 5'\n            }).addTo(map);\n\n            map.fitBounds(fallbackLine.getBounds(), { padding: [50, 50] });\n\n            window.status = \"ready\"\n        });\n</script>\n\n</body>\n</html>\n";

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
        string html = Template
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

            Directory.CreateDirectory(OutputPath); 

            string imageFileName = $"map_{Guid.NewGuid()}.png";
            string outputImagePath = Path.Combine(OutputPath, imageFileName);

            BrowserFetcher browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();

            await using IBrowser? browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                Args = ["--no-sandbox", "--disable-setuid-sandbox"]
            });

            await using IPage? page = await browser.NewPageAsync();

            string fileUri = new Uri(tempHtmlPath).AbsoluteUri;

            await page.GoToAsync(fileUri, WaitUntilNavigation.Networkidle2);

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