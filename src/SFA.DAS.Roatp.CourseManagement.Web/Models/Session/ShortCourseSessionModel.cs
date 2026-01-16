using SFA.DAS.Roatp.CourseManagement.Web.Models.AddShortCourses;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.Session;

public class ShortCourseSessionModel
{
    public string LarsCode { get; set; }
    public ShortCourseInformationViewModel ShortCourseInformation { get; set; } = new ShortCourseInformationViewModel();
}
