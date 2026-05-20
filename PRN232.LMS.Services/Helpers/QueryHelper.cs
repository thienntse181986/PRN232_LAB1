using PRN232.LMS.Services.Models.Request;
using Microsoft.EntityFrameworkCore;

namespace PRN232.LMS.Services.Helpers;

public static class QueryHelper
{
    public static IQueryable<T> ApplySorting<T>(IQueryable<T> query, string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort)) return query;

        var fields = sort.Split(',', StringSplitOptions.RemoveEmptyEntries);
        IOrderedQueryable<T>? ordered = null;

        foreach (var field in fields)
        {
            var trimmed = field.Trim();
            bool descending = trimmed.StartsWith('-');
            var propertyName = descending ? trimmed[1..] : trimmed;

            var prop = typeof(T).GetProperties()
                .FirstOrDefault(p => string.Equals(p.Name, propertyName, StringComparison.OrdinalIgnoreCase));

            if (prop is null) continue;

            var param = System.Linq.Expressions.Expression.Parameter(typeof(T), "x");
            var memberAccess = System.Linq.Expressions.Expression.MakeMemberAccess(param, prop);
            var keySelector = System.Linq.Expressions.Expression.Lambda(memberAccess, param);

            var methodName = ordered is null
                ? (descending ? "OrderByDescending" : "OrderBy")
                : (descending ? "ThenByDescending" : "ThenBy");

            var method = typeof(Queryable).GetMethods()
                .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), prop.PropertyType);

            var result = method.Invoke(null, new object[] { ordered ?? (object)query, keySelector });
            ordered = result as IOrderedQueryable<T>;
        }

        return ordered ?? query;
    }

    public static async Task<(IEnumerable<T> Items, int Total)> ApplyPagingAsync<T>(
        IQueryable<T> query, int page, int size)
    {
        var total = await query.CountAsync();
        var items = await query.Skip((page - 1) * size).Take(size).ToListAsync();
        return (items, total);
    }
}
