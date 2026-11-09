using System;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;

public class ShortCourseViewModel
{
    public int ProviderCourseId { get; set; }
    public string CourseName { get; set; }
    public int Level { get; set; }
    public string CourseDisplayName { get; set; }
    public string LarsCode { get; set; }
    public string StandardUrl { get; set; }
    public bool HasLocation { get; set; }
    public bool HasOnlineDeliveryOption { get; set; }
    public bool StandardRequiresMoreInfo => SetMissingInfo();
    public DateTime? LastDateStarts { get; set; }
    public bool HasLastDateStarts => SetHasLastDateStarts();
    public bool FundingClosing => SetFundingClosing();
    public bool FundingClosed => SetFundingClosed();
    public string TagColour => SetTagColour();

    public static implicit operator ShortCourseViewModel(Standard source)
    {
        return new ShortCourseViewModel
        {
            ProviderCourseId = source.ProviderCourseId,
            CourseName = source.CourseName,
            Level = source.Level,
            CourseDisplayName = source.DisplayName,
            LarsCode = source.LarsCode,
            HasLocation = source.HasLocations,
            HasOnlineDeliveryOption = source.HasOnlineDeliveryOption,
            LastDateStarts = source.LastDateStarts
        };
    }

    private bool SetMissingInfo()
    {
        return (!HasLocation && !HasOnlineDeliveryOption);
    }

    private bool SetHasLastDateStarts()
    {
        return LastDateStarts != null;
    }

    private bool SetFundingClosing()
    {
        return HasLastDateStarts && LastDateStarts.Value >= DateTime.UtcNow.Date;
    }

    private bool SetFundingClosed()
    {
        return HasLastDateStarts && LastDateStarts.Value < DateTime.UtcNow.Date;
    }

    private string SetTagColour()
    {
        if (FundingClosing)
        {
            return "govuk-tag govuk-tag--orange";
        }

        if (FundingClosed)
        {
            return "govuk-tag govuk-tag--grey";
        }

        return string.Empty;
    }
}
