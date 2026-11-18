using System.Collections.Generic;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Shared;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.Shared;

public sealed class PaginationViewModelTests
{
    private const int PageSize = PaginationViewModel.DefaultPageSize;
    private const string Ukprn = "123456789";
    private const string ViewStandardsUrl = $"{Ukprn}/standards/";

    private Mock<IUrlHelper> _urlHelperMock = null!;

    [SetUp]
    public void SetUp()
    {
        _urlHelperMock = new Mock<IUrlHelper>();
        _urlHelperMock
            .Setup(x =>
                x.RouteUrl(
                    It.Is<UrlRouteContext>(c =>
                        c.RouteName!.Equals(RouteNames.ViewStandards)
                    )
                )
            )
            .Returns(ViewStandardsUrl);
    }

    [Test]
    public void WhenCreatingPaginationViewModel_AndPageNumberIsProvided_ThenPageNumberIsSet()
    {
        var sut = new PaginationViewModel(
            1,
            1,
            PageSize,
            _urlHelperMock.Object,
            RouteNames.ViewStandards,
            []);

        sut.PageNumber.Should().Be(1);
    }

    [TestCase(1, 0)]
    [TestCase(1, 10)]
    [TestCase(3, 10)]
    public void WhenCreatingPaginationViewModel_AndTotalCountDoesNotExceedPageSize_ThenReturnsEmptyModel(int currentPage, int totalCount)
    {
        var sut = new PaginationViewModel(
            currentPage,
            totalCount,
            PageSize,
            _urlHelperMock.Object,
            RouteNames.ViewStandards,
            []);

        sut.Pages.Should().BeEmpty();
    }

    [TestCase(1, 0, 0)]
    [TestCase(1, 10, 0)]
    [TestCase(1, 70, 0)]
    [TestCase(2, 20, 1)]
    [TestCase(7, 70, 6)]
    public void WhenCreatingPaginationViewModel_AndNotOnFirstPage_ThenModelAddsPreviousLink(int currentPage, int totalCount, int expectedPageNumberInPreviousLink)
    {
        var sut = new PaginationViewModel(
            currentPage,
            totalCount,
            PageSize,
            _urlHelperMock.Object,
            RouteNames.ViewStandards,
            []);

        if (expectedPageNumberInPreviousLink > 0)
        {
            sut.Pages[0].Title.Should().Be(PaginationViewModel.PreviousPageTitle);
            sut.Pages[0].Url.Should().Contain($"PageNumber={expectedPageNumberInPreviousLink}");
        }
        else
        {
            sut.Pages.Should().NotContain(p => p.Title == PaginationViewModel.PreviousPageTitle);
        }
    }

    [TestCase(1, 0, 0)]
    [TestCase(1, 10, 0)]
    [TestCase(2, 20, 0)]
    [TestCase(1, 20, 2)]
    public void WhenCreatingPaginationViewModel_AndNotOnLastPage_ThenModelAddsNextLink(int currentPage, int totalCount, int expectedPageNumberInTheNextLink)
    {
        var sut = new PaginationViewModel(
            currentPage,
            totalCount,
            PageSize,
            _urlHelperMock.Object,
            RouteNames.ViewStandards,
            []);

        if (expectedPageNumberInTheNextLink > 0)
        {
            var lastPage = sut.Pages.Count - 1;
            sut.Pages[lastPage].Title.Should().Be(PaginationViewModel.NextPageTitle);
            sut.Pages[lastPage].Url.Should().Contain($"PageNumber={expectedPageNumberInTheNextLink}");
        }
        else
        {
            sut.Pages.Should().NotContain(p => p.Title == PaginationViewModel.NextPageTitle);
        }
    }

    [Test]
    public void WhenCreatingPaginationViewModel_AndMultiplePagesExist_ThenModelSetsCorrectPageLinks()
    {
        var sut = new PaginationViewModel(
            2,
            30,
            PageSize,
            _urlHelperMock.Object,
            RouteNames.ViewStandards,
            []);

        using (new AssertionScope())
        {
            sut.Pages.Should().HaveCount(5);
            sut.Pages[0].Title.Should().Be(PaginationViewModel.PreviousPageTitle);
            sut.Pages[0].Url.Should().Contain("PageNumber=1");
            sut.Pages[1].Title.Should().Be("1");
            sut.Pages[1].Url.Should().Contain("PageNumber=1");
            sut.Pages[2].Title.Should().Be("2");
            sut.Pages[2].Url.Should().BeNull();
            sut.Pages[3].Title.Should().Be("3");
            sut.Pages[3].Url.Should().Contain("PageNumber=3");
            sut.Pages[4].Title.Should().Be(PaginationViewModel.NextPageTitle);
            sut.Pages[4].Url.Should().Contain("PageNumber=3");
        }
    }

