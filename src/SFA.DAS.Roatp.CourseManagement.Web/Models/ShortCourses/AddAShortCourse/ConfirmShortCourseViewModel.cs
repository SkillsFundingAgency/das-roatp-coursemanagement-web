namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;

public class ConfirmShortCourseViewModel : ConfirmShortCourseSubmitModel, IBackLink
{
    public ShortCourseInformationViewModel ShortCourseInformation { get; set; }
}

public class ConfirmShortCourseSubmitModel : ShortCourseBaseViewModel
{
    public bool? IsCorrectShortCourse { get; set; }
}