using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Carts.GetCart;

/// <summary>
/// Validator for GetCartCommand
/// </summary>
public class GetCartQueryValidator : AbstractValidator<GetCartQuery>
{
    /// <summary>
    /// Initializes validation rules for GetCartCommand
    /// </summary>
    public GetCartQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Cart ID is required");
    }
}
