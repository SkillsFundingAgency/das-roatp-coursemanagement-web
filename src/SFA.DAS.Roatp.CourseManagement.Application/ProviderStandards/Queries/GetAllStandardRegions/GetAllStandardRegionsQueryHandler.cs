using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAllStandardRegions
{
    public class GetAllStandardRegionsQueryHandler : IRequestHandler<GetAllStandardRegionsQuery, GetAllStandardRegionsQueryResult>
    {
        private readonly ILogger<GetAllStandardRegionsQueryHandler> _logger;
        private readonly IApiClient _apiClient;
        public GetAllStandardRegionsQueryHandler(IApiClient apiClient, ILogger<GetAllStandardRegionsQueryHandler> logger)
        {
            _logger = logger;
            _apiClient = apiClient;
        }
        public async Task<GetAllStandardRegionsQueryResult> Handle(GetAllStandardRegionsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Get All Standard Regions request received for ukprn {ukprn} and larsCode {larsCode}", request.Ukprn, request.LarsCode);

            var allStandardRegions = await _apiClient.Get<GetAllStandardRegionsQueryResult>($"providers/{request.Ukprn}/courses/{request.LarsCode}/standardsubregions");
            if (allStandardRegions == null)
            {
                var message = $"All Standard Regions not found for ukprn {request.Ukprn} and larsCode {request.LarsCode}";
                _logger.LogError(message);
                throw new ValidationException(message);
            }
            return new GetAllStandardRegionsQueryResult
            {
                Regions = allStandardRegions.Regions
            };
        }
    }
}
