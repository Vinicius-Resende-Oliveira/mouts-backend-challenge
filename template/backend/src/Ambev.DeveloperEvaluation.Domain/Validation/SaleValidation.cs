using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

public class SaleValidator : AbstractValidator<Sale>
{
    public SaleValidator()
    {
        RuleFor(item => item.Id).NotEmpty();
        RuleFor(item => item.SaleNumber).NotEmpty().GreaterThan(0);
        RuleFor(sale => sale.SaleDate).NotEmpty();
        RuleFor(sale => sale.Customer).NotEmpty().MinimumLength(3).MaximumLength(100);
        RuleFor(sale => sale.Branch).NotEmpty().MinimumLength(3).MaximumLength(100);
       
        RuleFor(sale => sale.Items).NotEmpty()
            .Must(items => items.Count > 0)
            .WithMessage("A sale must have at least one item.");

        RuleForEach(sale => sale.Items).SetValidator(new SaleItemValidator());
    }
}
