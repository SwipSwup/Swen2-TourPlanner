using System;
using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using log4net;
using System.Reflection;

namespace BL.External;

public class RouteService(HttpClient? client = null) : IRouteService
{
    private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    private readonly HttpClient _httpClient = client ?? new HttpClient();
    private const string ApiKey = "5b3ce3597851110001cf6248d1538c5cac7e47e09f411484b475dd46";
    private const string ApiEndpoint = "https://api.openrouteservice.org/v2/directions/driving-car";

    public async Task<RouteResult> GetRouteAsync(string from, string to)
    {
        try
        {
            log.Info($"Getting route from '{from}' to '{to}'.");

            Coordinate? startCoords = await GeocodeAsync(from);
            if (startCoords == null)
            {
                log.Error($"Failed to geocode start location: {from}");
                throw new Exception($"Failed to geocode start location: {from}");
            }

            Coordinate? endCoords = await GeocodeAsync(to);
            if (endCoords == null)
            {
                log.Error($"Failed to geocode end location: {to}");
                throw new Exception($"Failed to geocode end location: {to}");
            }

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

            log.Info("Sending routing request to OpenRouteService API.");
            HttpResponseMessage response = await _httpClient.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();

            using JsonDocument doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            JsonElement summary = doc.RootElement.GetProperty("routes")[0].GetProperty("summary");

            log.Info("Routing request successful.");

            return new RouteResult
            {
                Distance = (float)summary.GetProperty("distance").GetDouble() / 1000,
                EstimatedTime = TimeSpan.FromSeconds(summary.GetProperty("duration").GetDouble()),
                StartLatitude = startCoords.Latitude,
                StartLongitude = startCoords.Longitude,
                EndLatitude = endCoords.Latitude,
                EndLongitude = endCoords.Longitude
            };
        }
        catch (Exception ex)
        {
            log.Error("Error while getting route.", ex);
            throw;
        }
    }

    private async Task<Coordinate?> GeocodeAsync(string location)
    {
        try
        {
            log.Info($"Geocoding location: {location}");

            HttpResponseMessage response = await _httpClient.GetAsync(
                $"https://api.openrouteservice.org/geocode/search?api_key={ApiKey}&text={Uri.EscapeDataString(location)}"
            );

            if (!response.IsSuccessStatusCode)
            {
                log.Warn($"Geocoding failed for '{location}' with status code {response.StatusCode}");
                return null;
            }

            using JsonDocument doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            JsonElement coords = doc.RootElement.GetProperty("features")[0].GetProperty("geometry").GetProperty("coordinates");

            var coordinate = new Coordinate(coords[1].GetDouble(), coords[0].GetDouble());
            log.Info($"Geocoding success for '{location}': {coordinate.Latitude}, {coordinate.Longitude}");

            return coordinate;
        }
        catch (Exception ex)
        {
            log.Error($"Exception during geocoding '{location}'.", ex);
            return null;
        }
    }

    private record Coordinate(double Latitude, double Longitude);
}
