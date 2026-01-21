using SFA.DAS.Roatp.CourseManagement.Application.Standards.Queries.GetStandardInformation;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.AddAnApprenticeshipUnit;

public class ShortCourseInformationViewModel
{
    public string LarsCode { get; set; }
    public string CourseName { get; set; }
    public int Level { get; set; }
    public string IfateReferenceNumber { get; set; }
    public string Sector { get; set; }
    public string RegulatorName { get; set; }
    public ApprenticeshipType ApprenticeshipType { get; set; }
    public bool IsRegulatedForProvider { get; set; }
    public int Duration { get; set; }
    public DurationUnits DurationUnits { get; set; }
    public CourseType CourseType { get; set; }

    public static implicit operator ShortCourseInformationViewModel(GetStandardInformationQueryResult source)
        => new()
        {
            LarsCode = source.LarsCode,
            IfateReferenceNumber = source.IfateReferenceNumber,
            ApprenticeshipType = source.ApprenticeshipType,
            Sector = source.Route,
            CourseName = source.Title,
            Level = source.Level,
            RegulatorName = source.ApprovalBody,
            IsRegulatedForProvider = source.IsRegulatedForProvider,
            Duration = source.Duration,
            DurationUnits = source.DurationUnits,
            CourseType = source.CourseType,
        };
}