using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;


namespace SFA.DAS.Roatp.CourseManagement.Application.Regions.Queries.GetAllRegions
{
    public class GetAllRegionsQueryHandler : IRequestHandler<GetAllRegionsQuery, GetAllRegionsQueryResult>
    {
        private readonly ILogger<GetAllRegionsQueryHandler> _logger;
        private readonly IApiClient _apiClient;
        public GetAllRegionsQueryHandler(IApiClient apiClient, ILogger<GetAllRegionsQueryHandler> logger)
        {
            _logger = logger;
            _apiClient = apiClient;
        }
        public async Task<GetAllRegionsQueryResult> Handle(GetAllRegionsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Get All Regions request received");
            var regions = await _apiClient.Get<List<Domain.ApiModels.Region>>($"regions");
            if (regions == null)
            {
                _logger.LogError("All Regions not found");
                throw new ValidationException("All Regions not found", null);
            }
            return new GetAllRegionsQueryResult
            {
                Regions = regions
            };
        }
    }
}
