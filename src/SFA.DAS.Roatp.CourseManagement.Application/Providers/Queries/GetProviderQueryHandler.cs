using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;

namespace SFA.DAS.Roatp.CourseManagement.Application.Providers.Queries
{
    public class GetProviderQueryHandler : IRequestHandler<GetProviderQuery, GetProviderQueryResult>
    {
        private readonly ILogger<GetProviderQueryHandler> _logger;
        private readonly IApiClient _apiClient;
        public GetProviderQueryHandler(IApiClient apiClient, ILogger<GetProviderQueryHandler> logger)
        {
            _logger = logger;
            _apiClient = apiClient;
        }
        
        public async Task<GetProviderQueryResult> Handle(GetProviderQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Get Provider request received for Ukprn number {ukprn}", request.Ukprn);
            var provider = await _apiClient.Get<Domain.ApiModels.Provider>($"providers/{request.Ukprn}");
            if (provider == null)
            {
                _logger.LogError("Provider not found for {ukprn}", request.Ukprn);
                throw new InvalidOperationException($"Provider not found for UKPRN {request.Ukprn}");
            }

            return new GetProviderQueryResult
            {
                Provider = provider
            };
        }
    }
}
