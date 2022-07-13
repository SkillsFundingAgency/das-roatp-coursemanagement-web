using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails
{
    public class GetProviderCourseLocationsQueryHandler : IRequestHandler<GetProviderCourseLocationsQuery, GetProviderCourseLocationsQueryResult>
    {
        private readonly ILogger<GetProviderCourseLocationsQueryHandler> _logger;
        private readonly IApiClient _apiClient;
        public GetProviderCourseLocationsQueryHandler(IApiClient apiClient, ILogger<GetProviderCourseLocationsQueryHandler> logger)
        {
            _logger = logger;
            _apiClient = apiClient;
        }
        public async Task<GetProviderCourseLocationsQueryResult> Handle(GetProviderCourseLocationsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Get provider course locations request received for ukprn {ukprn} and larsCode {larsCode}", request.Ukprn, request.LarsCode);
            var providerCourseLocationsResult = await _apiClient.Get<GetProviderCourseLocationsQueryResult>($"providers/{request.Ukprn}/courses/{request.LarsCode}/locations/provider-locations");
            if (providerCourseLocationsResult == null)
            {
                var message = $"provider course locations not found for ukprn {request.Ukprn} and LarsCode {request.LarsCode}";
                _logger.LogError(message);
                throw new ValidationException(message);
            }

            return providerCourseLocationsResult;
        }
    }
}