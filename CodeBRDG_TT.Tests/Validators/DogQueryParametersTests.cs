using CodeBRDG_TT.Models;
using CodeBRDG_TT.Queries;
using CodeBRDG_TT.Validators;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

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
            attribute = "weight",
            order = "asc",
            pageNumber = 1,
            pageSize = 10
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
            attribute = "invalid_attribute", 
            order = "asc",
            pageNumber = 1,
            pageSize = 10
        };

        // Act & Assert
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(q => q.attribute)
            .WithErrorMessage("Invalid attribute name for sorting.");
    }

    [Fact]
    public void Validate_InvalidOrder_ShouldHaveValidationError()
    {
        // Arrange
        var query = new DogsQuery
        {
            attribute = "weight",
            order = "wrong_order", 
            pageNumber = 1,
            pageSize = 10
        };

        // Act & Assert
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(q => q.order)
            .WithErrorMessage("Order must be 'asc' or 'desc'.");
    }

    [Fact]
    public void Validate_InvalidPageNumber_ShouldHaveValidationError()
    {
        // Arrange
        var query = new DogsQuery
        {
            attribute = "weight",
            order = "asc",
            pageNumber = 0,
            pageSize = 10
        };

        // Act & Assert
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(q => q.pageNumber)
            .WithErrorMessage("Page number must be greater than zero.");
    }

    [Fact]
    public void Validate_InvalidPageSize_ShouldHaveValidationError()
    {
        // Arrange
        var query = new DogsQuery
        {
            attribute = "weight",
            order = "asc",
            pageNumber = 1,
            pageSize = 0 
        };

        // Act & Assert
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(q => q.pageSize)
            .WithErrorMessage("Page size must be greater than zero.");
    }

    [Fact]
    public void Validate_EmptyAttribute_ShouldNotHaveValidationError()
    {
        // Arrange
        var query = new DogsQuery
        {
            attribute = "", 
            order = "asc",
            pageNumber = 1,
            pageSize = 10
        };

        // Act & Assert
        var result = _validator.TestValidate(query);
        result.ShouldNotHaveValidationErrorFor(q => q.attribute);
    }

    [Fact]
    public void Validate_EmptyOrder_ShouldNotHaveValidationError()
    {
        // Arrange
        var query = new DogsQuery
        {
            attribute = "weight",
            order = "", 
            pageNumber = 1,
            pageSize = 10
        };

        // Act & Assert
        var result = _validator.TestValidate(query);
        result.ShouldNotHaveValidationErrorFor(q => q.order);
    }
}
