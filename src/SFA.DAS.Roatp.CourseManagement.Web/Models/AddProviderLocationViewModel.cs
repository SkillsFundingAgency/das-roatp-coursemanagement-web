using SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models;

public class AddProviderLocationViewModel : AddressSearchSubmitModel, IBackLink
{
    public string SubmitButtonText { get; set; }
    public string Route { get; set; }
    public bool IsAddJourney { get; set; }
    public string DisplayHeader { get; set; }
}