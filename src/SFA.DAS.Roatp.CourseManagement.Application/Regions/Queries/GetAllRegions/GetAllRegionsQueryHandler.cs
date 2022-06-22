using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
            var regions = await _apiClient.Get<List<Region>>($"regions");
            if (regions == null)
            {
                _logger.LogError("All Regions not found");
                throw new ValidationException("All Regions not found", null);
            }

            _logger.LogInformation("Get Standards details request received for ukprn {ukprn} and larsCode {larsCode}", request.Ukprn, request.LarsCode);
            var standardDetails = await _apiClient.Get<StandardDetails>($"providers/{request.Ukprn}/courses/{request.LarsCode}");
            if (standardDetails == null)
            {
                var message = $"Standard details not found for ukprn {request.Ukprn} and LarsCode {request.LarsCode}";
                _logger.LogError(message);
                throw new ValidationException(message);
            }
            var subRegionCourseLocations = standardDetails.ProviderCourseLocations.Where(a => a.LocationType == LocationType.Regional).ToList();
            if(subRegionCourseLocations.Any())
            {
                foreach (var region in regions)
                {
                    region.IsSelected = subRegionCourseLocations.Exists(r => r.LocationName == region.SubregionName);
                }
            }

            return new GetAllRegionsQueryResult
            {
                Regions = regions
            };
        }
    }
}
