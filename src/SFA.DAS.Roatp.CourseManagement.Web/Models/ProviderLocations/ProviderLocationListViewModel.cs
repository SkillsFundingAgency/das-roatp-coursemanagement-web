using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderLocations
{
    public class ProviderLocationListViewModel : IBackLink
    {
        public List<ProviderLocationViewModel> ProviderLocations { get; set; } = new List<ProviderLocationViewModel>();
        public string AddTrainingLocationLink { get; set; }

        public bool ShowNotificationBannerAddVenue { get; set; }
        public string ManageYourStandardsUrl { get; set; }
    }
}
