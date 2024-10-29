using CodeBRDG_TT.Commands;
using CodeBRDG_TT.Commands.Handlers;
using CodeBRDG_TT.Data.Repositories;
using CodeBRDG_TT.Data.UnitOfWork;
using CodeBRDG_TT.Models;
using FluentAssertions;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CodeBRDG_TT.Tests.Commands;

public class RegisterDogCommandHandlerTests
{
    private readonly RegisterDogCommandHandler _handler;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IRepository<Dog>> _mockDogRepository;

    public RegisterDogCommandHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockDogRepository = new Mock<IRepository<Dog>>();

        // Setup the UnitOfWork to return the mock repository
        _mockUnitOfWork.Setup(u => u.Dogs).Returns(_mockDogRepository.Object);

        _handler = new RegisterDogCommandHandler(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldRegisterDog()
    {
        // Arrange
        var command = new RegisterDogCommand
        {
            name = "Buddy",
            color = "brown",
            tail_length = 6,
            weight = 30
        };

        Dog addedDog = null;

        // Set up the mock to capture the dog being added
        _mockDogRepository.Setup(d => d.AddAsync(It.IsAny<Dog>()))
            .Callback<Dog>(dog => addedDog = dog);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        _mockDogRepository.Verify(d => d.AddAsync(It.IsAny<Dog>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);

        // Assert that the added dog matches the command
        addedDog.Should().NotBeNull();
        addedDog.name.Should().Be(command.name);
        addedDog.color.Should().Be(command.color);
        addedDog.tail_length.Should().Be(command.tail_length);
        addedDog.weight.Should().Be(command.weight);
    }
    [Fact]
    public async Task Handle_TailLengthNegative_ShouldThrowException()
    {
        // Arrange
        var command = new RegisterDogCommand
        {
            name = "Max",
            color = "golden",
            tail_length = -5, // Invalid tail length
            weight = 20
        };

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
            .WithMessage("Tail length must be greater than or equal to zero.*")
            .WithParameterName("tail_length"); // Ensure the exception references the correct parameter name

        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never); // Ensure SaveChangesAsync is not called
    }

    [Fact]
    public async Task Handle_CommandThrowsException_ShouldReturnFalse()
    {
        // Arrange
        var command = new RegisterDogCommand
        {
            name = "Rex",
            color = "white & black",
            tail_length = 5,
            weight = 20
        };

        // Set up mock to throw an exception
        _mockDogRepository.Setup(u => u.AddAsync(It.IsAny<Dog>()))
            .ThrowsAsync(new System.Exception("Database error"));

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<System.Exception>()
            .WithMessage("Database error");

        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never); 
    }
}
