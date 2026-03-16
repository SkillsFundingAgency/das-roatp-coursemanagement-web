namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;

public class ShortCourseContactInformationViewModel : ShortCourseBaseViewModel
{
    public string StandardInfoUrl { get; set; }
    public string ContactUsPhoneNumber { get; set; }
    public string ContactUsEmail { get; set; }
    public string ContactDetailsChangeLink { get; set; } = "#";
}