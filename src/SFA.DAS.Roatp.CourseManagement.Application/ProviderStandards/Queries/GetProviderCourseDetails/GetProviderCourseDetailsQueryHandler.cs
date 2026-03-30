using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetProviderCourseDetails
{
    public class GetProviderCourseDetailsQueryHandler : IRequestHandler<GetProviderCourseDetailsQuery, GetProviderCourseDetailsQueryResult>
    {
        private readonly ILogger<GetProviderCourseDetailsQueryHandler> _logger;
        private readonly IApiClient _apiClient;
        public GetProviderCourseDetailsQueryHandler(IApiClient apiClient, ILogger<GetProviderCourseDetailsQueryHandler> logger)
        {
            _logger = logger;
            _apiClient = apiClient;
        }
        public async Task<GetProviderCourseDetailsQueryResult> Handle(GetProviderCourseDetailsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Get Standards details request received for ukprn {ukprn} and larsCode {larsCode}", request.Ukprn, request.LarsCode);
            var standardDetails = await _apiClient.Get<Domain.ApiModels.StandardDetails>($"providers/{request.Ukprn}/courses/{request.LarsCode}");
            if (standardDetails == null)
            {
                _logger.LogError($"Standard details not found for ukprn {request.Ukprn} and LarsCode {request.LarsCode}");
                return null;
            }

            return standardDetails;
        }
    }
}