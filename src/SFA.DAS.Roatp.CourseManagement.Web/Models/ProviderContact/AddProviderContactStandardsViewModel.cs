using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderContact;

public class AddProviderContactStandardsViewModel : AddProviderContactStandardsSubmitViewModel, IBackLink
{
    public List<ProviderContactStandardModel> Standards { get; set; }
    public List<ProviderContactStandardModel> ApprenticeshipUnits { get; set; }
}

public class AddProviderContactStandardsSubmitViewModel
{
    public List<int> SelectedProviderCourseIds { get; set; } = new List<int>();
}