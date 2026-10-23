using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.Standards;

public class ViewStandardsRequestModelTests
{
    [Test]
    public void HasFundingStatusFilter_WhenNoStatusSelected_ThenIsFalse()
    {
        var requestModel = new ViewStandardsRequestModel();

        requestModel.HasFundingStatusFilter.Should().BeFalse();
        requestModel.HasFilters.Should().BeFalse();
    }

    [Test]
    public void HasFundingStatusFilter_WhenSomeStatusSelected_ThenIsTrue()
    {
        var requestModel = new ViewStandardsRequestModel
        {
            FundingStatus = [FundingStatus.FundingClosing]
        };

        requestModel.HasFundingStatusFilter.Should().BeTrue();
        requestModel.HasFilters.Should().BeTrue();
    }

    [Test]
    public void HasFundingStatusFilter_WhenAllStatusesSelected_ThenIsTrue()
    {
        var requestModel = new ViewStandardsRequestModel
        {
            FundingStatus =
            [
                FundingStatus.FundingClosing,
                FundingStatus.FundingClosed
            ]
        };

        requestModel.HasFundingStatusFilter.Should().BeTrue();
        requestModel.HasFilters.Should().BeTrue();
    }

    [Test]
    public void HasSearchTermFilter_WhenSearchTermProvided_ThenIsTrue()
    {
        var requestModel = new ViewStandardsRequestModel
        {
            SearchTerm = "Paint"
        };

        requestModel.HasSearchTermFilter.Should().BeTrue();
        requestModel.HasFilters.Should().BeTrue();
    }

    [Test]
    public void ToQueryString_WhenFiltersApplied_ThenIncludesSearchTermAndFundingStatuses()
    {
        var requestModel = new ViewStandardsRequestModel
        {
            SearchTerm = " Paint ",
            FundingStatus = [FundingStatus.FundingClosing, FundingStatus.FundingClosed]
        };

        var queryString = requestModel.ToQueryString();

        queryString.Should().Contain((nameof(ViewStandardsRequestModel.SearchTerm), "Paint"));
        queryString.Should().Contain((nameof(ViewStandardsRequestModel.FundingStatus), nameof(FundingStatus.FundingClosing)));
        queryString.Should().Contain((nameof(ViewStandardsRequestModel.FundingStatus), nameof(FundingStatus.FundingClosed)));
    }

    [Test]
    public void ToQueryString_WhenNoFilters_ThenReturnsEmpty()
    {
        var requestModel = new ViewStandardsRequestModel();

        requestModel.ToQueryString().Should().BeEmpty();
    }
}
