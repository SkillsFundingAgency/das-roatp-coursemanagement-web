namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderContact;

public class AddProviderContactCompleteViewModel
{
    public string EmailAddress { get; set; }
    public string PhoneNumber { get; set; }
    public string ManageCoursesUrl { get; set; }
    public CourseListViewModel CheckedStandards { get; set; }
    public CourseListViewModel CheckedApprenticeshipUnits { get; set; }

    public bool ShowBoth { get; set; }
    public bool ShowPhoneOnly { get; set; }
    public bool ShowEmailOnly { get; set; }

    public bool ShowStandards { get; set; }
    public bool ShowApprenticeshipUnits { get; set; }
}
