using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

/// <summary>
/// Repository interface for all entity operations
/// </summary>
public interface IBaseRepository<TEntity> where TEntity : BaseEntity
{
    void Update(TEntity entity);
    Task<TEntity> CreateAsync(TEntity user, CancellationToken cancellationToken = default);
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    IQueryable<TEntity> GetAll(CancellationToken cancellationToken = default);

    IQueryable<TEntity> Filter(IQueryable<TEntity> queryable, string property, string? filter);
    IQueryable<TEntity> FilterIn(IQueryable<TEntity> queryable, string property, IEnumerable<object?> filter);
    IQueryable<TEntity> FilterRange(IQueryable<TEntity> queryable, string property, object? min = null, object? max = null);
    IQueryable<TEntity> NumericFilter(IQueryable<TEntity> queryable, string property, decimal? filter);
    IQueryable<TEntity> DateFilter(IQueryable<TEntity> queryable, string property, DateTime? filter);
    IQueryable<TEntity> OrderByFields(IQueryable<TEntity> queryable, params (string field, bool desc)[] orders);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
