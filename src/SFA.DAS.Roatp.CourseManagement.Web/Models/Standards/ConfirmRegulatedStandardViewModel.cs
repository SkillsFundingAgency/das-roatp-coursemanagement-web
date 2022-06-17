using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Application.Standard.Commands.UpdateApprovedByRegulator;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.Standards
{
    public class ConfirmRegulatedStandardViewModel
    {
        [FromRoute]
        public int LarsCode { get; set; }
        public string RegulatorName { get; set; }
        public bool IsRegulatedStandard => !string.IsNullOrEmpty(RegulatorName);
        public bool? IsApprovedByRegulator { get; set; }
        public string BackLink { get; set; }
        public string CancelLink { get; set; }
        public string RefererLink { get; set; }
        public static implicit operator ConfirmRegulatedStandardViewModel(StandardDetails source)
        {
            return new ConfirmRegulatedStandardViewModel
            {
                RegulatorName = source.RegulatorName,
                IsApprovedByRegulator = source.IsApprovedByRegulator
            };
        }
        public static implicit operator UpdateApprovedByRegulatorCommand(ConfirmRegulatedStandardViewModel model) =>
            new UpdateApprovedByRegulatorCommand
            {
                LarsCode = model.LarsCode,
                IsApprovedByRegulator = model.IsApprovedByRegulator.GetValueOrDefault(),
            };
    }
}
