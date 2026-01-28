using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;

public class SelectShortCourseLocationViewModel : SelectShortCourseLocationSubmitModel, IBackLink
{
    public CourseType CourseType { get; set; }
}
