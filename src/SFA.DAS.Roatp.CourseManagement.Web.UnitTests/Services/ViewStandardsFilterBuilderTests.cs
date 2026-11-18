using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.DfESignIn.Auth.Extensions;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Filters.FilterComponents;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using static SFA.DAS.Roatp.CourseManagement.Web.Services.FilterService;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Services;

public class ViewStandardsFilterBuilderTests
{
    private const int Ukprn = 12345678;
    private const string ViewStandardsUrl = "/view-standards";

    [Test]
    public void WhenApplyingSearchTermFilter_ThenMatchesCourseDisplayName()
    {
        var filtered = ViewStandardsFilterBuilder.ApplyFilters(
            CreateCourses(),
            new ViewStandardsRequestModel { SearchTerm = "cleaning" });

        filtered.Should().ContainSingle(course => course.LarsCode == "124");
    }

    [Test]
    public void WhenApplyingSearchTermFilter_ThenMatchesLarsCode()
    {
        var filtered = ViewStandardsFilterBuilder.ApplyFilters(
            CreateCourses(),
            new ViewStandardsRequestModel { SearchTerm = "124" });

        filtered.Should().ContainSingle(course => course.LarsCode == "124");
    }

    [Test]
    public void WhenApplyingSearchTermFilter_AndLarsCodeIsPartial_ThenDoesNotMatch()
    {
        var filtered = ViewStandardsFilterBuilder.ApplyFilters(
            CreateCourses(),
            new ViewStandardsRequestModel { SearchTerm = "12" });

        filtered.Should().BeEmpty();
    }

    [Test]
    public void WhenApplyingFundingClosingFilter_ThenReturnsCoursesWithPastLastDateStarts()
    {
        var filtered = ViewStandardsFilterBuilder.ApplyFilters(
            CreateCourses(),
            new ViewStandardsRequestModel
            {
                FundingStatus = [FundingStatus.FundingClosing]
            });

        filtered.Should().ContainSingle(course => course.LarsCode == "124");
    }

    [Test]
    public void WhenApplyingFundingClosedFilter_ThenReturnsCoursesWithCurrentOrFutureLastDateStarts()
    {
        var filtered = ViewStandardsFilterBuilder.ApplyFilters(
            CreateCourses(),
            new ViewStandardsRequestModel
            {
                FundingStatus = [FundingStatus.FundingClosed]
            });

        filtered.Should().ContainSingle(course => course.LarsCode == "163");
    }

    [Test]
    public void WhenApplyingBothFundingStatusFilters_ThenReturnsCoursesWithEitherStatus()
    {
        var filtered = ViewStandardsFilterBuilder.ApplyFilters(
            CreateCourses(),
            new ViewStandardsRequestModel
            {
                FundingStatus =
                [
                    FundingStatus.FundingClosing,
                    FundingStatus.FundingClosed
                ]
            });

        filtered.Should().HaveCount(2);
        filtered.Select(course => course.LarsCode)
            .Should().BeEquivalentTo("124", "163");
    }

    [Test]
    public void WhenApplyingNoFilters_ThenReturnsAllCourses()
    {
        var courses = CreateCourses();

        var filtered = ViewStandardsFilterBuilder.ApplyFilters(
            courses,
            new ViewStandardsRequestModel());

        filtered.Should().BeEquivalentTo(courses);
    }

    [Test]
    public void WhenCreatingFiltersViewModel_AndSearchTermHasSurroundingWhitespace_ThenTrimsClearFilterText()
    {
        var filters = CreateFiltersViewModel(
            new ViewStandardsRequestModel { SearchTerm = "  Beacon  " });

        filters.ClearFilterSections
            .Single(section => section.FilterType == FilterType.SearchTerm)
            .Items.Single()
            .DisplayText.Should().Be("Beacon");
    }

    [Test]
    public void WhenCreatingFiltersViewModel_ThenSetsRouteUkprnAndResultsFragment()
    {
        var filters = CreateFiltersViewModel(new ViewStandardsRequestModel());

        using (new AssertionScope())
        {
            filters.Route.Should().Be(RouteNames.ViewStandards);
            filters.Ukprn.Should().Be(Ukprn);
            filters.FilterResultsFragment.Should()
                .Be(ViewStandardsFilterBuilder.ViewStandardsFilterResultsFragment);
        }
    }

