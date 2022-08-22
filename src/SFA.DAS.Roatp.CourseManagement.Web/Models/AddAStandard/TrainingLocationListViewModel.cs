using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderCourseLocations;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard
{
    public class TrainingLocationListViewModel
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
