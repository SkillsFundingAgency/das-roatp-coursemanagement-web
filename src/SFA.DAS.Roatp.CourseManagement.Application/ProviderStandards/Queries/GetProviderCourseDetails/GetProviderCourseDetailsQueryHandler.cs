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
            _logger.LogInformation("Get provide course details request received for ukprn {Ukprn} and larsCode {LarsCode}", request.Ukprn, request.LarsCode);
            var standardDetails = await _apiClient.Get<Domain.ApiModels.StandardDetails>($"providers/{request.Ukprn}/courses/{request.LarsCode}");
            if (standardDetails == null)
            {
                _logger.LogError("Provide course details not found for ukprn {Ukprn} and LarsCode {LarsCode}", request.Ukprn, request.LarsCode);
                return null;
            }

            return standardDetails;
        }
    }
}