using MediatR;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAddresses
{
    public class GetAddressesQueryHandler : IRequestHandler<GetAddressesQuery, GetAddressesQueryResult>
    {
        private readonly IApiClient _apiClient;

        public GetAddressesQueryHandler(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<GetAddressesQueryResult> Handle(GetAddressesQuery request, CancellationToken cancellationToken)
        {
            var response = await _apiClient.Get<GetAddressesQueryResult>($"lookup/addresses?postcode={HttpUtility.UrlEncode(request.Postcode)}");
            return response;
        }
    }
}
