using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;


namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetProviderLocationDetails
{
    public class GetProviderLocationDetailsQueryHandler : IRequestHandler<GetProviderLocationDetailsQuery, GetProviderLocationDetailsQueryResult>
    {
        private readonly ILogger<GetProviderLocationDetailsQueryHandler> _logger;
        private readonly IApiClient _apiClient;
        public GetProviderLocationDetailsQueryHandler(IApiClient apiClient, ILogger<GetProviderLocationDetailsQueryHandler> logger)
        {
            _logger = logger;
            _apiClient = apiClient;
        }
        public async Task<GetProviderLocationDetailsQueryResult> Handle(GetProviderLocationDetailsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Get Provider Location request received for Ukprn number {Ukprn} and {Id}", request.Ukprn, request.Id);
            var providerLocation = await _apiClient.Get<Domain.ApiModels.ProviderLocation>($"providers/{request.Ukprn}/locations/{request.Id}");
            if (providerLocation == null)
            {
                _logger.LogError("Provider Location not found for {Ukprn} and {Id}", request.Ukprn, request.Id);
                return null;
            }

            return new GetProviderLocationDetailsQueryResult
            {
                ProviderLocation = providerLocation
            };
        }
    }
}
