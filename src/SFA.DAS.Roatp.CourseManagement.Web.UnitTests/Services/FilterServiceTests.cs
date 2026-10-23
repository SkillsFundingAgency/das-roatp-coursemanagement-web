using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Filters.FilterComponents;
using static SFA.DAS.Roatp.CourseManagement.Web.Services.FilterService;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Services;

[TestFixture]
public class FilterServiceTests
{
    private const string Ukprn = "123456";
    private const string ClearFiltersBaseUrl = $"{Ukprn}/restricted-courses";


    [Test]
    public void CreateInputFilterSection_ThenReturnsTextBoxSectionWithValues()
    {
        var section = CreateInputFilterSection(
            "search-term-input",
            SearchTermSectionHeading,
            SearchTermSectionSubHeading,
            nameof(FilterType.SearchTerm),
            "Beacon");

        section.Should().BeOfType<TextBoxFilterSectionViewModel>();
        section.Id.Should().Be("search-term-input");
        section.For.Should().Be(nameof(FilterType.SearchTerm));
        section.Heading.Should().Be(SearchTermSectionHeading);
        section.SubHeading.Should().Be(SearchTermSectionSubHeading);
        section.FilterComponentType.Should().Be(FilterComponentType.TextBox);
        ((TextBoxFilterSectionViewModel)section).InputValue.Should().Be("Beacon");
    }

    [Test]
    public void CreateInputFilterSection_AndInputValueIsNull_ThenUsesEmptyString()
    {
        var section = (TextBoxFilterSectionViewModel)CreateInputFilterSection(
            "id",
            "heading",
            "subHeading",
            "for",
            null);

        section.InputValue.Should().BeEmpty();
    }

    [Test]
    public void CreateCheckboxListFilterSection_ThenReturnsCheckboxSectionWithItems()
    {
        var items = new List<FilterItemViewModel>
        {
            new() { Value = "OpenToNewStarts", DisplayText = "Open to new starts", IsSelected = true }
        };

        var section = CreateCheckboxListFilterSection(
            "delivery-status-filter",
            nameof(FilterType.FundingStatus),
            FundingStatusSectionHeading,
            "Hint",
            items);

        section.Should().BeOfType<CheckboxListFilterSectionViewModel>();
        section.Id.Should().Be("delivery-status-filter");
        section.For.Should().Be(nameof(FilterType.FundingStatus));
        section.Heading.Should().Be(FundingStatusSectionHeading);
        section.SubHeading.Should().Be("Hint");
        section.FilterComponentType.Should().Be(FilterComponentType.CheckboxList);
        ((CheckboxListFilterSectionViewModel)section).Items.Should().BeEquivalentTo(items);
    }

    [Test]
    public void CreateCheckboxListFilterSection_AndSubHeadingIsNull_ThenUsesEmptyString()
    {
        var section = CreateCheckboxListFilterSection(
            "id",
            "for",
            "heading",
            null,
            []);

        section.SubHeading.Should().BeEmpty();
    }

    [Test]
    public void CreateClearFilterSections_AndNoSelectedFilters_ThenReturnsEmpty()
    {
        var result = CreateClearFilterSections([], ClearFiltersBaseUrl);

        result.Should().BeEmpty();
    }

    [Test]
    public void CreateClearFilterSections_AndFilterHasNoValues_ThenSkipsSection()
    {
        var selectedFilters = new Dictionary<FilterType, IEnumerable<string>>
        {
            [FilterType.SearchTerm] = []
        };

        var result = CreateClearFilterSections(selectedFilters, ClearFiltersBaseUrl);

        result.Should().BeEmpty();
    }

    [Test]
    public void CreateClearFilterSections_AndSingleFilter_ThenClearLinkIsBaseUrl()
    {
        var selectedFilters = new Dictionary<FilterType, IEnumerable<string>>
        {
            [FilterType.SearchTerm] = ["Beacon"]
        };

        var result = CreateClearFilterSections(selectedFilters, ClearFiltersBaseUrl);

        result.Should().ContainSingle();
        result[0].Title.Should().Be(SearchTermSectionHeading);
        result[0].FilterType.Should().Be(FilterType.SearchTerm);
        result[0].Items.Single().DisplayText.Should().Be("Beacon");
        result[0].Items.Single().ClearLink.Should().Be(ClearFiltersBaseUrl);
    }

