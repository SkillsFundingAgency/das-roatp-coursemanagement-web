using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderCourseLocations;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.Standards
{
    public class ProviderCourseLocationListViewModel
    {
        [FromRoute]
        public int LarsCode { get; set; }
        [FromRoute]
        public int Ukprn { get; set; }
        public List<ProviderCourseLocationViewModel> ProviderCourseLocations { get; set; } = new List<ProviderCourseLocationViewModel>();
        public string BackUrl { get; set; }
        public string CancelUrl { get; set; }
    }
}
