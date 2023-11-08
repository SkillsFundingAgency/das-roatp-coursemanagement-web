using MediatR;

namespace SFA.DAS.Roatp.CourseManagement.Application.Providers.Queries.GetProviderAccount
{
    public class GetProviderAccountStatusQuery : IRequest<GetProviderAccountStatusResult>
    {
        public long Ukprn { get; }

        public GetProviderAccountStatusQuery(long ukprn)
        {
            Ukprn = ukprn;
        }
    }
}
