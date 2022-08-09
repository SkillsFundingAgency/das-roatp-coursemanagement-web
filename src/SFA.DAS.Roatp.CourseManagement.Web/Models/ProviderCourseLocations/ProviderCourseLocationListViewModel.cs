using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderCourseLocations
{
    public class ProviderCourseLocationListViewModel
    {
        [FromRoute]
        public int LarsCode { get; set; }
        [FromRoute]
        public int Ukprn { get; set; }
        public List<ProviderCourseLocationViewModel> ProviderCourseLocations { get; set; } = new List<ProviderCourseLocationViewModel>();
        public string AddTrainingLocationUrl { get; set; }
        public string BackUrl { get; set; }
        public string CancelUrl { get; set; }
    }
}
