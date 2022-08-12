using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard
{
    public class StandardSessionModel
    {
        public int LarsCode { get; set; }
        public bool IsConfirmed { get; set; }
        public StandardInformationViewModel StandardInformation { get; set; } = new StandardInformationViewModel();
        public StandardContactInformationViewModel ContactInformation { get; set; } = new StandardContactInformationViewModel();
        public LocationOption LocationOption { get; set; }
        public bool? HasNationalDeliveryOption { get; set; }
    }
}
