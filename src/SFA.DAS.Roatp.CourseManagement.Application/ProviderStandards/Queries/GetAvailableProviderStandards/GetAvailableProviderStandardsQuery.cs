using MediatR;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAvailableProviderStandards
{
    public class GetAvailableProviderStandardsQuery: IRequest<GetAvailableProviderStandardsQueryResult>
    {
        public int Ukprn { get; }
        public GetAvailableProviderStandardsQuery(int ukprn)
        {
            Ukprn = ukprn;
        }
    }
}
