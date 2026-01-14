using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Constants;

namespace SFA.DAS.Roatp.CourseManagement.Application.Standards.Queries.GetStandardInformation;

public class GetStandardInformationQueryResult
{
    public string StandardUId { get; set; }
    public string IfateReferenceNumber { get; set; }
    public string LarsCode { get; set; }
    public string Title { get; set; }
    public int Level { get; set; }
    public ApprenticeshipType ApprenticeshipType { get; set; }
    public string ApprovalBody { get; set; }
    public string Route { get; set; }
    public int Duration { get; set; }
    public DurationUnits DurationUnits { get; set; }
    public bool IsRegulatedForProvider { get; set; }
    public CourseType CourseType { get; set; }
}