using System;
using SFA.DAS.Roatp.CourseManagement.Application.Courses.Queries.GetProviderAvailableCourseDetails;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Queries.GetStandardInformation;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Helpers;

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
    public bool HasLastDateStarts => CourseViewModelHelper.HasLastDateStarts(LastDateStarts);
    public bool FundingClosing => CourseViewModelHelper.IsFundingClosing(LastDateStarts);
    public string StatusText => CourseViewModelHelper.GetFundingStatusText(FundingClosing);
    public string TagColour => CourseViewModelHelper.GetTagColour(FundingClosing, false);

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
}