namespace Ambev.DeveloperEvaluation.Application.Carts.Common;

public class GetCartResult
{
    public Guid UserId { get; set; }
    public DateTime Date { get; set; }

    public List<BaseCartItem>? Products { get; set; }
}