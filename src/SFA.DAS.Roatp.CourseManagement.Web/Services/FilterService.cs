#nullable enable annotations

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFA.DAS.DfESignIn.Auth.Extensions;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Filters.Abstract;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Filters.FilterComponents;

namespace SFA.DAS.Roatp.CourseManagement.Web.Services;

public class FilterService
{
    public const string ClearFilter = "clearFilter";

    public const string SearchTermSectionHeading = "Provider name";
    public const string SearchTermSectionSubHeading = "Search by name or UKPRN";
    public const string CourseNameSectionHeading = "Course name";
    public const string CourseNameSectionSubHeading = "Search by name or LARS code";
    public const string FundingStatusSectionHeading = "Funding status";
    public const string LearningTypeSectionHeading = "Training type";

    public enum FilterComponentType
    {
        CheckboxList,
        TextBox
    }

    public enum FilterType
    {
        SearchTerm,
        FundingStatus
    }

    public static Dictionary<FilterType, string> ClearFilterSectionHeadings { get; } = new()
    {
        { FilterType.SearchTerm, SearchTermSectionHeading },
        { FilterType.FundingStatus, FundingStatusSectionHeading }
    };

    public static FilterSection CreateInputFilterSection(
        string id,
        string heading,
        string subHeading,
        string filterFor,
        string? inputValue)
        => new TextBoxFilterSectionViewModel
        {
            Id = id,
            For = filterFor,
            Heading = heading,
            SubHeading = subHeading,
            InputValue = inputValue ?? string.Empty
        };

    public static FilterSection CreateCheckboxListFilterSection(
        string id,
        string filterFor,
        string heading,
        string? subHeading,
        List<FilterItemViewModel> items)
        => new CheckboxListFilterSectionViewModel
        {
            Id = id,
            For = filterFor,
            Heading = heading,
            SubHeading = subHeading ?? string.Empty,
            Items = items
        };

    public static IReadOnlyList<ClearFilterSectionViewModel> CreateClearFilterSections(
        Dictionary<FilterType, IEnumerable<string>> selectedFilters,
        string clearFiltersBaseUrl,
        bool useDisplayText = false,
        string? filterResultsFragment = null,
        Dictionary<FilterType, string>? sectionHeadingOverrides = null)
    {
        if (selectedFilters.Count == 0)
        {
            return [];
        }

        List<ClearFilterSectionViewModel> clearFilterSections = [];

        foreach (var filter in selectedFilters)
        {
            if (!filter.Value.Any())
            {
                continue;
            }

            var title = sectionHeadingOverrides?.GetValueOrDefault(filter.Key)
                        ?? ClearFilterSectionHeadings[filter.Key];

            clearFilterSections.Add(new ClearFilterSectionViewModel
            {
                FilterType = filter.Key,
                Title = title,
                Items = filter.Value.Select(value => new ClearFilterItemViewModel
                {
                    DisplayText = GetDisplayText(filter.Key, value, useDisplayText),
                    ClearLink = BuildClearLink(
                        clearFiltersBaseUrl,
                        filter.Key,
                        value,
                        selectedFilters,
                        filterResultsFragment)
                }).ToList()
            });
        }

        return clearFilterSections;
    }

    public static void AddSelectedFilter(
        Dictionary<FilterType, IEnumerable<string>> filters,
        FilterType filterType,
        string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            filters[filterType] = [value];
        }
    }

    public static void AddSelectedFilter(
        Dictionary<FilterType, IEnumerable<string>> filters,
        FilterType filterType,
        IEnumerable<string> values)
    {
        var valuesToAdd = values.Where(value => !string.IsNullOrWhiteSpace(value)).ToList();
        if (valuesToAdd.Count > 0)
        {
            filters[filterType] = valuesToAdd;
        }
    }

    private static string GetDisplayText(FilterType filterType, string value, bool useDisplayText)
    {
        if (!useDisplayText)
        {
            return value;
        }

        return filterType switch
        {
            FilterType.FundingStatus when Enum.TryParse<FundingStatus>(value, out var status)
                => status.GetDescription(),
            _ => value
        };
    }

    private static string BuildClearLink(
        string clearFiltersBaseUrl,
        FilterType filterType,
        string value,
        Dictionary<FilterType, IEnumerable<string>> queryParams,
        string? filterResultsFragment)
    {
        var queryString = BuildQueryWithoutValue(filterType, value, queryParams);
        var clearLink = string.IsNullOrEmpty(queryString)
            ? clearFiltersBaseUrl
            : $"{clearFiltersBaseUrl}{queryString}";

        return string.IsNullOrWhiteSpace(filterResultsFragment)
            ? clearLink
            : $"{clearLink}#{filterResultsFragment}";
    }

    private static string BuildQueryWithoutValue(
        FilterType filterType,
        string value,
        Dictionary<FilterType, IEnumerable<string>> queryParams)
    {
        var queryBuilder = new StringBuilder();

        foreach (var param in queryParams)
        {
            if (!param.Value.Any())
            {
                continue;
            }

            var values = param.Key == filterType
                ? param.Value.Where(v => v != value)
                : param.Value;

            foreach (var val in values)
            {
                AppendQueryParam(queryBuilder, param.Key, val);
            }
        }

        return queryBuilder.Length > 0 ? queryBuilder.ToString() : string.Empty;
    }

    private static void AppendQueryParam(
        StringBuilder builder,
        FilterType key,
        string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        var encodedValue = Uri.EscapeDataString(value);

        builder
            .Append(builder.Length > 0 ? '&' : '?')
            .Append(key.ToString())
            .Append('=')
            .Append(encodedValue);
    }
}
