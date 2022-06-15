using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAllProviderStandards
{
    public class GetAllProviderStandardsQueryHandler : IRequestHandler<GetAllProviderStandardsQuery, GetAllProviderStandardsQueryResult>
    {
        private readonly ILogger<GetAllProviderStandardsQueryHandler> _logger;
        private readonly IApiClient _apiClient;
        public GetAllProviderStandardsQueryHandler(IApiClient apiClient, ILogger<GetAllProviderStandardsQueryHandler> logger)
        {
            _logger = logger;
            _apiClient = apiClient;
        }
        public async Task<GetAllProviderStandardsQueryResult> Handle(GetAllProviderStandardsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Get Standards request received for Ukprn number {ukprn}", request.Ukprn);
            try
            {
                var standards = await _apiClient.Get<List<Domain.ApiModels.Standard>>($"Standards/{request.Ukprn}");
                if (standards == null)
                {
                    _logger.LogInformation("Courses data not found for {ukprn}", request.Ukprn);
                    return null;
                }

                return new GetAllProviderStandardsQueryResult
                {
                    Standards = standards
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred trying to retrieve Standards for Ukprn {request.Ukprn}");
                throw;
            }
        }
    }
}
