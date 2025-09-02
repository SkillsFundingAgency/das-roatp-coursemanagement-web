namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderContact;

public class ProviderContactUpdateStandardsViewModel : ProviderContactUpdateStandardsSubmitViewModel, IBackLink
{
    public string BackUrl { get; set; }
}

public class ProviderContactUpdateStandardsSubmitViewModel
{
    public bool? HasOptedToUpdateExistingStandards { get; set; }
    public string EmailAddress { get; set; }
    public string PhoneNumber { get; set; }
}