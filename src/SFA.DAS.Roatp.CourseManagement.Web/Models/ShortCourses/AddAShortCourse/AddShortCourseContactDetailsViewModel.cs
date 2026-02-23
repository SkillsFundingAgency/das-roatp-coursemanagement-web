namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;

public class AddShortCourseContactDetailsViewModel : CourseContactDetailsSubmitModel, IBackLink
{
    public ShortCourseBaseViewModel ShortCourseBaseModel { get; set; } = new ShortCourseBaseViewModel();
    public bool ShowSavedContactDetailsText { get; set; }
    public string SubmitButtonText { get; set; }
}
