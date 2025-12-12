using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard
{
    public class CourseLocationAddViewModel : CourseLocationAddSubmitModel, IBackLink
    {
        [FromRoute]
        public string LarsCode { get; set; }
        public IEnumerable<SelectListItem> TrainingVenues { get; set; }
    }

    public class CourseLocationAddSubmitModel
    {
        public string TrainingVenueNavigationId { get; set; }
        public bool HasDayReleaseDeliveryOption { get; set; }
        public bool HasBlockReleaseDeliveryOption { get; set; }
    }
}
