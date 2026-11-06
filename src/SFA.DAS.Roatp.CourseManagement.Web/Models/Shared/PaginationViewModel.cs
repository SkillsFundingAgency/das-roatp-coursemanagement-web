using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.Shared;

public sealed class PaginationViewModel
{
    public const int DefaultPageSize = 10;

    public const string PreviousPageTitle = "« Previous";

    public const string NextPageTitle = "Next »";

    private const string PageNumberQueryName = "PageNumber";

    public int PageNumber { get; set; }

    public List<PageLink> Pages { get; } = [];

    private readonly List<(string, string)> _queryParams;
    private readonly string _routeName;
    private readonly IUrlHelper _urlHelper;
    private readonly string? _filterResultsFragment;

    public PaginationViewModel(
        int pageNumber,
        int totalCount,
        int pageSize,
        IUrlHelper urlHelper,
        string routeName,
        List<(string, string)> queryParams,
        string? filterResultsFragment = null)
    {
        PageNumber = pageNumber;
        _routeName = routeName;
        _queryParams = [.. queryParams];
        _urlHelper = urlHelper;
        _filterResultsFragment = filterResultsFragment;

        if (totalCount == 0)
        {
            return;
        }

        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        if (totalPages <= 1 || PageNumber > totalPages)
        {
            return;
        }

        var (startPage, endPage) = GetPageRange(PageNumber, totalCount, pageSize);

        AddPreviousLinkIfApplicable(PageNumber);
        AddPageLinks(PageNumber, startPage, endPage);
        AddNextLinkIfApplicable(totalPages, PageNumber);
    }

    private void AddPreviousLinkIfApplicable(int currentPage)
    {
        if (currentPage > 1)
        {
            Pages.Add(new(PreviousPageTitle, GetPageLink(currentPage - 1)));
        }
    }

    private void AddPageLinks(int currentPage, int startPage, int endPage)
    {
        for (var pageNumber = startPage; pageNumber <= endPage; pageNumber++)
        {
            string? pageUrl = pageNumber == currentPage ? null : GetPageLink(pageNumber);
            Pages.Add(new(pageNumber.ToString(), pageUrl));
        }
    }

    private void AddNextLinkIfApplicable(int totalPages, int currentPage)
    {
        if (currentPage < totalPages)
        {
            Pages.Add(new(NextPageTitle, GetPageLink(currentPage + 1)));
        }
    }

    private string GetPageLink(int pageNumber)
    {
        var index = _queryParams.FindIndex(q => q.Item1 == PageNumberQueryName);

        if (index >= 0)
        {
            _queryParams[index] = (PageNumberQueryName, pageNumber.ToString());
        }
        else
        {
            _queryParams.Add((PageNumberQueryName, pageNumber.ToString()));
        }

        var queryString = string.Join("&", _queryParams.Select(q =>
            $"{q.Item1}={Uri.EscapeDataString(q.Item2)}"));

        var pageUrl = $"{_urlHelper.RouteUrl(_routeName)}?{queryString}";

        return string.IsNullOrWhiteSpace(_filterResultsFragment)
            ? pageUrl
            : $"{pageUrl}#{_filterResultsFragment}";
    }

    public static (int startPage, int endPage) GetPageRange(int currentPage, int totalRecords, int pageSize)
    {
        var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

        var startPage = Math.Max(1, currentPage - 2);
        var endPage = Math.Min(totalPages, currentPage + 3);

        if (endPage - startPage < 5)
        {
            if (startPage == 1)
            {
                endPage = Math.Min(totalPages, startPage + 5);
            }
            else
            {
                startPage = Math.Max(1, endPage - 5);
            }
        }

        return (startPage, endPage);
    }
}

public record PageLink(string Title, string? Url)
{
    public bool HasLink => !string.IsNullOrWhiteSpace(Url);
}