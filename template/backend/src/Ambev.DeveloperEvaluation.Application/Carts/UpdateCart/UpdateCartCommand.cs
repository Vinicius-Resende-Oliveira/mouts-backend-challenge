using Ambev.DeveloperEvaluation.Application.Carts.Common;
using Ambev.DeveloperEvaluation.Common.Validation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Carts.UpdateCart;

public class UpdateCartCommand : BaseCart, IRequest<UpdateCartResult>
{
    public Guid Id { get; set; }
    public string Password { get; set; } = string.Empty;

    public ValidationResultDetail Validate()
    {
        var validator = new UpdateCartCommandValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }
}
