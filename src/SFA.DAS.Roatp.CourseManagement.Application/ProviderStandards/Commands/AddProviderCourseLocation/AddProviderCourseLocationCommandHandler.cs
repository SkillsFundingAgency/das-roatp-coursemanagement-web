using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.AddProviderCourseLocation
{
    public class AddProviderCourseLocationCommandHandler : IRequestHandler<AddProviderCourseLocationCommand, Unit>
    {
        private readonly IApiClient _apiClient;
        private readonly ILogger<AddProviderCourseLocationCommandHandler> _logger;

        public AddProviderCourseLocationCommandHandler(ILogger<AddProviderCourseLocationCommandHandler> logger, IApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        public async Task<Unit> Handle(AddProviderCourseLocationCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Command triggered to create provider course location: for ukprn: {ukprn} larsCode: {larsCode} locationNavigationId :{locationNavigationId} by user:{userid}", request.Ukprn, request.LarsCode, request.LocationNavigationId, request.UserId);

            var statusCode = await _apiClient.Post($"providers/{request.Ukprn}/courses/{request.LarsCode}/create-providercourselocation", request);

            if (statusCode != HttpStatusCode.Created)
            {
                _logger.LogError("Failed to create provider course location for ukprn:{ukprn} larsCode: {larsCode} locationNavigationId :{locationNavigationId} by user:{userid}", request.Ukprn, request.LarsCode, request.LocationNavigationId, request.UserId);
                var exception = new System.InvalidOperationException($"Response to create provider course location for ukprn:{request.Ukprn} larsCode: {request.LarsCode} locationNavigationId {request.LocationNavigationId} did not come back with success code");
                throw exception;
            }

            return Unit.Value;
        }
    }
}
