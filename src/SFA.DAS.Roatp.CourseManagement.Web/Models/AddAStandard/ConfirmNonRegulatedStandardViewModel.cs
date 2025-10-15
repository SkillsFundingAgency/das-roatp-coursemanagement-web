using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard
{
    public class ConfirmNonRegulatedStandardViewModel : ConfirmNonRegulatedStandardSubmitModel, IBackLink
    {
        public StandardInformationViewModel StandardInformation { get; set; }
    }

    public class ConfirmNonRegulatedStandardSubmitModel
    {
        public bool? IsCorrectStandard { get; set; }
    }
}
