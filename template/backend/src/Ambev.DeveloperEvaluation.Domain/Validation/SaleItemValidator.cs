using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

public class SaleItemValidator : AbstractValidator<SaleItem>
{
    public SaleItemValidator()
    {
        RuleFor(item => item.Id).NotEmpty();
        RuleFor(item => item.ProductId).NotEmpty();
        RuleFor(item => item.Quantity).GreaterThan(0).LessThanOrEqualTo(20);
        RuleFor(item => item.UnitPrice).GreaterThan(0);
        RuleFor(item => item.Discount).GreaterThanOrEqualTo(0).LessThan(1);
        RuleFor(item => item.TotalValue).GreaterThanOrEqualTo(0);
    }
}
