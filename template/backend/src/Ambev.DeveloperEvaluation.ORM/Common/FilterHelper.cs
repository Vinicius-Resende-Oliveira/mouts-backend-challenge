using System.Linq.Dynamic.Core;

namespace Ambev.DeveloperEvaluation.ORM.Common;

public static class FilterHelper
{
    public static IQueryable<TEntity> Filter<TEntity>(this IQueryable<TEntity> queryable, string property, string? filter)
    {
        if (string.IsNullOrWhiteSpace(property) || string.IsNullOrWhiteSpace(filter) || filter == "*")
            return queryable;

        if (filter.StartsWith("*") && filter.EndsWith("*") && filter.Length > 2)
        {
            var value = filter.Trim('*');
            return queryable.Where($"{property}.Contains(@0)", value);
        }

        if (filter.StartsWith("*"))
            return queryable.Where($"{property}.EndsWith(@0)", filter.TrimStart('*'));

        if (filter.EndsWith("*"))
            return queryable.Where($"{property}.StartsWith(@0)", filter.TrimEnd('*'));

        return queryable.Where($"{property} == @0", filter);
    }

    public static IQueryable<TEntity> FilterIn<TEntity>(this IQueryable<TEntity> queryable, string property, IEnumerable<object?> values)
    {
        if (string.IsNullOrWhiteSpace(property) || values == null) return queryable;

        var vals = values.Where(v => v != null).ToArray();
        if (vals.Length == 0) return queryable;

        var conditions = string.Join(" OR ", Enumerable.Range(0, vals.Length).Select(i => $"{property} == @{i}"));
        return queryable.Where(conditions, vals);
    }

    public static IQueryable<TEntity> FilterRange<TEntity>(this IQueryable<TEntity> queryable, string property, object? min = null, object? max = null)
    {
        if (string.IsNullOrWhiteSpace(property) || (min is null && max is null)) return queryable;

        if (min is not null && max is not null)
            return queryable.Where($"{property} >= @0 AND {property} <= @1", min, max);

        if (min is not null)
            return queryable.Where($"{property} >= @0", min);

        return queryable.Where($"{property} <= @0", max!);
    }

    public static IQueryable<TEntity> FilterNumeric<TEntity>(this IQueryable<TEntity> queryable, string property, decimal? filter)
    {
        if (string.IsNullOrWhiteSpace(property) || !filter.HasValue) return queryable;

        if (property.StartsWith("_min", StringComparison.OrdinalIgnoreCase))
            return queryable.FilterRange(property[4..], min: filter.Value);

        if (property.StartsWith("_max", StringComparison.OrdinalIgnoreCase))
            return queryable.FilterRange(property[4..], max: filter.Value);

        return queryable.Where($"{property} == @0", filter.Value);
    }

    public static IQueryable<TEntity> FilterDate<TEntity>(this IQueryable<TEntity> queryable, string property, DateTime? filter)
    {
        if (string.IsNullOrWhiteSpace(property) || !filter.HasValue) return queryable;

        if (property.StartsWith("_min", StringComparison.OrdinalIgnoreCase))
            return queryable.FilterRange(property[4..], min: filter.Value);

        if (property.StartsWith("_max", StringComparison.OrdinalIgnoreCase))
            return queryable.FilterRange(property[4..], max: filter.Value);

        return queryable.Where($"{property} == @0", filter.Value);
    }

    public static IQueryable<TEntity> OrderByFields<TEntity>(this IQueryable<TEntity> queryable, params (string field, bool desc)[] orders)
    {
        if (orders == null || orders.Length == 0) return queryable;

        var orderStr = string.Join(", ", orders
            .Where(o => !string.IsNullOrWhiteSpace(o.field))
            .Select(o => $"{o.field} {(o.desc ? "desc" : "asc")}"));

        if (string.IsNullOrWhiteSpace(orderStr)) return queryable;
        return queryable.OrderBy(orderStr);
    }
}
