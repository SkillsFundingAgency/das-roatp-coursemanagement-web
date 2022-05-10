using MediatR;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries
{
    public class GetProviderLocationQuery : IRequest<GetProviderLocationQueryResult>
    {
        public int Ukprn { get; }

        public GetProviderLocationQuery(int ukprn)
        {
            Ukprn = ukprn;
        }
    }
}
