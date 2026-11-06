using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.Shared;

public static class PaginationHelper
{
    public static (List<T> PagedItems, int TotalCount, PaginationViewModel Pagination) Paginate<T>(
        List<T> filteredItems,
        int requestedPageNumber,
        IUrlHelper urlHelper,
        string routeName,
        List<(string, string)> queryParams,
        string? filterResultsFragment = null)
    {
        var pageSize = PaginationViewModel.DefaultPageSize;
        var totalCount = filteredItems.Count;
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));
        var pageNumber = requestedPageNumber < 1 ? 1 : Math.Min(requestedPageNumber, totalPages);

        var pagedItems = filteredItems
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var pagination = new PaginationViewModel(
            pageNumber,
            totalCount,
            pageSize,
            urlHelper,
            routeName,
            queryParams,
            filterResultsFragment);

        return (pagedItems, totalCount, pagination);
    }
}