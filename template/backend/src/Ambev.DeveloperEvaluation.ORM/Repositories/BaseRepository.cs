using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.ORM.Common;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
{
    private readonly DefaultContext _context;
    protected DbSet<TEntity> DbSet { get; set; }

    public BaseRepository(DefaultContext context)
    {
        this._context = context;
        DbSet = _context.Set<TEntity>();
    }

    /// <summary>
    /// Update entity in the database
    /// </summary>
    /// <param name="entity">The entity to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created entity</returns>
    public void Update(TEntity entity)
    {
        DbSet.Update(entity);
    }

    /// <summary>
    /// Creates a new entity in the database
    /// </summary>
    /// <param name="entity">The entity to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created entity</returns>
    public async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    /// <summary>
    /// Retrieves a entity by their unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the entity</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The entity if found, null otherwise</returns>
    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    /// <summary>
    /// Deletes a entity from the database
    /// </summary>
    /// <param name="id">The unique identifier of the entity to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the entity was deleted, false if not found</returns>
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return false;

        DbSet.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <summary>
    /// Return IQueryable of all entities in the database
    /// </summary>
    /// <param name="entity">The entity to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created entity</returns>
    public IQueryable<TEntity> GetAll(CancellationToken cancellationToken = default)
    {
        return DbSet;
    }

    public IQueryable<TEntity> Filter(IQueryable<TEntity> queryable, string property, string? filter)
    {
        return queryable.Filter(property, filter);
    }

    public IQueryable<TEntity> FilterIn(IQueryable<TEntity> queryable, string property, IEnumerable<object?> filter)
    {
        return queryable.FilterIn(property, filter);
    }

    public IQueryable<TEntity> FilterRange(IQueryable<TEntity> queryable, string property, object? min = null, object? max = null)
    {
        return queryable.FilterRange(property, min, max);
    }

    public IQueryable<TEntity> NumericFilter(IQueryable<TEntity> queryable, string property, decimal? filter)
    {
        return queryable.FilterNumeric(property, filter);
    }

    public IQueryable<TEntity> DateFilter(IQueryable<TEntity> queryable, string property, DateTime? filter)
    {

        return queryable.FilterDate(property, filter);
    }

    public IQueryable<TEntity> OrderByFields(IQueryable<TEntity> queryable, params (string field, bool desc)[] orders)
    {
        return queryable.OrderByFields(orders);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}