using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderLocations
{
    public class ProviderLocationListViewModel : ViewModelBase
    {
        public List<ProviderLocationViewModel> ProviderLocations { get; set; } = new List<ProviderLocationViewModel>();
    }
}