    [Test]
    public void WhenCreatingFiltersViewModel_ThenBuildsSearchAndFundingStatusSections()
    {
        var filters = CreateFiltersViewModel(new ViewStandardsRequestModel());

        using (new AssertionScope())
        {
            filters.FilterSections.Should().HaveCount(2);

            var searchSection = filters.FilterSections[0]
                .Should().BeOfType<TextBoxFilterSectionViewModel>().Subject;
            searchSection.Heading.Should().Be(CourseNameSectionHeading);
            searchSection.SubHeading.Should().Be(CourseNameSectionSubHeading);

            var fundingSection = filters.FilterSections[1]
                .Should().BeOfType<CheckboxListFilterSectionViewModel>().Subject;
            fundingSection.Heading.Should().Be(FundingStatusSectionHeading);
            fundingSection.Items.Should().HaveCount(2);
        }
    }

    [Test]
    public void WhenCreatingFiltersViewModel_ThenMarksSelectedFundingStatus()
    {
        var filters = CreateFiltersViewModel(new ViewStandardsRequestModel
        {
            FundingStatus = [FundingStatus.FundingClosing]
        });

        var fundingSection = filters.FilterSections[1]
            .Should().BeOfType<CheckboxListFilterSectionViewModel>().Subject;

        using (new AssertionScope())
        {
            fundingSection.Items
                .Single(item => item.Value == nameof(FundingStatus.FundingClosing))
                .IsSelected.Should().BeTrue();

            fundingSection.Items
                .Single(item => item.Value == nameof(FundingStatus.FundingClosed))
                .IsSelected.Should().BeFalse();
        }
    }

    [Test]
    public void WhenCreatingFiltersViewModel_AndMultipleFiltersAreSelected_ThenBuildsClearLinksForRemainingFilters()
    {
        var filters = CreateFiltersViewModel(new ViewStandardsRequestModel
        {
            SearchTerm = "Beacon",
            FundingStatus =
            [
                FundingStatus.FundingClosing,
                FundingStatus.FundingClosed
            ]
        });

        using (new AssertionScope())
        {
            filters.ClearFilterSections.Should().HaveCount(2);

            var clearSearchLink = filters.ClearFilterSections
                .Single(section => section.FilterType == FilterType.SearchTerm)
                .Items.Single()
                .ClearLink;

            clearSearchLink.Should().Be(
                $"{ViewStandardsUrl}?FundingStatus=FundingClosing&FundingStatus=FundingClosed" +
                $"#{ViewStandardsFilterBuilder.ViewStandardsFilterResultsFragment}");

            var clearClosingStatusLink = filters.ClearFilterSections
                .Single(section => section.FilterType == FilterType.FundingStatus)
                .Items.Single(item =>
                    item.DisplayText == FundingStatus.FundingClosing.GetDescription())
                .ClearLink;

            clearClosingStatusLink.Should().Be(
                $"{ViewStandardsUrl}?SearchTerm=Beacon&FundingStatus=FundingClosed" +
                $"#{ViewStandardsFilterBuilder.ViewStandardsFilterResultsFragment}");
        }
    }

    [Test]
    public void WhenCreatingFiltersViewModel_AndClearingTheLastFilter_ThenClearLinkIsBaseUrlWithFragment()
    {
        var filters = CreateFiltersViewModel(new ViewStandardsRequestModel
        {
            SearchTerm = "Beacon"
        });

        filters.ClearFilterSections
            .Single()
            .Items.Single()
            .ClearLink.Should().Be(
                $"{ViewStandardsUrl}#{ViewStandardsFilterBuilder.ViewStandardsFilterResultsFragment}");
    }

    private static FiltersViewModel CreateFiltersViewModel(ViewStandardsRequestModel requestModel) =>
        ViewStandardsFilterBuilder.CreateFiltersViewModel(
            requestModel,
            Ukprn,
            CreateUrlHelper().Object);

    private static List<StandardViewModel> CreateCourses() =>
    [
        new()
        {
            LarsCode = "124",
            CourseDisplayName = "Cleaning hygiene operative",
            LastDateStarts = DateTime.UtcNow.Date.AddDays(1)
        },
        new()
        {
            LarsCode = "163",
            CourseDisplayName = "Beacon manager",
            LastDateStarts = DateTime.UtcNow.Date.AddDays(-1)
        },
        new()
        {
            LarsCode = "999",
            CourseDisplayName = "Paint unit",
            LastDateStarts = null
        }
    ];

    private static Mock<IUrlHelper> CreateUrlHelper()
    {
        var urlHelper = new Mock<IUrlHelper>();
        urlHelper
            .Setup(helper => helper.RouteUrl(It.IsAny<UrlRouteContext>()))
            .Returns((UrlRouteContext context) =>
            {
                context.RouteName.Should().Be(RouteNames.ViewStandards);
                return ViewStandardsUrl;
            });

        return urlHelper;
    }
}