    [Test]
    public void CreateClearFilterSections_AndMultipleFilters_ThenClearLinkKeepsOtherFilters()
    {
        var selectedFilters = new Dictionary<FilterType, IEnumerable<string>>
        {
            [FilterType.SearchTerm] = ["Beacon"],
            [FilterType.FundingStatus] = ["FundingClosing", "FundingClosed"]
        };

        var result = CreateClearFilterSections(selectedFilters, ClearFiltersBaseUrl, useDisplayText: true);

        result.Should().HaveCount(2);

        var clearProviderLink = result
            .Single(section => section.FilterType == FilterType.SearchTerm)
            .Items.Single().ClearLink;

        clearProviderLink.Should().Be(
            $"{ClearFiltersBaseUrl}?FundingStatus=FundingClosing&FundingStatus=FundingClosed");

        var clearOpenLink = result
            .Single(section => section.FilterType == FilterType.FundingStatus)
            .Items.Single(item => item.DisplayText == "Funding closing for new starts")
            .ClearLink;

        clearOpenLink.Should().Be($"{ClearFiltersBaseUrl}?SearchTerm=Beacon&FundingStatus=FundingClosed");
    }

    [Test]
    public void CreateClearFilterSections_AndValueNeedsEncoding_ThenEncodesQueryValue()
    {
        var selectedFilters = new Dictionary<FilterType, IEnumerable<string>>
        {
            [FilterType.SearchTerm] = ["Beacon & Co"],
            [FilterType.FundingStatus] = ["FundingClosing"]
        };

        var clearDeliveryLink = CreateClearFilterSections(selectedFilters, ClearFiltersBaseUrl)
            .Single(section => section.FilterType == FilterType.FundingStatus)
            .Items.Single().ClearLink;

        clearDeliveryLink.Should().Be($"{ClearFiltersBaseUrl}?SearchTerm=Beacon%20%26%20Co");
    }

    [Test]
    public void CreateClearFilterSections_AndSelectedFiltersIncludesEmptyFilter_ThenIgnoresEmptyFilterInClearLinks()
    {
        var selectedFilters = new Dictionary<FilterType, IEnumerable<string>>
        {
            [FilterType.SearchTerm] = ["Beacon"],
            [FilterType.FundingStatus] = []
        };

        var result = CreateClearFilterSections(selectedFilters, ClearFiltersBaseUrl);

        result.Should().ContainSingle();
        result[0].FilterType.Should().Be(FilterType.SearchTerm);
        result[0].Items.Single().ClearLink.Should().Be(ClearFiltersBaseUrl);
    }

    [Test]
    public void CreateClearFilterSections_AndSelectedFiltersIncludesWhitespace_ThenOmitsWhitespaceFromClearLink()
    {
        var selectedFilters = new Dictionary<FilterType, IEnumerable<string>>
        {
            [FilterType.SearchTerm] = ["Beacon"],
            [FilterType.FundingStatus] = ["FundingClosing", "   "]
        };

        var clearProviderLink = CreateClearFilterSections(selectedFilters, ClearFiltersBaseUrl)
            .Single(section => section.FilterType == FilterType.SearchTerm)
            .Items.Single().ClearLink;

        clearProviderLink.Should().Be($"{ClearFiltersBaseUrl}?FundingStatus=FundingClosing");
    }

    [Test]
    public void CreateClearFilterSections_AndSectionHeadingOverridesProvided_ThenUsesOverrideTitle()
    {
        var selectedFilters = new Dictionary<FilterType, IEnumerable<string>>
        {
            [FilterType.SearchTerm] = ["Paint"]
        };

        var result = CreateClearFilterSections(
            selectedFilters,
            ClearFiltersBaseUrl,
            sectionHeadingOverrides: new Dictionary<FilterType, string>
            {
                [FilterType.SearchTerm] = CourseNameSectionHeading
            });

        result.Should().ContainSingle();
        result[0].Title.Should().Be(CourseNameSectionHeading);
    }

    [Test]
    public void AddSelectedFilter_WithSingleValueAndValueIsBlank_ThenDoesNotAdd()
    {
        var filters = new Dictionary<FilterType, IEnumerable<string>>();

        AddSelectedFilter(filters, FilterType.SearchTerm, "  ");

        filters.Should().BeEmpty();
    }

    [Test]
    public void AddSelectedFilter_WithSingleValueAndValueIsNotBlank_ThenAddsValue()
    {
        var filters = new Dictionary<FilterType, IEnumerable<string>>();

        AddSelectedFilter(filters, FilterType.SearchTerm, "Beacon");

        filters[FilterType.SearchTerm].Should().BeEquivalentTo("Beacon");
    }

    [Test]
    public void AddSelectedFilter_WithMultipleValues_AndAllValuesAreBlank_ThenDoesNotAdd()
    {
        var filters = new Dictionary<FilterType, IEnumerable<string>>();

        AddSelectedFilter(filters, FilterType.FundingStatus, ["", "  "]);

        filters.Should().BeEmpty();
    }

    [Test]
    public void AddSelectedFilter_WithMultipleValuesAndSomeValuesAreBlank_ThenAddsNonBlankValues()
    {
        var filters = new Dictionary<FilterType, IEnumerable<string>>();

        AddSelectedFilter(filters, FilterType.FundingStatus, ["FundingClosing", "", "FundingClosed"]);

        filters[FilterType.FundingStatus].Should().BeEquivalentTo("FundingClosing", "FundingClosed");
    }
}
