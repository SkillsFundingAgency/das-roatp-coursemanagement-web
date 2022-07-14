using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderLocations
{
    public class ProviderLocationListViewModel 
    {
        public List<ProviderLocationViewModel> ProviderLocations { get; set; } = new List<ProviderLocationViewModel>();
        public string BackUrl { get; set; }
        public string AddTrainingLocationLink { get; set; }
    }
}
