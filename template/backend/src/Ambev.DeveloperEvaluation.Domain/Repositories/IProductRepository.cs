using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

public interface IProductRepository : IBaseRepository<Product>
{
    Task<List<Product>> GetByIdsAsync(List<Guid> ids, CancellationToken cancellation);
}