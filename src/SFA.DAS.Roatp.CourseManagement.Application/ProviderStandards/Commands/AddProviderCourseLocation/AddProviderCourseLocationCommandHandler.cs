using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Commands.CreateProviderLocation
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
            _logger.LogInformation("Command triggered to create provider course location: {locationName} for ukprn: {ukprn} larsCode: {larsCode} by user:{userid}", request.LocationName, request.Ukprn, request.LarsCode, request.UserId);

            var statusCode = await _apiClient.Post($"providers/{request.Ukprn}/locations/create-providercourselocation", request);

            if (statusCode != HttpStatusCode.Created)
            {
                _logger.LogError("Failed to create provider course location: {locationName} for ukprn:{ukprn} larsCode: {larsCode} by user:{userid}", request.LocationName, request.Ukprn, request.LarsCode, request.UserId);
                var exception = new System.InvalidOperationException($"Response to create provider course location {request.LocationName} for ukprn:{request.Ukprn} larsCode: {request.LarsCode} did not come back with success code");
                throw exception;
            }

            return Unit.Value;
        }
    }
}
