using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAnApprenticeshipUnit;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.Session;

public class ApprenticeshipUnitSessionModel
{
    public string LarsCode { get; set; }
    public CourseInformation CourseInformation { get; set; } = new CourseInformation();
}
