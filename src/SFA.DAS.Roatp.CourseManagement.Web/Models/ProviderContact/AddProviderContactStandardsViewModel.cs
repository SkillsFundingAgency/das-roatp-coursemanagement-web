using System.Collections.Generic;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderContact;

public class AddProviderContactStandardsViewModel : AddProviderContactStandardsSubmitViewModel, IBackLink
{
    public List<ProviderContactStandardModel> Standards { get; set; }
    public List<ProviderContactStandardModel> ApprenticeshipUnits { get; set; }
    public bool ShowStandards { get; set; }
    public bool ShowApprenticeshipUnits { get; set; }
}

public class AddProviderContactStandardsSubmitViewModel
{
    public List<int> SelectedProviderCourseIds { get; set; } = new List<int>();
}