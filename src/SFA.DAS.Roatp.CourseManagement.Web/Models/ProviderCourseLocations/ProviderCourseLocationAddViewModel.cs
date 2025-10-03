using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderCourseLocations
{
    public class ProviderCourseLocationAddViewModel : ProviderCourseLocationAddSubmitModel, IBackLink
    {
        [FromRoute]
        public int LarsCode { get; set; }
        public IEnumerable<SelectListItem> TrainingVenues { get; set; }
    }

    public class ProviderCourseLocationAddSubmitModel
    {
        public string TrainingVenueNavigationId { get; set; }
        public bool HasDayReleaseDeliveryOption { get; set; }
        public bool HasBlockReleaseDeliveryOption { get; set; }
    }
}
