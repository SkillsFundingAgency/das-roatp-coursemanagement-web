using System;
using MediatR;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetProviderLocationDetails;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetProviderLocationDetailsForDeletion;

public class GetProviderLocationDetailsForDeletionQuery : IRequest<GetProviderLocationDetailsQueryResult>
{
    public int Ukprn { get; }
    public Guid Id { get; }

    public GetProviderLocationDetailsForDeletionQuery(int ukprn, Guid id)
    {
        Ukprn = ukprn;
        Id = id;
    }
}