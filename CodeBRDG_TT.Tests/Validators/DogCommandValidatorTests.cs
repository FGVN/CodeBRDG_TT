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
                new Dog { name = "Bolt" },
                new Dog { name = "Jessie" }
            }.AsQueryable());

        _validator = new DogCommandValidator(_unitOfWorkMock.Object);
    }

    [Fact]
    public void Validate_ValidCommand_ShouldNotHaveValidationError()
    {
        // Arrange
        var command = new RegisterDogCommand
        {
            name = "Rocky",
            color = "Brown",
            tail_length = 5,
            weight = 20
        };

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyName_ShouldHaveValidationError()
    {
        // Arrange
        var command = new RegisterDogCommand
        {
            name = "",
            color = "Brown",
            tail_length = 5,
            weight = 20
        };

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.name)
            .WithErrorMessage("Dog name is required.");
    }

    [Fact]
    public void Validate_DuplicateName_ShouldHaveValidationError()
    {
        // Arrange
        var command = new RegisterDogCommand
        {
            name = "Bolt",  
            color = "Brown",
            tail_length = 5,
            weight = 20
        };

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.name)
            .WithErrorMessage("A dog with this name already exists.");
    }

    [Fact]
    public void Validate_EmptyColor_ShouldHaveValidationError()
    {
        // Arrange
        var command = new RegisterDogCommand
        {
            name = "Rocky",
            color = "",
            tail_length = 5,
            weight = 20
        };

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.color)
            .WithErrorMessage("Color is required.");
    }

    [Fact]
    public void Validate_NonPositiveTailLength_ShouldHaveValidationError()
    {
        // Arrange
        var command = new RegisterDogCommand
        {
            name = "Rocky",
            color = "Brown",
            tail_length = -1,  
            weight = 20
        };

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.tail_length)
            .WithErrorMessage("Tail length must be greater than zero.");
    }

    [Fact]
    public void Validate_NonPositiveWeight_ShouldHaveValidationError()
    {
        // Arrange
        var command = new RegisterDogCommand
        {
            name = "Rocky",
            color = "Brown",
            tail_length = 5,
            weight = 0 
        };

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.weight)
            .WithErrorMessage("Weight must be greater than zero.");
    }
}
