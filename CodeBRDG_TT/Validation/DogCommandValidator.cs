using CodeBRDG_TT.Commands;
using CodeBRDG_TT.Data.UnitOfWork;
using FluentValidation;

public class DogCommandValidator : AbstractValidator<RegisterDogCommand>
{
    public DogCommandValidator(IUnitOfWork unitOfWork)
    {
        var existingDogNames = unitOfWork.Dogs.GetAll().Select(d => d.Name).ToHashSet();

        RuleFor(d => d.Name)
            .NotEmpty().WithMessage("Dog name is required.")
            .Must(name => !existingDogNames.Contains(name))
            .WithMessage("A dog with this name already exists.");

        RuleFor(d => d.Color)
            .NotEmpty().WithMessage("Color is required.");

        RuleFor(d => d.TailLength)
            .GreaterThan(0).WithMessage("Tail length must be greater than zero.");

        RuleFor(d => d.Weight)
            .GreaterThan(0).WithMessage("Weight must be greater than zero.");
    }
}
