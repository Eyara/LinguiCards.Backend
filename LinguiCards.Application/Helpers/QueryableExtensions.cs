using LinguiCards.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace LinguiCards.Application.Helpers;

public static class QueryableExtensions
{
    public static async Task<PaginatedResult<T>> ToPaginatedResultAsync<T>(this IQueryable<T> query, int pageNumber, int pageSize)
    {
        var count = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PaginatedResult<T>(items, count, pageNumber, pageSize);
    }
}
