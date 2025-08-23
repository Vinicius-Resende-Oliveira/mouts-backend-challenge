using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Carts.Common;

public class BaseCartItemValidator : AbstractValidator<BaseCartItem>
{
    public BaseCartItemValidator()
    {
        RuleFor(cart => cart.ProductId).NotEmpty()
            .NotEmpty()
            .WithMessage("The productId cannot be empty.");

        RuleFor(cart => cart.Quantity)
            .NotEmpty()
            .GreaterThan(0)
            .WithMessage("Quantity must be greather 0.")
            .LessThan(21)
            .WithMessage("Quantity must be less 20.");
    }
}
