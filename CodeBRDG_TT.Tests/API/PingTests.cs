using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CodeBRDG_TT.API;

public class PingTests : IClassFixture<WebApplicationFactory<Program>> // Replace `Program` with your Startup class if necessary
{
    private readonly HttpClient _client;

    public PingTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Ping_ShouldReturnOkResponse()
    {
        // Act
        var response = await _client.GetAsync("/ping");

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        var responseBody = await response.Content.ReadAsStringAsync();
        Assert.Equal("Dogshouseservice.Version1.0.1", responseBody);
    }

}
