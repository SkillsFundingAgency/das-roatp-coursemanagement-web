using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAvailableProviderLocations
{
    public class GetAvailableProviderLocationsQueryHandler : IRequestHandler<GetAvailableProviderLocationsQuery, GetAvailableProviderLocationsQueryResult>
    {
        private readonly IApiClient _apiClient;
        private readonly ILogger<GetAvailableProviderLocationsQueryHandler> _logger;

        public GetAvailableProviderLocationsQueryHandler(IApiClient apiClient, ILogger<GetAvailableProviderLocationsQueryHandler> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        public async Task<GetAvailableProviderLocationsQueryResult> Handle(GetAvailableProviderLocationsQuery request, CancellationToken cancellationToken)
        {
            var result = await _apiClient.Get<GetAvailableProviderLocationsQueryResult>($"providers/{request.Ukprn}/locations/{request.LarsCode}/available-providerlocations");
            _logger.LogInformation("Found {count} available provider locations for ukprn: {ukprn} larsCode: {larsCode}", result.AvailableProviderLocations.Count, request.Ukprn, request.LarsCode);
            return result;
        }
    }
}
