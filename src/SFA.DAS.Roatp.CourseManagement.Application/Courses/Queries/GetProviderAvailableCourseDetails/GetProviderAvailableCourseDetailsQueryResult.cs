using System;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;

namespace SFA.DAS.Roatp.CourseManagement.Application.Courses.Queries.GetProviderAvailableCourseDetails;

public class GetProviderAvailableCourseDetailsQueryResult
{
    public string LarsCode { get; set; }
    public int Level { get; set; }
    public string Title { get; set; }
    public string Sector { get; set; }
    public bool IsRegulatedForProvider { get; set; }
    public string ApprovalBody { get; set; }
    public DateTime? LastDateStarts { get; set; }
    public string IfateReferenceNumber { get; set; }
    public LearningType LearningType { get; set; }
    public int Duration { get; set; }
    public DurationUnits DurationUnits { get; set; }
}
