using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;


public class UseSavedContactDetailsViewModel : IBackLink
{
    [FromRoute]
    public required int Ukprn { get; set; }

    public string EmailAddress { get; set; }
    public string PhoneNumber { get; set; }
    public bool? ShowEmail { get; set; }
    public bool? ShowPhone { get; set; }
    public string BackUrl { get; set; }
    public bool? IsUsingSavedContactDetails { get; set; }
}