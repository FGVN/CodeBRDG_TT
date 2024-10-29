using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using CodeBRDG_TT.Commands;
using CodeBRDG_TT.Controllers;
using CodeBRDG_TT.Queries;
using MediatR;
using CodeBRDG_TT.Models;

namespace CodeBRDG_TT.Tests.Controllers;

public class DogControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly DogController _controller;

    public DogControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new DogController(_mediatorMock.Object);
    }

    [Fact]
    public async Task QueryDogs_ValidQuery_ReturnsOkResult()
    {
        // Arrange
        var query = new DogsQuery(); 
        var expectedDogs = new List<Dog>();
        _mediatorMock.Setup(m => m.Send(query, It.IsAny<CancellationToken>())).ReturnsAsync(expectedDogs);

        // Act
        var result = await _controller.QueryDogs(query);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedDogs, okResult.Value);
    }

    [Fact]
    public async Task RegisterDog_ValidCommand_ReturnsCreatedResult()
    {
        // Arrange
        var command = new RegisterDogCommand { name = "Buddy" }; 
        _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        var result = await _controller.RegisterDog(command);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(_controller.QueryDogs), createdResult.ActionName);
        Assert.Equal(command.name, createdResult.RouteValues["name"]);
    }

    [Fact]
    public async Task RegisterDog_InvalidCommand_ReturnsBadRequest()
    {
        // Arrange
        var command = new RegisterDogCommand { name = "Buddy" };
        _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act
        var result = await _controller.RegisterDog(command);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Failed to create dog.", badRequestResult.Value);
    }
}
