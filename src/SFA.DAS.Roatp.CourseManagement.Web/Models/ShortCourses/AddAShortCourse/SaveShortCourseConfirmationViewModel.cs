namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;

public class SaveShortCourseConfirmationViewModel : ShortCourseBaseViewModel
{
    public required string CourseName { get; set; }
    public string DashboardLink { get; set; } = "#";
    public string ManageTrainingTypeLink { get; set; } = "#";
}
