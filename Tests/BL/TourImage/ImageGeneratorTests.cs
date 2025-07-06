using BL.TourImage;
using Microsoft.Extensions.Configuration;

namespace Tests.BL.TourImage;

[TestFixture]
public class MapImageGeneratorTests
{
    private string _outputPath;
    private MapImageGenerator _generator;

    [SetUp]
    public void SetUp()
    {
        
        _outputPath = Path.Combine(Path.GetTempPath(), "MapImageTest", Guid.NewGuid().ToString());
        Directory.CreateDirectory(_outputPath);

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "TourImages:OutputDirectory", _outputPath }
            })
            .Build();

        _generator = new MapImageGenerator(config);

        // Ensure the template exists
        string assetDir = Path.Combine(AppContext.BaseDirectory, "BL/External/Assets");
        Directory.CreateDirectory(assetDir);

        string templatePath = Path.Combine(assetDir, "MapTemplate.html");
        Console.WriteLine(templatePath);
        if (!File.Exists(templatePath))
        {
            File.WriteAllText(templatePath, @"
<!DOCTYPE html>
<html>
<head>
  <meta charset='utf-8' />
  <title>Map</title>
  <link 
    rel='stylesheet' 
    href='https://unpkg.com/leaflet@1.9.4/dist/leaflet.css' />
  <script 
    src='https://unpkg.com/leaflet@1.9.4/dist/leaflet.js'></script>
</head>
<body>
  <div id='map' style='width:800px;height:600px;'></div>
  <script>
    var map = L.map('map').setView([{{startLat}}, {{startLon}}], 8);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png').addTo(map);
    L.marker([{{startLat}}, {{startLon}}]).addTo(map);
    L.marker([{{endLat}}, {{endLon}}]).addTo(map);
    setTimeout(() => { window.status = 'ready'; }, 1000);
  </script>
</body>
</html>");
        }
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_outputPath))
            Directory.Delete(_outputPath, true);
    }

    [Test]
    public async Task GenerateMapImageWithLeaflet_ValidRoute_ShouldCreatePngFile()
    {
        // Arrange
        var route = new RouteResult
        {
            StartLatitude = 48.2082,
            StartLongitude = 16.3738,
            EndLatitude = 47.0707,
            EndLongitude = 15.4395
        };

        // Act
        string? imagePath = await _generator.GenerateMapImageWithLeaflet(route);

        // Assert
        Assert.That(imagePath, Is.Not.Null);
        Console.WriteLine($"[Test] Received image path: {imagePath}");

        Assert.That(File.Exists(imagePath),
            Is.True);
    }
}