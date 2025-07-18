using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetProviderLocationDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetProviderLocationDetailsForDeletion
{
    public class GetProviderLocationDetailsForDeletionQueryHandler : IRequestHandler<GetProviderLocationDetailsForDeletionQuery, GetProviderLocationDetailsQueryResult>
    {
        private readonly ILogger<GetProviderLocationDetailsForDeletionQueryHandler> _logger;
        private readonly IApiClient _apiClient;
        public GetProviderLocationDetailsForDeletionQueryHandler(IApiClient apiClient, ILogger<GetProviderLocationDetailsForDeletionQueryHandler> logger)
        {
            _logger = logger;
            _apiClient = apiClient;
        }
        public async Task<GetProviderLocationDetailsQueryResult> Handle(GetProviderLocationDetailsForDeletionQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Get Provider Location request received for Ukprn number {Ukprn} and {Id}", request.Ukprn, request.Id);
            var providerLocation = await _apiClient.Get<ProviderLocation>($"providers/{request.Ukprn}/locations/{request.Id}");
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
