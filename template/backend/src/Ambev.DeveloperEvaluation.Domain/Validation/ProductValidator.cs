using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

public class ProductValidator : AbstractValidator<Product>
{
    public ProductValidator()
    {
        RuleFor(product => product.Title).NotEmpty().MinimumLength(3).MaximumLength(250);
        RuleFor(product => product.Price).NotEmpty().GreaterThan(0);
        RuleFor(product => product.Description).NotEmpty().MinimumLength(1).MaximumLength(2000);
        RuleFor(product => product.Category).NotEmpty().MinimumLength(3).MaximumLength(100);
        RuleFor(product => product.Image).NotEmpty().MinimumLength(3).MaximumLength(250);
        RuleFor(product => product.Rating).SetValidator(new RatingValidator());
    }
}
