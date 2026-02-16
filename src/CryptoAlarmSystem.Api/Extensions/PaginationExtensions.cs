using CryptoAlarmSystem.Api.Models;

namespace CryptoAlarmSystem.Api.Extensions;

/// <summary>
/// Pagination yardımcı metodları
/// </summary>
public static class PaginationExtensions
{
    /// <summary>
    /// IQueryable için sayfalama uygular
    /// </summary>
    public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> query, PaginationRequest pagination)
    {
        return query
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize);
    }

    /// <summary>
    /// List için sayfalama uygular
    /// </summary>
    public static List<T> ApplyPagination<T>(this List<T> list, PaginationRequest pagination)
    {
        return list
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToList();
    }

    /// <summary>
    /// PagedResponse oluşturur
    /// </summary>
    public static PagedResponse<T> ToPagedResponse<T>(this List<T> data, int pageNumber, int pageSize, int totalRecords)
    {
        return new PagedResponse<T>(data, pageNumber, pageSize, totalRecords);
    }
}
