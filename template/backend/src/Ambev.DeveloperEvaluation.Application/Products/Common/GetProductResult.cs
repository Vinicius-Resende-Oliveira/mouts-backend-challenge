namespace Ambev.DeveloperEvaluation.Application.Products.Common;

/// <summary>
/// Common response  model for product operations
/// </summary>
public class GetProductResult : BaseProduct
{
    /// <summary>
    /// The unique identifier of the product
    /// </summary>
    public Guid Id { get; set; }
}
