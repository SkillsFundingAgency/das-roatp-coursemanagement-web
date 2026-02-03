using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;

public class AddShortCourseContactDetailsViewModel : CourseContactDetailsSubmitModel, IBackLink
{
    public bool ShowSavedContactDetailsText { get; set; }
    public CourseType CourseType { get; set; }
}
