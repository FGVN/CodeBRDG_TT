using CodeBRDG_TT.Queries;
using CodeBRDG_TT.Validators;
using FluentValidation.TestHelper;

namespace CodeBRDG_TT.Tests.Validators;

public class DogsQueryValidatorTests
{
    private readonly DogsQueryValidator _validator;

    public DogsQueryValidatorTests()
    {
        _validator = new DogsQueryValidator();
    }

    [Fact]
    public void Validate_ValidParameters_ShouldNotHaveValidationError()
    {
        // Arrange
        var query = new DogsQuery
        {
            Attribute = "weight",
            Order = "asc",
            PageNumber= 1,
            PageSize = 10
        };

        // Act & Assert
        var result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_InvalidAttribute_ShouldHaveValidationError()
    {
        // Arrange
        var query = new DogsQuery
        {
            Attribute = "invalid_attribute", 
            Order = "asc",
            PageNumber= 1,
            PageSize = 10
        };

        // Act & Assert
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(q => q.Attribute)
            .WithErrorMessage("Invalid attribute name for sorting.");
    }

    [Fact]
    public void Validate_InvalidOrder_ShouldHaveValidationError()
    {
        // Arrange
        var query = new DogsQuery
        {
            Attribute = "weight",
            Order = "wrong_order", 
            PageNumber= 1,
            PageSize = 10
        };

        // Act & Assert
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(q => q.Order)
            .WithErrorMessage("Order must be 'asc' or 'desc'.");
    }

    [Fact]
    public void Validate_InvalidPageNumber_ShouldHaveValidationError()
    {
        // Arrange
        var query = new DogsQuery
        {
            Attribute = "weight",
            Order = "asc",
            PageNumber= 0,
            PageSize = 10
        };

        // Act & Assert
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(q => q.PageNumber)
            .WithErrorMessage("Page number must be greater than zero.");
    }

    [Fact]
    public void Validate_InvalidPageSize_ShouldHaveValidationError()
    {
        // Arrange
        var query = new DogsQuery
        {
            Attribute = "weight",
            Order = "asc",
            PageNumber= 1,
            PageSize = 0 
        };

        // Act & Assert
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(q => q.PageSize)
            .WithErrorMessage("Page size must be greater than zero.");
    }

    [Fact]
    public void Validate_EmptyAttribute_ShouldNotHaveValidationError()
    {
        // Arrange
        var query = new DogsQuery
        {
            Attribute = "", 
            Order = "asc",
            PageNumber= 1,
            PageSize = 10
        };

        // Act & Assert
        var result = _validator.TestValidate(query);
        result.ShouldNotHaveValidationErrorFor(q => q.Attribute);
    }

    [Fact]
    public void Validate_EmptyOrder_ShouldNotHaveValidationError()
    {
        // Arrange
        var query = new DogsQuery
        {
            Attribute = "weight",
            Order = "", 
            PageNumber = 1,
            PageSize = 10
        };

        // Act & Assert
        var result = _validator.TestValidate(query);
        result.ShouldNotHaveValidationErrorFor(q => q.Order);
    }
}
