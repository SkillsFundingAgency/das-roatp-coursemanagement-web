using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.DfESignIn.Auth.Extensions;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Filters.FilterComponents;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using static SFA.DAS.Roatp.CourseManagement.Web.Services.FilterService;

namespace SFA.DAS.Roatp.CourseManagement.Web.Services;

public static class ViewStandardsFilterBuilder
{
    public const string ViewStandardsFilterResultsFragment = "view-standards-results";

    private const string SearchTermInputId = "search-term-input";
    private const string FundingStatusFilterId = "funding-status-filter";
    private const string FundingClosingDesrcription = "Courses you can enrol new government funded learners on, but only until the last start date";
    private const string FundingClosedDesrcription = "Courses you can't enrol new learners on, advertise on Find apprenticeship training or claim funding for";
    private const string FundingSectionSubHeading = "Funding statuses are set by the Apprenticeship Service. You cannot change them.";

    public static FiltersViewModel CreateFiltersViewModel(
        ViewStandardsRequestModel requestModel,
        int ukprn,
        IUrlHelper urlHelper)
    {
        var selectedFilters = new Dictionary<FilterType, IEnumerable<string>>();
        AddSelectedFilter(selectedFilters, FilterType.SearchTerm, requestModel.SearchTerm?.Trim());

        if (requestModel.HasFundingStatusFilter)
        {
            AddSelectedFilter(
                selectedFilters,
                FilterType.FundingStatus,
                requestModel.FundingStatus.Distinct().Select(type => type.ToString()));
        }

        var sectionHeadingOverrides = new Dictionary<FilterType, string>
        {
            [FilterType.SearchTerm] = CourseNameSectionHeading
        };

        var clearFiltersBaseUrl = urlHelper.RouteUrl(RouteNames.ViewStandards, new { ukprn });

        return new FiltersViewModel
        {
            Route = RouteNames.ViewStandards,
            Ukprn = ukprn,
            FilterResultsFragment = ViewStandardsFilterResultsFragment,
            FilterSections =
            [
                CreateInputFilterSection(
                    SearchTermInputId,
                    CourseNameSectionHeading,
                    CourseNameSectionSubHeading,
                    nameof(FilterType.SearchTerm),
                    requestModel.SearchTerm),
                CreateCheckboxListFilterSection(
                    FundingStatusFilterId,
                    nameof(FilterType.FundingStatus),
                    FundingStatusSectionHeading,
                    FundingSectionSubHeading,
                    BuildFundingStatusItems(requestModel))
            ],
            ClearFilterSections = CreateClearFilterSections(
                selectedFilters,
                clearFiltersBaseUrl,
                useDisplayText: true,
                ViewStandardsFilterResultsFragment,
                sectionHeadingOverrides)
        };
    }

    public static List<StandardViewModel> ApplyFilters(
        List<StandardViewModel> courses,
        ViewStandardsRequestModel requestModel)
    {
        var filtered = courses;

        if (requestModel.HasSearchTermFilter)
        {
            var searchTerm = requestModel.SearchTerm.Trim();
            filtered = filtered.Where(course =>
                course.CourseDisplayName
                    .Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                || course.LarsCode.Equals(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if (requestModel.HasFundingStatusFilter)
        {
            var selectedStatuses = requestModel.FundingStatus.Distinct().ToHashSet();
            filtered = filtered.Where(course =>
                (selectedStatuses.Contains(FundingStatus.FundingClosing) && course.FundingClosing) ||
                (selectedStatuses.Contains(FundingStatus.FundingClosed) && course.FundingClosed))
                .ToList();
        }

        return filtered;
    }

    private static List<FilterItemViewModel> BuildFundingStatusItems(ViewStandardsRequestModel requestModel)
        =>
        [
            CreateFundingStatusItem(FundingStatus.FundingClosing, FundingClosingDesrcription, requestModel),
            CreateFundingStatusItem(FundingStatus.FundingClosed, FundingClosedDesrcription, requestModel)
        ];

    private static FilterItemViewModel CreateFundingStatusItem(
        FundingStatus fundingStatus,
        string description,
        ViewStandardsRequestModel requestModel)
        => new()
        {
            Value = fundingStatus.ToString(),
            DisplayText = fundingStatus.GetDescription(),
            DisplayDescription = description,
            IsSelected = requestModel.FundingStatus.Contains(fundingStatus)
        };
}
