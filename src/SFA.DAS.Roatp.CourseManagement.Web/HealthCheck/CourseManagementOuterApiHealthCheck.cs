using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.HealthCheck
{
    public class CourseManagementOuterApiHealthCheck : IHealthCheck
    {
        public const string HealthCheckResultDescription = "CourseManagement Outer API Health Check";

        private readonly IApiClient _apiClient;
        private readonly ILogger<CourseManagementOuterApiHealthCheck> _logger;

        public CourseManagementOuterApiHealthCheck(IApiClient apiClient, ILogger<CourseManagementOuterApiHealthCheck> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            var response = await _apiClient.Get("ping");

            if (response == HttpStatusCode.OK)
            {
                return HealthCheckResult.Healthy(HealthCheckResultDescription);
            }

            _logger.LogError("CourseManagement Outer API ping failed : [Code: {response}]", response);
            return HealthCheckResult.Unhealthy(HealthCheckResultDescription);
        }
    }
}
