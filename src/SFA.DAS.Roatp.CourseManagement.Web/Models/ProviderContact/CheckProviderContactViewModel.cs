namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderContact;

public class CheckProviderContactViewModel : IBackLink
{
    public string BackUrl { get; set; }
    public string ChangeProviderContactDetailsLink { get; set; }
    public string EmailAddress { get; set; }
    public string PhoneNumber { get; set; }
}