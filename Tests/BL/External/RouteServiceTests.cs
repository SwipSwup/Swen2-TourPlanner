using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BL.External;
using NUnit.Framework;

namespace Tests.BL.External
{
    [TestFixture]
    public class RouteServiceTests
    {
        private HttpClient _mockHttpClient = null!;
        private MockHttpMessageHandler _mockHandler = null!;

        [SetUp]
        public void SetUp()
        {
            _mockHandler = new MockHttpMessageHandler();
            _mockHttpClient = new HttpClient(_mockHandler);
        }

        [TearDown]
        public void TearDown()
        {
            _mockHttpClient.Dispose();
            _mockHandler.Dispose();
        }

        [Test]
        public async Task GetRouteAsync_ValidLocations_ReturnsValidResult()
        {
            var service = new RouteService(_mockHttpClient);

            _mockHandler.SetupGeocode("Vienna", 48.2082, 16.3738);
            _mockHandler.SetupGeocode("Graz", 47.0707, 15.4395);
            _mockHandler.SetupRoute(200000, 7200); // 200 km, 2 hours

            RouteResult result = await service.GetRouteAsync("Vienna", "Graz");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Distance, Is.EqualTo(200f));
            Assert.That(result.EstimatedTime, Is.EqualTo(TimeSpan.FromHours(2)));
            Assert.That(result.StartLatitude, Is.EqualTo(48.2082));
            Assert.That(result.StartLongitude, Is.EqualTo(16.3738));
            Assert.That(result.EndLatitude, Is.EqualTo(47.0707));
            Assert.That(result.EndLongitude, Is.EqualTo(15.4395));
        }

        [Test]
        public void GetRouteAsync_GeocodeStartFails_ThrowsException()
        {
            var service = new RouteService(_mockHttpClient);

            // Setup geocode fail for start location (no response or bad response)
            _mockHandler.SetupFailGeocode();

            // End location geocode would not be called but just in case:
            _mockHandler.SetupGeocode("Graz", 47.0707, 15.4395);

            Assert.ThrowsAsync<Exception>(async () => await service.GetRouteAsync("InvalidStart", "Graz"));
        }

        [Test]
        public void GetRouteAsync_GeocodeEndFails_ThrowsException()
        {
            var service = new RouteService(_mockHttpClient);

            _mockHandler.SetupGeocode("Vienna", 48.2082, 16.3738);

            // Setup fail geocode for end location
            _mockHandler.SetupFailGeocode();

            Assert.ThrowsAsync<Exception>(async () => await service.GetRouteAsync("Vienna", "InvalidEnd"));
        }

        [Test]
        public async Task GetRouteAsync_SameStartAndEndLocation_ReturnsZeroDistanceAndTime()
        {
            var service = new RouteService(_mockHttpClient);

            // Setup geocode for the same location twice
            _mockHandler.SetupGeocode("Vienna", 48.2082, 16.3738);
            _mockHandler.SetupGeocode("Vienna", 48.2082, 16.3738);

            // Setup route with zero distance and zero duration
            _mockHandler.SetupRoute(0, 0);

            RouteResult result = await service.GetRouteAsync("Vienna", "Vienna");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Distance, Is.EqualTo(0f));
            Assert.That(result.EstimatedTime, Is.EqualTo(TimeSpan.Zero));
            Assert.That(result.StartLatitude, Is.EqualTo(48.2082));
            Assert.That(result.StartLongitude, Is.EqualTo(16.3738));
            Assert.That(result.EndLatitude, Is.EqualTo(48.2082));
            Assert.That(result.EndLongitude, Is.EqualTo(16.3738));
        }

        [Test]
        public void GetRouteAsync_NullOrEmptyLocations_ThrowsException()
        {
            var service = new RouteService(_mockHttpClient);

            Assert.ThrowsAsync<Exception>(async () => await service.GetRouteAsync("", ""));
            Assert.ThrowsAsync<Exception>(async () => await service.GetRouteAsync(null!, null!));
        }



        [Test]
        public void GetRouteAsync_RouteRequestFails_ThrowsException()
        {
            var service = new RouteService(_mockHttpClient);

            _mockHandler.SetupGeocode("Vienna", 48.2082, 16.3738);
            _mockHandler.SetupGeocode("Graz", 47.0707, 15.4395);

            // Setup failure response for routing request
            _mockHandler.SetupRouteFail();

            Assert.ThrowsAsync<HttpRequestException>(async () => await service.GetRouteAsync("Vienna", "Graz"));
        }
    }

    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly Queue<HttpResponseMessage> _responses = new();

        public void SetupGeocode(string location, double lat, double lon)
        {
            string json = $@"
            {{
                ""features"": [
                    {{
                        ""geometry"": {{
                            ""coordinates"": [ {lon.ToString(CultureInfo.InvariantCulture)}, {lat.ToString(CultureInfo.InvariantCulture)} ]
                        }}
                    }}
                ]
            }}";
            _responses.Enqueue(CreateResponse(json));
        }

        public void SetupFailGeocode()
        {
            // Simulate failed geocode with 404 NotFound response
            _responses.Enqueue(new HttpResponseMessage(HttpStatusCode.NotFound));
        }

        public void SetupRoute(double distanceMeters, double durationSeconds)
        {
            string json = $@"
            {{
                ""routes"": [
                    {{
                        ""summary"": {{
                            ""distance"": {distanceMeters.ToString(CultureInfo.InvariantCulture)},
                            ""duration"": {durationSeconds.ToString(CultureInfo.InvariantCulture)}
                        }}
                    }}
                ]
            }}";
            _responses.Enqueue(CreateResponse(json));
        }

        public void SetupRouteFail()
        {
            // Simulate routing API failure
            _responses.Enqueue(new HttpResponseMessage(HttpStatusCode.InternalServerError));
        }

        private HttpResponseMessage CreateResponse(string content)
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            };
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (_responses.Count == 0)
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
            return Task.FromResult(_responses.Dequeue());
        }
    }
}
