using Microsoft.EntityFrameworkCore;
using Stpm.Core.Collections;
using System.Linq.Dynamic.Core;
using Stpm.Core.Contracts;

namespace Stpm.Services.Extensions;

public static class PagedListExtensions
{
    // Tạo biểu thức dùng để sắp xếp dữ liệu
    // Sử dụng sau mệnh đề ORDER BY trong truy vấn
    public static string GetOrderExpression(this IPagingParams pagingParams, string defaultColumn = "Id")
    {
        var column = string.IsNullOrWhiteSpace(pagingParams.SortColumn) ? defaultColumn : pagingParams.SortColumn;

        var order = "ASC".Equals(pagingParams.SortOrder, StringComparison.OrdinalIgnoreCase) ? pagingParams.SortOrder : "DESC";

        return $"{column} {order}";
    }

    public static async Task<IPagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, IPagingParams pagingParams, CancellationToken cancellationToken = default)
    {
        var totalCount = await source.CountAsync(cancellationToken);
        var items = await source.OrderBy(pagingParams.GetOrderExpression())
                                .Skip((pagingParams.PageNumber - 1) * pagingParams.PageSize)
                                .Take(pagingParams.PageSize)
                                .ToListAsync(cancellationToken);

        return new PagedList<T>
          (
            items,
            pagingParams.PageNumber,
            pagingParams.PageSize,
            totalCount
          );
    }

    public static async Task<IPagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, int pageNumber = 1, int pageSize = 10, string sortColumn = "Id", string sortOrder = "DESC", CancellationToken cancellationToken = default)
    {
        var totalCount = await source.CountAsync(cancellationToken);
        var items = await source.OrderBy($"{sortColumn} {sortOrder}")
                                .Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync(cancellationToken);

        return new PagedList<T>(items, pageNumber, pageSize, totalCount);
    }

    public static async Task<IPagedList<T>> ToPagedListAsync<T>(this IEnumerable<T> source, IPagingParams pagingParams)
    {
        var totalCount = await Task.FromResult(source.Count());
        var items = await Task.FromResult(source.OrderBy(x => pagingParams.GetOrderExpression())
                                .Skip((pagingParams.PageNumber - 1) * pagingParams.PageSize)
                                .Take(pagingParams.PageSize)
                                .ToList());

        return new PagedList<T>
          (
            items,
            pagingParams.PageNumber,
            pagingParams.PageSize,
            totalCount
          );
    }

    public static async Task<IPagedList<T>> ToPagedListAsync<T>(this IEnumerable<T> source, int pageNumber = 1, int pageSize = 10, string sortColumn = "Id", string sortOrder = "DESC")
    {
        
        var totalCount = await Task.FromResult(source.Count());
        var items = await Task.FromResult(source.OrderBy(x => $"{sortColumn} {sortOrder}")
                                .Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize)
                                .ToList());

        return new PagedList<T>(items, pageNumber, pageSize, totalCount);
    }

}