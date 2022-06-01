using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.Standards
{
    public class ConfirmRegulatedStandardViewModel
    {
        public string RegulatorName { get; set; }
        public bool? IsApprovedByRegulator { get; set; }
        public string BackUrl { get; set; }
        public static implicit operator ConfirmRegulatedStandardViewModel(StandardDetails source)
        {
            return new ConfirmRegulatedStandardViewModel
            {
                RegulatorName = source.RegulatorName,
                IsApprovedByRegulator = source.IsApprovedByRegulator
            };
        }
    }
}
