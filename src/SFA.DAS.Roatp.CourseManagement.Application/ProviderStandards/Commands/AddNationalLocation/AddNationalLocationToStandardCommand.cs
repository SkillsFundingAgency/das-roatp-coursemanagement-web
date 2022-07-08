using MediatR;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.AddNationalLocation
{
    public class AddNationalLocationToStandardCommand : IRequest<Unit>
    {
        public int Ukprn { get; }
        public int LarsCode { get; }
        public string UserId { get; }
        public AddNationalLocationToStandardCommand(int ukprn, int larsCode, string userId)
        {
            Ukprn = ukprn;
            LarsCode = larsCode;
            UserId = userId;
        }
    }
}
