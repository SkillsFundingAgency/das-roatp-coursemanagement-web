using System;
using SFA.DAS.DfESignIn.Auth.Extensions;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;

namespace SFA.DAS.Roatp.CourseManagement.Web.Helpers;

public static class CourseViewModelHelper
{
    public static bool HasLastDateStarts(DateTime? lastDateStarts) =>
        lastDateStarts.HasValue;

    public static bool IsFundingClosing(DateTime? lastDateStarts) =>
        lastDateStarts.HasValue && lastDateStarts.Value >= DateTime.UtcNow.Date;

    public static bool IsFundingClosed(DateTime? lastDateStarts) =>
        lastDateStarts.HasValue && lastDateStarts.Value < DateTime.UtcNow.Date;

    public static string GetFundingStatusText(bool fundingClosing) =>
        fundingClosing
            ? FundingStatus.FundingClosing.GetDescription()
            : string.Empty;

    public static string GetTagColour(bool fundingClosing, bool fundingClosed)
    {
        if (fundingClosing)
        {
            return "govuk-tag govuk-tag--orange";
        }

        if (fundingClosed)
        {
            return "govuk-tag govuk-tag--grey";
        }

        return string.Empty;
    }
}