    [Test]
    public void WhenCreatingPaginationViewModel_AndParamsDoNotContainPageNumber_ThenPageNumberIsAddedAndCorrectPageLinksAreSet()
    {
        var sut = new PaginationViewModel(
            1,
            20,
            PageSize,
            _urlHelperMock.Object,
            RouteNames.ViewStandards,
            []);

        using (new AssertionScope())
        {
            sut.Pages.Should().HaveCount(3);
            sut.Pages[1].Title.Should().Be("2");
            sut.Pages[1].Url.Should().Contain("PageNumber=2");
        }
    }

    [Test]
    public void WhenCreatingPaginationViewModel_AndParamsContainPageNumber_ThenPageNumberIsUpdatedInLinks()
    {
        var queryParams = new List<(string, string)>
        {
            (nameof(ViewStandardsRequestModel.SearchTerm), "Beacon"),
            ("PageNumber", "1")
        };

        var sut = new PaginationViewModel(
            1,
            20,
            PageSize,
            _urlHelperMock.Object,
            RouteNames.ViewStandards,
            queryParams);

        sut.Pages.Should().Contain(p =>
            p.Title == "2"
            && p.Url!.Contains("PageNumber=2")
            && p.Url.Contains("SearchTerm=Beacon"));
    }

    [TestCase(1, 10, 0, 0)]
    [TestCase(2, 11, 2, 1)]
    [TestCase(3, 70, 6, 1)]
    [TestCase(4, 70, 6, 2)]
    [TestCase(9, 90, 6, 4)]
    public void WhenCreatingPaginationViewModel_AndGivenCurrentPageAndTotalCount_ThenModelCreatesCorrectNumberOfPageLinks(
        int currentPage,
        int totalCount,
        int expectedPageLinksCount,
        int startPageNumber)
    {
        var sut = new PaginationViewModel(
            currentPage,
            totalCount,
            PageSize,
            _urlHelperMock.Object,
            RouteNames.ViewStandards,
            []);

        sut.Pages.RemoveAll(p =>
            p.Title == PaginationViewModel.PreviousPageTitle
            || p.Title == PaginationViewModel.NextPageTitle);

        using (new AssertionScope())
        {
            sut.Pages.Should().HaveCount(expectedPageLinksCount);
            if (expectedPageLinksCount > 0)
            {
                var pageNumber = startPageNumber;
                for (var index = 0; index < expectedPageLinksCount; index++)
                {
                    sut.Pages[index].Title.Should().Be(pageNumber.ToString());
                    pageNumber++;
                }
            }
        }
    }

    [TestCase(1, 70, 10, 1, 6)]
    [TestCase(2, 70, 10, 1, 6)]
    [TestCase(3, 70, 10, 1, 6)]
    [TestCase(5, 70, 10, 2, 7)]
    [TestCase(6, 70, 10, 2, 7)]
    [TestCase(7, 70, 10, 2, 7)]
    [TestCase(7, 80, 10, 3, 8)]
    [TestCase(20, 100, 10, 5, 10)]
    public void WhenGettingPageRange_AndGivenCurrentPageAndTotalRecords_ThenPageRangeAdjustsCorrectly(
        int currentPage,
        int totalRecords,
        int pageSize,
        int expectedStartPage,
        int expectedEndPage)
    {
        var (startPage, endPage) = PaginationViewModel.GetPageRange(currentPage, totalRecords, pageSize);

        using (new AssertionScope())
        {
            startPage.Should().Be(expectedStartPage);
            endPage.Should().Be(expectedEndPage);
        }
    }

    [Test]
    public void WhenCreatingPageLink_AndUrlIsSet_ThenHasLinkIsTrue()
    {
        var pageLink = new PageLink("Title", "//dummyurl");

        pageLink.HasLink.Should().BeTrue();
    }

    [Test]
    public void WhenCreatingPageLink_AndUrlIsNotSet_ThenHasLinkIsFalse()
    {
        var pageLink = new PageLink("Title", string.Empty);

        pageLink.HasLink.Should().BeFalse();
    }

    [Test]
    public void WhenCreatingPageLink_AndUrlIsNull_ThenHasLinkIsFalse()
    {
        var pageLink = new PageLink("Title", null);

        pageLink.HasLink.Should().BeFalse();
    }

    [Test]
    public void WhenCreatingPageLink_AndUrlIsWhitespace_ThenHasLinkIsFalse()
    {
        var pageLink = new PageLink("Title", "   ");

        pageLink.HasLink.Should().BeFalse();
    }
}
