using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderContact;

public class AddProviderContactCompleteViewModel
{
    public string EmailAddress { get; set; }
    public string PhoneNumber { get; set; }
    public string BackUrl { get; set; }
    public string ReviewYourDetailsUrl { get; set; }
    public List<string> CheckedStandards { get; set; }

    public bool UseBulletedList { get; set; }
}
