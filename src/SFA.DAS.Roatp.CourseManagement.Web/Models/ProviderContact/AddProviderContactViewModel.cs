namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderContact;

public class AddProviderContactViewModel : AddProviderContactSubmitViewModel, IBackLink
{
}

public class AddProviderContactSubmitViewModel
{
    public string EmailAddress { get; set; }
    public string PhoneNumber { get; set; }
}
