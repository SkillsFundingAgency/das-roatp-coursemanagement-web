using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Commands.CreateProviderLocation
{
    public class CreateProviderLocationCommandHandler : IRequestHandler<CreateProviderLocationCommand, Unit>
    {
        private readonly IApiClient _apiClient;
        private readonly ILogger<CreateProviderLocationCommandHandler> _logger;

        public CreateProviderLocationCommandHandler(ILogger<CreateProviderLocationCommandHandler> logger, IApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        public async Task<Unit> Handle(CreateProviderLocationCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Command triggered to create provider location: {locationName} for ukprn: {ukprn} by user:{userid}", request.LocationName, request.Ukprn, request.UserId);

            var statusCode = await _apiClient.Post($"providers/{request.Ukprn}/locations/create-provider-location", request);

            if (statusCode != HttpStatusCode.Created)
            {
                _logger.LogError("Failed to create provider location: {locationName} for ukprn:{ukprn} by user:{userid}", request.LocationName, request.Ukprn, request.UserId);
                var exception = new System.InvalidOperationException($"Response to create provider location {request.LocationName} for ukprn:{request.Ukprn} did not come back with success code");
                throw exception;
            }

            return Unit.Value;
        }
    }
}
