namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderContact;

public class ConfirmUpdateStandardsViewModel : ConfirmUpdateStandardsSubmitViewModel, IBackLink
{
}

public class ConfirmUpdateStandardsSubmitViewModel
{
    public bool? HasOptedToUpdateExistingStandards { get; set; }
    public string EmailAddress { get; set; }
    public string PhoneNumber { get; set; }
    public bool EmailAddressOnlyUpdate { get; set; }
    public bool PhoneNumberOnlyUpdate { get; set; }
    public bool EmailAddressAndPhoneNumberUpdate { get; set; }
}