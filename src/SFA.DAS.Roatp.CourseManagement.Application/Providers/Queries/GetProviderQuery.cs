using MediatR;

namespace SFA.DAS.Roatp.CourseManagement.Application.Providers.Queries
{
    public class GetProviderQuery : IRequest<GetProviderQueryResult>
    {
        public int Ukprn { get; }

        public GetProviderQuery(int ukprn)
        {
            Ukprn = ukprn;
        }
    }
}
