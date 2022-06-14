using MediatR;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAllProviderStandards
{
    public class GetAllProviderStandardsQuery : IRequest<GetAllProviderStandardsQueryResult>
    {
        public int Ukprn { get; }

        public GetAllProviderStandardsQuery(int ukprn)
        {
            Ukprn = ukprn;
        }
    }
}
