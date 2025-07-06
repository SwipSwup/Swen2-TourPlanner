using System.Globalization;
using System.Net;
using System.Net.Mime;
using System.Text;
using BL.External;
using BL.TourImage;
using Microsoft.Extensions.Configuration;

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
            RouteService service = new RouteService(_mockHttpClient);

            _mockHandler.SetupGeocode("Vienna", 48.2082, 16.3738);
            _mockHandler.SetupGeocode("Graz", 47.0707, 15.4395);
            _mockHandler.SetupRoute(200000, 7200); // 200km, 2h

            RouteResult result = await service.GetRouteAsync("Vienna", "Graz");

            
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Distance, Is.EqualTo(200f));
            Assert.That(result.EstimatedTime, Is.EqualTo(TimeSpan.FromHours(2)));
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