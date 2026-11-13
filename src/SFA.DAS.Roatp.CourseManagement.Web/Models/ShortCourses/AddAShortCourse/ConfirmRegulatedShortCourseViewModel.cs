namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;

public class ConfirmRegulatedShortCourseViewModel : ConfirmRegulatedShortCourseSubmitModel, IBackLink
{
    public ShortCourseInformationViewModel ShortCourseInformation { get; set; }
}

public class ConfirmRegulatedShortCourseSubmitModel : ShortCourseBaseViewModel
{
    public bool? IsApprovedByRegulator { get; set; }
}
