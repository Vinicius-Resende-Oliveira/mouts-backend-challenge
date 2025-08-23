using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Validator for UpdatedSaleCommand that defines validation rules for product updation command.
/// </summary>
public class UpdateSaleCommandValidator : AbstractValidator<UpdateSaleCommand>
{
    /// <summary>
    /// Initializes a new instance of the UpdateSaleCommandValidator with defined validation rules.
    /// </summary>
    /// <remarks>
    /// </remarks>
    public UpdateSaleCommandValidator()
    {
        RuleFor(sale => sale.SaleDate).NotEmpty();
        RuleFor(sale => sale.Customer).NotEmpty().MinimumLength(3).MaximumLength(100);
        RuleFor(sale => sale.Branch).NotEmpty().MinimumLength(3).MaximumLength(100);

        RuleFor(sale => sale.Items).NotEmpty()
            .Must(items => items.Count > 0)
            .WithMessage("A sale must have at least one item.");

        RuleForEach(sale => sale.Items).SetValidator(new UpdateSaleItemCommandValidator());
    }
}


/// <summary>
/// Validator for UpdatedSaleItemCommand that defines validation rules for product updation command.
/// </summary>
public class UpdateSaleItemCommandValidator : AbstractValidator<UpdateSaleItemCommand>
{
    /// <summary>
    /// Initializes a new instance of the UpdateSaleItemCommandValidator with defined validation rules.
    /// </summary>
    /// <remarks>
    /// </remarks>
    public UpdateSaleItemCommandValidator()
    {
        RuleFor(item => item.ProductId).NotEmpty();
        RuleFor(item => item.Quantity).GreaterThan(0).LessThanOrEqualTo(20);
    }
}
