using SFA.DAS.Roatp.CourseManagement.Domain.Models;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models
{
    public class EditLocationOptionViewModel
    {
        public LocationOption LocationOption { get; set; }
        public string BackLink { get; set; } = "#";
        public string CancelLink { get; set; } = "#";
    }
}
