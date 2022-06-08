using MediatR;

namespace SFA.DAS.Roatp.CourseManagement.Application.Standard.Commands.UpdateConfirmRegulatedStandard
{
    public class UpdateConfirmRegulatedStandardCommand : IRequest
    {
        public int Ukprn{ get; set; }
        public int LarsCode { get; set; }
        public bool? IsApprovedByRegulator { get; set; }
    }
}
