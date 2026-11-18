using System;
using SFA.DAS.Roatp.CourseManagement.Application.Courses.Queries.GetProviderAvailableCourseDetails;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Queries.GetStandardInformation;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Helpers;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;

public class ShortCourseInformationViewModel
{
    public string LarsCode { get; set; }
    public string CourseName { get; set; }
    public int Level { get; set; }
    public string IfateReferenceNumber { get; set; }
    public string Sector { get; set; }
    public string RegulatorName { get; set; }
    public LearningType LearningType { get; set; }
    public bool IsRegulatedForProvider { get; set; }
    public int Duration { get; set; }
    public DurationUnits DurationUnits { get; set; }
    public string CourseDisplayName => $"{CourseName} (level {Level})";
    public DateTime? LastDateStarts { get; set; }
    public bool HasLastDateStarts => CourseViewModelHelper.HasLastDateStarts(LastDateStarts);
    public bool FundingClosing => CourseViewModelHelper.IsFundingClosing(LastDateStarts);
    public string StatusText => CourseViewModelHelper.GetFundingStatusText(FundingClosing);
    public string TagColour => CourseViewModelHelper.GetTagColour(FundingClosing, false);

    public static implicit operator ShortCourseInformationViewModel(GetStandardInformationQueryResult source)
        => new()
        {
            LarsCode = source.LarsCode,
            IfateReferenceNumber = source.IfateReferenceNumber,
            LearningType = source.LearningType,
            Sector = source.Route,
            CourseName = source.Title,
            Level = source.Level,
            RegulatorName = source.ApprovalBody,
            IsRegulatedForProvider = source.IsRegulatedForProvider,
            Duration = source.Duration,
            DurationUnits = source.DurationUnits
        };

    public static implicit operator ShortCourseInformationViewModel(GetProviderAvailableCourseDetailsQueryResult source)
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
            Duration = source.Duration,
            DurationUnits = source.DurationUnits,
            LastDateStarts = source.LastDateStarts
        };
}