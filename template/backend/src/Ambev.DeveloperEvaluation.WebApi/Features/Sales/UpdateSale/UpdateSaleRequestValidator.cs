using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

/// <summary>
/// Validator for UpdateSaleRequest that defines validation rules for sale creation.
/// </summary>
public class UpdateSaleRequestValidator : AbstractValidator<UpdateSaleRequest>
{
    /// <summary>
    /// Initializes a new instance of the UpdateSaleRequestValidator with defined validation rules.
    /// </summary>
    /// <remarks>
    /// </remarks>
    public UpdateSaleRequestValidator()
    {
        RuleFor(sale => sale.SaleDate).NotEmpty();
        RuleFor(sale => sale.Customer).NotEmpty().MinimumLength(3).MaximumLength(100);
        RuleFor(sale => sale.Branch).NotEmpty().MinimumLength(3).MaximumLength(100);

        RuleFor(sale => sale.Items).NotEmpty()
            .Must(items => items.Count > 0)
            .WithMessage("A sale must have at least one item.");

        RuleForEach(sale => sale.Items).SetValidator(new UpdateSaleItemRequestValidator());
    }
}


/// <summary>
/// Validator for UpdatedSaleItemRequest that defines validation rules for product updation command.
/// </summary>
public class UpdateSaleItemRequestValidator : AbstractValidator<UpdateSaleItemRequest>
{
    /// <summary>
    /// Initializes a new instance of the UpdateSaleItemRequestValidator with defined validation rules.
    /// </summary>
    /// <remarks>
    /// </remarks>
    public UpdateSaleItemRequestValidator()
    {
        RuleFor(item => item.ProductId).NotEmpty();
        RuleFor(item => item.Quantity).GreaterThan(0).LessThanOrEqualTo(20);
    }
}
