using CodeBRDG_TT.Models;
using CodeBRDG_TT.Queries;
using FluentValidation;
using System.Reflection;

namespace CodeBRDG_TT.Validators;

public class DogsQueryValidator : AbstractValidator<DogsQuery>
{
    public DogsQueryValidator()
    {
        RuleFor(q => q.Attribute)
            .Must(attribute => string.IsNullOrEmpty(attribute) || typeof(Dog).GetProperty(attribute, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance) != null)
            .WithMessage("Invalid attribute name for sorting.");

        RuleFor(q => q.Order)
            .Must(order => string.IsNullOrEmpty(order) || order.Equals("asc", StringComparison.OrdinalIgnoreCase) || order.Equals("desc", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Order must be 'asc' or 'desc'.");

        RuleFor(q => q.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than zero.");

        RuleFor(q => q.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than zero.");
    }
}
