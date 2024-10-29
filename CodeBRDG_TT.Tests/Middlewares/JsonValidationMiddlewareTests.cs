using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CodeBRDG_TT.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace CodeBRDG_TT.Tests.Middlewares;

public class JsonValidationMiddlewareTests
{
    private readonly Mock<RequestDelegate> _nextMock;
    private readonly JsonValidationMiddleware _middleware;

    public JsonValidationMiddlewareTests()
    {
        _nextMock = new Mock<RequestDelegate>();
        _middleware = new JsonValidationMiddleware(_nextMock.Object);
    }

    [Fact]
    public async Task InvokeAsync_ValidJson_ShouldCallNextMiddleware()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.ContentType = "application/json";
        var validJson = "{\"name\":\"Buddy\", \"age\":5}";
        context.Request.Body = GenerateStreamFromString(validJson);
        context.Request.EnableBuffering(); 

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        _nextMock.Verify(m => m.Invoke(context), Times.Once);
        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_InvalidJson_ShouldReturnBadRequest()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.ContentType = "application/json";
        var invalidJson = "{\"name\":\"Buddy\", \"tail_length\":null}"; 
        context.Request.Body = GenerateStreamFromString(invalidJson);
        context.Request.EnableBuffering(); 

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_ValidJson_ShouldProceed()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.ContentType = "application/json";
        var validJson = "{\"name\":\"Buddy\", \"tail_length\":5, \"weight\":20}";
        context.Request.Body = GenerateStreamFromString(validJson);
        context.Request.EnableBuffering();

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
    }


    [Fact]
    public async Task InvokeAsync_NullValues_ShouldReturnBadRequest()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.ContentType = "application/json";

        var invalidJson = "{\"name\":\"Buddy\", \"tail_length\":null}";
        context.Request.Body = GenerateStreamFromString(invalidJson);
        context.Request.EnableBuffering(); 

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
    }


    [Fact]
    public async Task InvokeAsync_DecimalValue_ShouldReturnBadRequest()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.ContentType = "application/json";
        var invalidJson = "{\"name\":\"Buddy\", \"tail_length\":5.5}";
        context.Request.Body = GenerateStreamFromString(invalidJson);
        context.Request.EnableBuffering();

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
    }


    private static Stream GenerateStreamFromString(string value)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(value);
        writer.Flush();
        stream.Position = 0; // Reset the stream position to the beginning
        return stream;
    }

    private class ErrorResponse
    {
        public string error { get; set; }
        public string message { get; set; }
        public int lineNumber { get; set; }
        public int bytePositionInLine { get; set; }
    }
}
