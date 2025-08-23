using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

/// <summary>
/// Validator for CreateSaleRequest that defines validation rules for product creation.
/// </summary>
public class CreateSaleRequestValidator : AbstractValidator<CreateSaleRequest>
{
    /// <summary>
    /// Initializes a new instance of the CreateSaleCommandValidator with defined validation rules.
    /// </summary>
    /// <remarks>
    /// </remarks>
    public CreateSaleRequestValidator()
    {
        RuleFor(sale => sale.CartId).NotEmpty();
        RuleFor(sale => sale.SaleDate).NotEmpty();
        RuleFor(sale => sale.Customer).NotEmpty().MinimumLength(3).MaximumLength(100);
        RuleFor(sale => sale.Branch).NotEmpty().MinimumLength(3).MaximumLength(100);
    }
}