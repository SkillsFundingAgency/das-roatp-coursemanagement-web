namespace SFA.DAS.Roatp.CourseManagement.Web.Models.AddShortCourses;

public class ConfirmShortCourseViewModel : ConfirmShortCourseSubmitModel, IBackLink
{
    public ShortCourseInformationViewModel ShortCourseInformation { get; set; }
}

public class ConfirmShortCourseSubmitModel
{
    public bool? IsCorrectShortCourse { get; set; }
}