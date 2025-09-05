namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderContact;

public class ProviderContactUpdateViewModel : IBackLink
{

    public string EmailAddress { get; set; }
    public string PhoneNumber { get; set; }
    public string BackUrl { get; set; }
    public string ReviewYourDetailsUrl { get; set; }
    public string ChangeEmailPhoneUrl { get; set; } = "#";
    public string ChangeSelectedStandardsUrl { get; set; } = "#";

    public bool ShowEmail { get; set; } = true;
    public bool ShowPhone { get; set; } = true;
}