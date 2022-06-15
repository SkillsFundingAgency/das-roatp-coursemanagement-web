using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;


namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAllProviderLocations
{
    public class GetAllProviderLocationsQueryHandler : IRequestHandler<GetAllProviderLocationsQuery, GetAllProviderLocationsQueryResult>
    {
        private readonly ILogger<GetAllProviderLocationsQueryHandler> _logger;
        private readonly IApiClient _apiClient;
        public GetAllProviderLocationsQueryHandler(IApiClient apiClient, ILogger<GetAllProviderLocationsQueryHandler> logger)
        {
            _logger = logger;
            _apiClient = apiClient;
        }
        public async Task<GetAllProviderLocationsQueryResult> Handle(GetAllProviderLocationsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Get Provider Locations request received for Ukprn number {ukprn}", request.Ukprn);
            var trainingLocations = await _apiClient.Get<List<Domain.ApiModels.ProviderLocation>>($"providers/{request.Ukprn}/locations");
            if (trainingLocations == null)
            {
                _logger.LogError("Provider Locations not found for {ukprn}", request.Ukprn);
                throw new ValidationException("Provider Locations not found for {request.Ukprn}", null);
            }

            var providerLocations = trainingLocations.FindAll(l => l.LocationType == Domain.ApiModels.LocationType.Provider);

            return new GetAllProviderLocationsQueryResult
            {
                ProviderLocations = providerLocations
            };
        }
    }
}
