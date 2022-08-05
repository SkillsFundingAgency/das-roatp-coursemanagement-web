using MediatR;
using System;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetProviderLocationDetails
{
    public class GetProviderLocationDetailsQuery : IRequest<GetProviderLocationDetailsQueryResult>
    {
        public int Ukprn { get; }
        public Guid Id { get; }

        public GetProviderLocationDetailsQuery(int ukprn, Guid id)
        {
            Ukprn = ukprn;
            Id = id;
        }
    }
}
