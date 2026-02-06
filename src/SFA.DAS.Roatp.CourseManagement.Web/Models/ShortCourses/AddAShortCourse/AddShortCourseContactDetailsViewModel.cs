namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;

public class AddShortCourseContactDetailsViewModel : CourseContactDetailsSubmitModel, IBackLink
{
    public bool ShowSavedContactDetailsText { get; set; }
    public string ApprenticeshipType { get; set; }
}
