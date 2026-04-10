namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderContact;

public class AddProviderContactViewModel : AddProviderContactSubmitViewModel, IBackLink
{
    public bool ExistingContactDetailsAvailable { get; set; }
}

public class AddProviderContactSubmitViewModel
{
    public string EmailAddress { get; set; }
    public string PhoneNumber { get; set; }
}
