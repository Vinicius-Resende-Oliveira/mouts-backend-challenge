using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Application.Carts.Common;

public class GetCartResult : BaseEntity
{
    public Guid UserId { get; set; }
    public DateTime Date { get; set; }

    public List<BaseCartItem>? Products { get; set; }
}