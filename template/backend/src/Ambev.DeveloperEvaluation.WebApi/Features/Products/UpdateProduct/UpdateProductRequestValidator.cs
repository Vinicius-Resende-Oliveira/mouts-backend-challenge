using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.UpdateProduct;

/// <summary>
/// Validator for UpdateProductRequest that defines validation rules for product creation.
/// </summary>
public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    /// <summary>
    /// Initializes a new instance of the UpdateProductCommandValidator with defined validation rules.
    /// </summary>
    /// <remarks>
    /// Validation rules include:
    /// - Title: Required, must be between 3 and 250 characters
    /// - Price: Required, must be GreaterThan 0
    /// - Description: Required, must be between 1 and 2000 characters
    /// - Category: Required, must be between 3 and 100 characters
    /// - Image: Required, must be between 3 and 250 characters
    /// </remarks>
    public UpdateProductRequestValidator()
    {
        RuleFor(product => product.Title).NotEmpty().MinimumLength(3).MaximumLength(250);
        RuleFor(product => product.Price).NotEmpty().GreaterThan(0);
        RuleFor(product => product.Description).NotEmpty().MinimumLength(1).MaximumLength(2000);
        RuleFor(product => product.Category).NotEmpty().MinimumLength(3).MaximumLength(100);
        RuleFor(product => product.Image).NotEmpty().MinimumLength(3).MaximumLength(250);
    }
}