using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;

public class ConfirmShortCourseViewModel : ConfirmShortCourseSubmitModel, IBackLink
{
    public ShortCourseInformationViewModel ShortCourseInformation { get; set; }
}

public class ConfirmShortCourseSubmitModel
{
    public bool? IsCorrectShortCourse { get; set; }
    public required CourseType CourseType { get; set; }
}