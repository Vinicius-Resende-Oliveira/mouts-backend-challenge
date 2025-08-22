using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class Cart : BaseEntity
{
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public DateTime Date { get; set; }

    public ICollection<CartItem>? Products { get; set; }

    public Cart()
    {
        CreatedAt = DateTime.UtcNow;
    }
}