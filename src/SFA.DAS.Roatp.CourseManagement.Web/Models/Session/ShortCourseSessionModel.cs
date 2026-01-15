using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAnApprenticeshipUnit;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.Session;

public class ShortCourseSessionModel
{
    public string LarsCode { get; set; }
    public ShortCourseInformationViewModel ShortCourseInformation { get; set; } = new ShortCourseInformationViewModel();
}
