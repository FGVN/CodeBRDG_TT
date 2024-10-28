using CodeBRDG_TT.Commands;
using CodeBRDG_TT.Data.UnitOfWork;
using FluentValidation;

public class DogCommandValidator : AbstractValidator<RegisterDogCommand>
{
    public DogCommandValidator(IUnitOfWork unitOfWork)
    {
        var existingDogNames = unitOfWork.Dogs.GetAll().Select(d => d.name).ToHashSet();

        RuleFor(d => d.name)
            .NotEmpty().WithMessage("Dog name is required.")
            .Must(name => !existingDogNames.Contains(name))
            .WithMessage("A dog with this name already exists.");

        RuleFor(d => d.color)
            .NotEmpty().WithMessage("Color is required.");

        RuleFor(d => d.tail_length)
            .GreaterThan(0).WithMessage("Tail length must be greater than zero.");

        RuleFor(d => d.weight)
            .GreaterThan(0).WithMessage("Weight must be greater than zero.");
    }
}
