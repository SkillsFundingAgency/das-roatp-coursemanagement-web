using System.Collections.Generic;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderContact;

public class ProviderContactSessionModel
{
    public string EmailAddress { get; set; }
    public string PhoneNumber { get; set; }
    public List<ProviderContactStandardModel> Standards { get; set; }

    public bool? UpdateExistingStandards { get; set; }

}
