using System.Linq.Expressions;
using UserService.Application.Enums;

namespace UserService.Application.Extensions;

public static class QueryExtension
{
    public static IQueryable<T> FilterByDeletedStatus<T>(
        this IQueryable<T> query,
        DeletedStatus deletedStatus,
        Expression<Func<T, bool>> deletedPropertySelector
    )
    {
        query = deletedStatus switch
        {
            DeletedStatus.All => query,
            DeletedStatus.OnlyDeleted => query.Where(deletedPropertySelector),
            DeletedStatus.OnlyActive
                => query.Where(
                    Expression.Lambda<Func<T, bool>>(
                        Expression.Not(deletedPropertySelector.Body),
                        deletedPropertySelector.Parameters
                    )
                ),
        };

        return query;
    }
}
