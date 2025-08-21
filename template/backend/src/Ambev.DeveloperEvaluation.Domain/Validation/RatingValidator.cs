using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

public class RatingValidator : AbstractValidator<Rating>
{
    public RatingValidator()
    {
        RuleFor(rating => rating.Rate)
            .NotEmpty()
            .GreaterThan(0)
            .LessThan(5)
            .WithMessage("Rate must be between 0 and 5.");

        RuleFor(rating => rating.Count)
            .NotEmpty()
            .GreaterThan(0)
            .WithMessage("Count must be greather 0.");
    }
}