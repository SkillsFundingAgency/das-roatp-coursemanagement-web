using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard
{
    public class ConfirmNewRegulatedStandardViewModel : ConfirmNewRegulatedStandardSubmitModel
    {
        public StandardInformationViewModel StandardInformation { get; set; }
        public string CancelLink { get; set; }
    }
}