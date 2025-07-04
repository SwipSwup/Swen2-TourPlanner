using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;

namespace BL.External;

public class RouteService : IRouteService
{
    private readonly HttpClient _httpClient;
    private const string ApiKey = "5b3ce3597851110001cf6248d1538c5cac7e47e09f411484b475dd46"; 
    private const string ApiEndpoint = "https://api.openrouteservice.org/v2/directions/driving-car";

    public RouteService(HttpClient? client = null)
    {
        _httpClient = client ?? new HttpClient();
    }

    public async Task<RouteResult> GetRouteAsync(string from, string to)
    {
        Coordinate? startCoords = await GeocodeAsync(from);
        Coordinate? endCoords = await GeocodeAsync(to);

        if (startCoords == null || endCoords == null)
            throw new Exception("Failed to resolve coordinates.");

        var request = new
        {
            coordinates = new[]
            {
                new[] { startCoords.Longitude, startCoords.Latitude },
                new[] { endCoords.Longitude, endCoords.Latitude }
            }
        };

        HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, ApiEndpoint)
        {
            Content = JsonContent.Create(request)
        };
        requestMessage.Headers.Add("Authorization", ApiKey);

        HttpResponseMessage response = await _httpClient.SendAsync(requestMessage);
        response.EnsureSuccessStatusCode();
        string json = await response.Content.ReadAsStringAsync();

        using JsonDocument doc = JsonDocument.Parse(json);

        JsonElement summary = doc.RootElement.GetProperty("routes")[0]
            .GetProperty("summary");

        float distance = (float)summary.GetProperty("distance").GetDouble() / 1000;
        TimeSpan duration = TimeSpan.FromSeconds(summary.GetProperty("duration").GetDouble());

        return new RouteResult
        {
            Distance = distance,
            EstimatedTime = duration,
            StartLatitude = startCoords.Latitude,
            StartLongitude = startCoords.Longitude,
            EndLatitude = endCoords.Latitude,
            EndLongitude = endCoords.Longitude
        };
    }


    private async Task<Coordinate?> GeocodeAsync(string location)
    {
        HttpResponseMessage response = await _httpClient.GetAsync(
            $"https://api.openrouteservice.org/geocode/search?api_key={ApiKey}&text={Uri.EscapeDataString(location)}"
        );
        
        if (!response.IsSuccessStatusCode) 
            return null;

        string json = await response.Content.ReadAsStringAsync();
        
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement coords = doc.RootElement.GetProperty("features")[0].GetProperty("geometry")
            .GetProperty("coordinates");
        
        return new Coordinate(coords[1].GetDouble(), coords[0].GetDouble());
    }

    private record Coordinate(double Latitude, double Longitude);
}