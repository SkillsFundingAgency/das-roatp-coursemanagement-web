namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.ManageShortCourses;

public class ManageShortCoursesViewModel : ShortCourseBaseViewModel, IBackLink
{
    public CourseLinksViewModel CourseLinks { get; set; }
    public string AddAShortCourseLink { get; set; }
    public bool ShowShortCourseHeading { get; set; }
}
