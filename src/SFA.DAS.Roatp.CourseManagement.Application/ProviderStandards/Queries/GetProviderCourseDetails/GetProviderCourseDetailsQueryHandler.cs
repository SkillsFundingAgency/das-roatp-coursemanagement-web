using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetProviderCourseDetails
{
    public class GetProviderCourseDetailsQueryHandler : IRequestHandler<GetProviderCourseDetailsQuery, GetProviderCourseDetailsQueryResult>
    {
        private readonly IProviderCourseDetailsCachedService _providerCourseDetailsCachedService;
        private readonly ILogger<GetProviderCourseDetailsQueryHandler> _logger;
        public GetProviderCourseDetailsQueryHandler(IProviderCourseDetailsCachedService providerCourseDetailsCachedService, ILogger<GetProviderCourseDetailsQueryHandler> logger)
        {
            _logger = logger;
            _providerCourseDetailsCachedService = providerCourseDetailsCachedService;
        }
        public async Task<GetProviderCourseDetailsQueryResult> Handle(GetProviderCourseDetailsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Get provide course details request received for ukprn {Ukprn} and larsCode {LarsCode}", request.Ukprn, request.LarsCode);
            var standardDetails = await _providerCourseDetailsCachedService.GetCachedProviderCourseDetails(request.Ukprn, request.LarsCode);
            if (standardDetails == null)
            {
                _logger.LogError("Provide course details not found for ukprn {Ukprn} and LarsCode {LarsCode}", request.Ukprn, request.LarsCode);
                return null;
            }

            return standardDetails;
        }
    }
}