using MediatR;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderContact.Queries;

public class GetLatestProviderContactQuery : IRequest<GetLatestProviderContactQueryResult>
{
    public int Ukprn { get; }

    public GetLatestProviderContactQuery(int ukprn)
    {
        Ukprn = ukprn;
    }
}