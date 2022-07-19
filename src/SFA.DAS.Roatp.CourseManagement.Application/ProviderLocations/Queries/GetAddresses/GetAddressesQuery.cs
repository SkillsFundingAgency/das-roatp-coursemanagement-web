using MediatR;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAddresses
{
    public class GetAddressesQuery : IRequest<GetAddressesQueryResult>
    {
        public string Postcode { get; }
        public GetAddressesQuery(string postcode)
        {
            Postcode = postcode;
        }
    }
}
