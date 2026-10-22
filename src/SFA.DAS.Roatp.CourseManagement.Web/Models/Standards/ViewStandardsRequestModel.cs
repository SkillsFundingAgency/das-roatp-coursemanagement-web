using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;

public class ViewStandardsRequestModel
{
    public string SearchTerm { get; set; } = string.Empty;

    public List<FundingStatus> FundingStatus { get; set; } = [];

    public bool HasSearchTermFilter => !string.IsNullOrWhiteSpace(SearchTerm);

    public bool HasFundingStatusFilter => FundingStatus.Count > 0;

    public bool HasFilters => HasSearchTermFilter || HasFundingStatusFilter;

    public List<(string, string)> ToQueryString()
    {
        var queryParams = new List<(string, string)>();

        if (HasSearchTermFilter)
        {
            queryParams.Add((nameof(SearchTerm), SearchTerm.Trim()));
        }

        foreach (var fundingStatus in FundingStatus.Distinct())
        {
            queryParams.Add((nameof(FundingStatus), fundingStatus.ToString()));
        }

        return queryParams;
    }
}
