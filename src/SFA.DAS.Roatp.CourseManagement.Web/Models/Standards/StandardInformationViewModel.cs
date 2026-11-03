using System;
using SFA.DAS.DfESignIn.Auth.Extensions;
using SFA.DAS.Roatp.CourseManagement.Application.Courses.Queries.GetProviderAvailableCourseDetails;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Queries.GetStandardInformation;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;

public class StandardInformationViewModel
{
    public string LarsCode { get; set; }
    public string CourseName { get; set; }
    public int Level { get; set; }
    public string IfateReferenceNumber { get; set; }
    public string Sector { get; set; }
    public string RegulatorName { get; set; }
    public LearningType LearningType { get; set; }
    public string CourseDisplayName => $"{CourseName} (level {Level})";
    public bool IsRegulatedForProvider { get; set; }
    public DateTime? LastDateStarts { get; set; }
    public bool HasLastDateStarts => SetHasLastDateStarts();
    public bool FundingClosing => SetFundingClosing();
    public string StatusText => SetStatusText();
    public string TagColour => SetTagColour();

    public static implicit operator StandardInformationViewModel(GetStandardInformationQueryResult source)
        => new()
        {
            LarsCode = source.LarsCode,
            IfateReferenceNumber = source.IfateReferenceNumber,
            LearningType = source.LearningType,
            Sector = source.Route,
            CourseName = source.Title,
            Level = source.Level,
            RegulatorName = source.ApprovalBody,
            IsRegulatedForProvider = source.IsRegulatedForProvider
        };

    public static implicit operator StandardInformationViewModel(GetProviderAvailableCourseDetailsQueryResult source)
        => new()
        {
            LarsCode = source.LarsCode,
            IfateReferenceNumber = source.IfateReferenceNumber,
            LearningType = source.LearningType,
            Sector = source.Sector,
            CourseName = source.Title,
            Level = source.Level,
            RegulatorName = source.ApprovalBody,
            IsRegulatedForProvider = source.IsRegulatedForProvider,
            LastDateStarts = source.LastDateStarts
        };

    private bool SetHasLastDateStarts()
    {
        return LastDateStarts != null;
    }

    private bool SetFundingClosing()
    {
        return HasLastDateStarts && LastDateStarts.Value > DateTime.UtcNow.Date;
    }

    private string SetStatusText()
    {
        if (FundingClosing)
        {
            return FundingStatus.FundingClosing.GetDescription();
        }

        return string.Empty;
    }

    private string SetTagColour()
    {
        if (FundingClosing)
        {
            return "govuk-tag govuk-tag--orange";
        }

        return string.Empty;
    }
}