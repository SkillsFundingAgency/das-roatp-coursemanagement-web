using MediatR;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.Regions.Queries;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.HealthCheck
{
    public class CourseManagementOuterApiHealthCheck : IHealthCheck
    {
        public const string HealthCheckResultDescription = "CourseManagement Outer API Health Check";

        private readonly IMediator _mediator;
        private readonly ILogger<CourseManagementOuterApiHealthCheck> _logger;

        public CourseManagementOuterApiHealthCheck(ILogger<CourseManagementOuterApiHealthCheck> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            _logger.LogInformation("CourseManagement Outer API pinging call");
            try
            {
                var allRegionsAndSubRegions = await _mediator.Send(new GetAllRegionsAndSubRegionsQuery(), cancellationToken);
                if (allRegionsAndSubRegions != null && allRegionsAndSubRegions.Regions != null && allRegionsAndSubRegions.Regions.Count() > 0)
                {
                    return HealthCheckResult.Healthy(HealthCheckResultDescription);
                }
                _logger.LogError("CourseManagement Outer API ping failed");
                return HealthCheckResult.Unhealthy(HealthCheckResultDescription);
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message, "CourseManagement Outer API ping failed");
                return HealthCheckResult.Unhealthy(HealthCheckResultDescription);
            }
        }
    }
}
