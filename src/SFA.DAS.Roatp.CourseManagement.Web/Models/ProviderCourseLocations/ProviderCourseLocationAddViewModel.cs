using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderCourseLocations
{
    public class ProviderCourseLocationAddViewModel : ProviderCourseLocationAddSubmitModel
    {
        [FromRoute]
        public int LarsCode { get; set; }
        public IEnumerable<SelectListItem> TrainingVenues { get; set; }
        public string BackLink { get; set; }
        public string CancelLink { get; set; }
    }

    public class ProviderCourseLocationAddSubmitModel
    {
        public string TrainingVenue { get; set; }
        public bool HasDayReleaseDeliveryOption { get; set; }
        public bool HasBlockReleaseDeliveryOption { get; set; }
    }
}
