using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Application.Carts.Common;

public class BaseCartItem
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}