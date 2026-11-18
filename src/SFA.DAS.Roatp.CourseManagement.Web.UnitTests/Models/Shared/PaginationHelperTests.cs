using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Shared;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.Shared;

public class PaginationHelperTests
{
    private const string CoursesUrl = "/courses";

    [Test]
    public void WhenPaginating_AndThereAreNoItems_ThenReturnsEmptyItemsAndNoPageLinks()
    {
        var result = PaginationHelper.Paginate(
            new List<int>(), 1, new StubUrlHelper(), "courses", new List<(string, string)>());

        result.PagedItems.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.Pagination.PageNumber.Should().Be(1);
        result.Pagination.Pages.Should().BeEmpty();
    }

    [Test]
    public void WhenPaginating_AndThereAreTenItems_ThenReturnsAllItemsWithoutPageLinks()
    {
        var result = PaginationHelper.Paginate(
            CreateItems(10), 1, new StubUrlHelper(), "courses", new List<(string, string)>());

        result.PagedItems.Should().HaveCount(10);
        result.TotalCount.Should().Be(10);
        result.Pagination.Pages.Should().BeEmpty();
    }

    [Test]
    public void WhenPaginating_AndThereAreMoreThanTenItems_ThenReturnsFirstPageWithNextLink()
    {
        var result = PaginationHelper.Paginate(
            CreateItems(15), 1, new StubUrlHelper(), "courses", new List<(string, string)>());

        result.PagedItems.Should().HaveCount(PaginationViewModel.DefaultPageSize);
        result.PagedItems[0].Should().Be(1);
        result.PagedItems[9].Should().Be(10);
        result.TotalCount.Should().Be(15);
        result.Pagination.PageNumber.Should().Be(1);
        result.Pagination.Pages.Should().NotContain(page => page.Title == PaginationViewModel.PreviousPageTitle);
        result.Pagination.Pages.Should().Contain(page =>
            page.Title == PaginationViewModel.NextPageTitle && page.Url == CoursesUrl + "?PageNumber=2");
    }

    [Test]
    public void WhenPaginating_AndPageNumberIsTwo_ThenReturnsSecondPageWithPreviousLink()
    {
        var result = PaginationHelper.Paginate(
            CreateItems(15), 2, new StubUrlHelper(), "courses", new List<(string, string)>());

        result.PagedItems.Should().HaveCount(5);
        result.PagedItems[0].Should().Be(11);
        result.PagedItems[4].Should().Be(15);
        result.Pagination.PageNumber.Should().Be(2);
        result.Pagination.Pages.Should().Contain(page =>
            page.Title == PaginationViewModel.PreviousPageTitle && page.Url == CoursesUrl + "?PageNumber=1");
        result.Pagination.Pages.Should().NotContain(page => page.Title == PaginationViewModel.NextPageTitle);
    }

    [Test]
    public void WhenPaginating_AndPageNumberIsLessThanOne_ThenReturnsFirstPage()
    {
        var result = PaginationHelper.Paginate(
            CreateItems(15), 0, new StubUrlHelper(), "courses", new List<(string, string)>());

        result.Pagination.PageNumber.Should().Be(1);
        result.PagedItems.Should().HaveCount(PaginationViewModel.DefaultPageSize);
        result.Pagination.Pages.Should().NotContain(page => page.Title == PaginationViewModel.PreviousPageTitle);
    }

    [Test]
    public void WhenPaginating_AndPageNumberExceedsTotalPages_ThenReturnsLastPage()
    {
        var result = PaginationHelper.Paginate(
            CreateItems(15), 99, new StubUrlHelper(), "courses", new List<(string, string)>());

        result.Pagination.PageNumber.Should().Be(2);
        result.PagedItems.Should().HaveCount(5);
        result.PagedItems[0].Should().Be(11);
        result.Pagination.Pages.Should().Contain(page => page.Title == PaginationViewModel.PreviousPageTitle);
        result.Pagination.Pages.Should().NotContain(page => page.Title == PaginationViewModel.NextPageTitle);
    }

    [Test]
    public void WhenPaginating_AndQueryParametersAndFragmentAreProvided_ThenIncludesThemInPageLink()
    {
        var queryParams = new List<(string, string)>
        {
            ("SearchTerm", "electrician & plumber"),
            ("PageNumber", "8")
        };

        var result = PaginationHelper.Paginate(
            CreateItems(15), 1, new StubUrlHelper(), "courses", queryParams, "results");

        result.Pagination.Pages.Should().Contain(page =>
            page.Title == "2"
            && page.Url == CoursesUrl + "?SearchTerm=electrician%20%26%20plumber&PageNumber=2#results");
        queryParams[1].Should().Be(("PageNumber", "8"));
    }

    [TestCase(1, 100, 10, 1, 6)]
    [TestCase(5, 100, 10, 3, 8)]
    [TestCase(10, 100, 10, 5, 10)]
    [TestCase(1, 21, 10, 1, 3)]
    public void WhenGettingPageRange_ThenReturnsExpectedStartAndEndPages(
        int currentPage, int totalRecords, int pageSize, int expectedStartPage, int expectedEndPage)
    {
        var result = PaginationViewModel.GetPageRange(currentPage, totalRecords, pageSize);

        result.startPage.Should().Be(expectedStartPage);
        result.endPage.Should().Be(expectedEndPage);
    }

    private static List<int> CreateItems(int count)
    {
        var items = new List<int>(count);
        for (var item = 1; item <= count; item++)
        {
            items.Add(item);
        }

        return items;
    }

    private sealed class StubUrlHelper : IUrlHelper
    {
        public ActionContext ActionContext { get; } = new();

        public string Action(UrlActionContext actionContext) => string.Empty;

        public string Content(string contentPath) => contentPath ?? string.Empty;

        public bool IsLocalUrl(string url) => true;

        public string Link(string routeName, object values) => string.Empty;

        public string RouteUrl(UrlRouteContext routeContext) => CoursesUrl;
    }
}
