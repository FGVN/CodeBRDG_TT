using CodeBRDG_TT.Commands;
using CodeBRDG_TT.Commands.Handlers;
using CodeBRDG_TT.Data.Repositories;
using CodeBRDG_TT.Data.UnitOfWork;
using CodeBRDG_TT.Models;
using FluentAssertions;
using Moq;

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

        _mockUnitOfWork.Setup(u => u.Dogs).Returns(_mockDogRepository.Object);

        _handler = new RegisterDogCommandHandler(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldRegisterDog()
    {
        // Arrange
        var command = new RegisterDogCommand("Buddy", "brown", 6, 30 );

        Dog? addedDog = null;

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
        addedDog!.Name.Should().Be(command.Name);
        addedDog.Color.Should().Be(command.Color);
        addedDog.TailLength.Should().Be(command.TailLength);
        addedDog.Weight.Should().Be(command.Weight);
    }
    [Fact]
    public async Task Handle_TailLengthNegative_ShouldThrowException()
    {
        // Arrange
        var command = new RegisterDogCommand("Max", "golden", -5, 20);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
            .WithMessage("Tail length must be greater than or equal to zero.*")
            .WithParameterName("TailLength"); 

        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_CommandThrowsException_ShouldReturnFalse()
    {
        // Arrange
        var command = new RegisterDogCommand("Rex", "white & black", 5, 20);

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
