using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard
{
    public class CourseLocationAddViewModel : CourseLocationAddSubmitModel
    {
        [FromRoute]
        public int LarsCode { get; set; }
        public IEnumerable<SelectListItem> TrainingVenues { get; set; }
        public string BackLink { get; set; }
        public string CancelLink { get; set; }
    }

    public class CourseLocationAddSubmitModel
    {
        public string TrainingVenueNavigationId { get; set; }
        public bool HasDayReleaseDeliveryOption { get; set; }
        public bool HasBlockReleaseDeliveryOption { get; set; }
    }
}
