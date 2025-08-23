using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class CartRepository : BaseRepository<Cart>, ICartRepository
{
    private DefaultContext context { get; set; }
    public CartRepository(DefaultContext context) :base(context)
        => this.context = context;

    /// <summary>
    /// Retrieves a cart by their unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the cart</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The cart if found, null otherwise</returns>
    public new async Task<Cart?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Carts
            .Include(c => c.Products)
            .Include("Products.Product")
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }
}
