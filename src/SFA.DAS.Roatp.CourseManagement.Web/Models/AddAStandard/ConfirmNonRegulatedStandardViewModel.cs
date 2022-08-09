using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard
{
    public class ConfirmNonRegulatedStandardViewModel : ConfirmNonRegulatedStandardSubmitModel
    {
        public StandardInformationViewModel StandardInformation { get; set; }
        public string CancelLink { get; set; }
    }

    public class ConfirmNonRegulatedStandardSubmitModel
    {
        public int LarsCode { get; set; }
        public bool? IsCorrectStandard { get; set; }
    }
}
