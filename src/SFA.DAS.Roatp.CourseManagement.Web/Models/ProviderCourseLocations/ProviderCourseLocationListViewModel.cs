using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderCourseLocations
{
    public class ProviderCourseLocationListViewModel : IBackLink
    {
        [FromRoute]
        public string LarsCode { get; set; }
        [FromRoute]
        public int Ukprn { get; set; }
        public List<ProviderCourseLocationViewModel> ProviderCourseLocations { get; set; } = new List<ProviderCourseLocationViewModel>();
        public string AddTrainingLocationUrl { get; set; }
    }
}
