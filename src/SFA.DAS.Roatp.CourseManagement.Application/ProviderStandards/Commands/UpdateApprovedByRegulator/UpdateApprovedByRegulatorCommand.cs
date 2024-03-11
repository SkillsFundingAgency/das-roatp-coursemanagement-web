using MediatR;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.UpdateApprovedByRegulator
{
    public class UpdateApprovedByRegulatorCommand : IRequest<Unit>
    {
        public int Ukprn { get; set; }
        public int LarsCode { get; set; }
        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
        public bool IsApprovedByRegulator { get; set; }
    }
}
