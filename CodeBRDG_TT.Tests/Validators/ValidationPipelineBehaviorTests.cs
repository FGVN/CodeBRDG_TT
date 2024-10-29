using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;

namespace CodeBRDG_TT.Tests.Validators;
public class ValidationPipelineBehaviorTests
{
    private readonly Mock<IValidator<TestRequest>> _validatorMock;
    private readonly ValidationPipelineBehavior<TestRequest, TestResponse> _behavior;

    public ValidationPipelineBehaviorTests()
    {
        _validatorMock = new Mock<IValidator<TestRequest>>();
        _behavior = new ValidationPipelineBehavior<TestRequest, TestResponse>(new[] { _validatorMock.Object });
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldProceedToNext()
    {
        // Arrange
        var request = new TestRequest();
        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var nextMock = new Mock<RequestHandlerDelegate<TestResponse>>();
        nextMock.Setup(n => n()).ReturnsAsync(new TestResponse());

        // Act
        var response = await _behavior.Handle(request, nextMock.Object, CancellationToken.None);

        // Assert
        nextMock.Verify(n => n(), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidRequest_ShouldThrowValidationException()
    {
        // Arrange
        var request = new TestRequest();
        var failures = new List<ValidationFailure> { new ValidationFailure("Property", "Validation failed") };
        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _behavior.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_MultipleValidators_ShouldAggregateErrors()
    {
        // Arrange
        var request = new TestRequest();
        var validator1 = new Mock<IValidator<TestRequest>>();
        var validator2 = new Mock<IValidator<TestRequest>>();

        validator1.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure> { new ValidationFailure("Prop1", "Error 1") }));

        validator2.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure> { new ValidationFailure("Prop2", "Error 2") }));

        var behavior = new ValidationPipelineBehavior<TestRequest, TestResponse>(new[] { validator1.Object, validator2.Object });

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() => behavior.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None));
        Assert.Equal(2, exception.Errors.Count()); 
    }
}

// Supporting classes 
public class TestRequest : IRequest<TestResponse> { }
public class TestResponse { }
