using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models
{
    public class EditNationalDeliveryOptionViewModel
    {
        [FromRoute]
        public int LarsCode { get; set; }
        public bool? HasNationalDeliveryOption { get; set; }
        public string BackLink { get; set; } = "#";
        public string CancelLink { get; set; } = "#";
    }
}
