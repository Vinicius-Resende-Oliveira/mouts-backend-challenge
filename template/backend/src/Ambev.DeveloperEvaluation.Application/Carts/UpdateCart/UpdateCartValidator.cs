using Ambev.DeveloperEvaluation.Application.Carts.Common;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Carts.UpdateCart;

public class UpdateCartCommandValidator : AbstractValidator<UpdateCartCommand>
{
    public UpdateCartCommandValidator()
    {
        RuleFor(product => product.Id).NotEmpty();
        RuleFor(cart => cart.UserId)
            .NotEmpty()
            .WithMessage("The userId cannot be empty.");
        RuleFor(cart => cart.Date)
            .NotEmpty()
            .WithMessage("The date cannot be empty.");
        RuleFor(cart => cart.Products)
            .NotEmpty()
            .WithMessage("Cart must contain at least one product.")
            .Must(products => products?.Count > 0)
            .WithMessage("Cart must contain at least one product.")
            .Must(products =>
            {
                if (products == null) return true;
                var productIds = products.Select(p => p.ProductId);
                return productIds.Distinct().Count() == productIds.Count();
            })
            .WithMessage("Cart cannot contain more than one item with the same product id.")
            .ForEach(item => item.SetValidator(new BaseCartItemValidator()));
    }
}
