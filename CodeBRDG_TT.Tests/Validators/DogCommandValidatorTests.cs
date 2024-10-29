using CodeBRDG_TT.Commands;
using CodeBRDG_TT.Data.UnitOfWork;
using FluentValidation.TestHelper;
using Moq;
using CodeBRDG_TT.Models;

namespace CodeBRDG_TT.Tests.Validators;

public class DogCommandValidatorTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly DogCommandValidator _validator;

    public DogCommandValidatorTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.Dogs.GetAll())
            .Returns(new List<Dog>
            {
                new Dog ( "Bolt", "white", 10, 5 ),
                new Dog ( "Jessie", "goldenbrown", 2, 13 )
            }.AsQueryable());

        _validator = new DogCommandValidator(_unitOfWorkMock.Object);
    }

    [Fact]
    public void Validate_ValidCommand_ShouldNotHaveValidationError()
    {
        // Arrange
        var command = new RegisterDogCommand("Rocky", "Brown", 5, 20);

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyName_ShouldHaveValidationError()
    {
        // Arrange
        var command = new RegisterDogCommand("", "Brown", 5, 20);

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.Name)
            .WithErrorMessage("Dog name is required.");
    }

    [Fact]
    public void Validate_DuplicateName_ShouldHaveValidationError()
    {
        // Arrange
        var command = new RegisterDogCommand("Bolt",  "Brown", 5, 20);

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.Name)
            .WithErrorMessage("A dog with this name already exists.");
    }

    [Fact]
    public void Validate_EmptyColor_ShouldHaveValidationError()
    {
        // Arrange
        var command = new RegisterDogCommand("Rocky", "", 5, 20);

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.Color)
            .WithErrorMessage("Color is required.");
    }

    [Fact]
    public void Validate_NonPositiveTailLength_ShouldHaveValidationError()
    {
        // Arrange
        var command = new RegisterDogCommand("Rocky", "Brown", -1, 20);

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.TailLength)
            .WithErrorMessage("Tail length must be greater than zero.");
    }

    [Fact]
    public void Validate_NonPositiveWeight_ShouldHaveValidationError()
    {
        // Arrange
        var command = new RegisterDogCommand("Rocky","Brown",5, 0);

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.Weight)
            .WithErrorMessage("Weight must be greater than zero.");
    }
}
