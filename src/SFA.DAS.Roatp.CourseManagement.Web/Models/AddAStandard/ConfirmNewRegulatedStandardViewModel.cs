using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard
{
    public class ConfirmNewRegulatedStandardViewModel : ConfirmNewRegulatedStandardSubmitModel, IBackLink
    {
        public string ContinueLink { get; set; }
        public StandardInformationViewModel StandardInformation { get; set; }
    }

    public class ConfirmNewRegulatedStandardSubmitModel
    {
        public bool? IsApprovedByRegulator { get; set; }
    }
}