using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class ProductRepository(DefaultContext context) : BaseRepository<Product>(context), IProductRepository
{
    public async Task<List<Product>> GetByIdsAsync(List<Guid> ids, CancellationToken cancellation)
    {
        return await  DbSet
            .Where(p => ids.Contains(p.Id))
            .ToListAsync(cancellation);
    }
}
