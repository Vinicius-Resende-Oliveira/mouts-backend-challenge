using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

public class CartItemValidator : AbstractValidator<CartItem>
{
    public CartItemValidator()
    {
        RuleFor(cart => cart.CartId)
            .NotEmpty()
            .WithMessage("The cartId cannot be empty.");

        RuleFor(cart => cart.ProductId).NotEmpty()
            .NotEmpty()
            .WithMessage("The productId cannot be empty.");

        RuleFor(cart => cart.Quantity)
            .NotEmpty()
            .GreaterThan(0)
            .WithMessage("Quantity must be greather 0.");
    }
}
