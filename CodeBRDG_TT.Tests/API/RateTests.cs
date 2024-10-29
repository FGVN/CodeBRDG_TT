using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace CodeBRDG_TT.API
{
    public class RateTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly int _permitLimit;
        private readonly int _windowInSeconds;

        public RateTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();

            // Get configuration values from the application
            var scope = factory.Services.CreateScope();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            _permitLimit = configuration.GetValue<int>("RateLimiting:PermitLimit");
            _windowInSeconds = configuration.GetValue<int>("RateLimiting:WindowInSeconds");
        }

        [Fact]
        public async Task RateLimiting_ShouldReturnTooManyRequests()
        {
            // Arrange
            var successfulResponses = 0;
            var failedResponses = 0;
            var tasks = new Task<HttpResponseMessage>[20]; // Total requests

            for (int i = 0; i < 20; i++)
            {
                // The number of successful requests should be equal to the permit limit
                if (i < _permitLimit) 
                {
                    tasks[i] = _client.GetAsync("/ping");
                }
                else 
                {
                    tasks[i] = _client.GetAsync("/ping");
                }
            }

            // Act
            var responses = await Task.WhenAll(tasks);

            // Assert
            foreach (var response in responses)
            {
                if (response.IsSuccessStatusCode)
                {
                    successfulResponses++;
                }
                else if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    failedResponses++;
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    Assert.Contains($"error\": \"Too many requests. (More than {_permitLimit} per {_windowInSeconds} seconds)", errorResponse);
                }
            }

            Assert.Equal(_permitLimit, successfulResponses); // Only permitLimit should be successful
            Assert.Equal(20 - _permitLimit, failedResponses); // The rest should be rate limited
        }
    }
}
