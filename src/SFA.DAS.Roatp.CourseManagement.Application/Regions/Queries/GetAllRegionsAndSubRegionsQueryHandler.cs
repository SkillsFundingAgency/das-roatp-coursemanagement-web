using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Application.Regions.Queries
{
    public class GetAllRegionsAndSubRegionsQueryHandler : IRequestHandler<GetAllRegionsAndSubRegionsQuery, GetAllRegionsAndSubRegionsQueryResult>
    {
        private readonly ILogger<GetAllRegionsAndSubRegionsQueryHandler> _logger;
        private readonly IApiClient _apiClient;
        public const string LookupRegionsRoute = "lookup/regions";

        public GetAllRegionsAndSubRegionsQueryHandler(IApiClient apiClient, ILogger<GetAllRegionsAndSubRegionsQueryHandler> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        public async Task<GetAllRegionsAndSubRegionsQueryResult> Handle(GetAllRegionsAndSubRegionsQuery request, CancellationToken cancellationToken)
        {
            var regions = await _apiClient.Get<GetAllRegionsAndSubRegionsQueryResult>(LookupRegionsRoute);
            if (regions == null)
            {
                var error = "Get regions did not come back with successful response";
                _logger.LogError(error);
                throw new InvalidOperationException(error);
            }
            return regions;
        }
    }
}
