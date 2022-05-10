using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderLocations
{
    public class ProviderLocationListViewModel : ViewModelBase
    {
        public ProviderLocationListViewModel(HttpContext context) : base(context)
        {
        }
        public List<ProviderLocationViewModel> ProviderLocations { get; set; } = new List<ProviderLocationViewModel>();
    }
}
