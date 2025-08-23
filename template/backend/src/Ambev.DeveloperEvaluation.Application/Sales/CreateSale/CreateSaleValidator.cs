using Ambev.DeveloperEvaluation.Application.Sales.Common;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleCommandValidator : AbstractValidator<CreateSaleCommand>
{
    /// <summary>
    /// Initializes a new instance of the CreateSaleCommandValidator with defined validation rules.
    /// </summary>
    /// <remarks>
    /// </remarks>
    public CreateSaleCommandValidator()
    {
        RuleFor(sale => sale.CartId).NotEmpty();
        RuleFor(sale => sale.SaleDate).NotEmpty();
        RuleFor(sale => sale.Customer).NotEmpty().MinimumLength(3).MaximumLength(100);
        RuleFor(sale => sale.Branch).NotEmpty().MinimumLength(3).MaximumLength(100);
    }
}