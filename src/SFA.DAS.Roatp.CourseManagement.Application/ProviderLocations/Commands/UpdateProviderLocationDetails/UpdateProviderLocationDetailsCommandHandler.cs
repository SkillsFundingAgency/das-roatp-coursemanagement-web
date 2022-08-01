using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Commands.UpdateProviderLocationDetails
{
    public class UpdateProviderLocationDetailsCommandHandler : IRequestHandler<UpdateProviderLocationDetailsCommand, Unit>
    {
        private readonly ILogger<UpdateProviderLocationDetailsCommandHandler> _logger;
        private readonly IApiClient _apiClient;

        public UpdateProviderLocationDetailsCommandHandler(ILogger<UpdateProviderLocationDetailsCommandHandler> logger, IApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }


        public async Task<Unit> Handle(UpdateProviderLocationDetailsCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Update provider location details request for ukprn:{ukprn} Id:{Id}", command.Ukprn, command.Id);

            var statusCode = await _apiClient.Post($"providers/{command.Ukprn}/locations/{command.Id}/update-provider-location-details", command);

            if (statusCode != System.Net.HttpStatusCode.NoContent)
            {
                _logger.LogError("Failed to update provider location details request for ukprn:{ukprn} Id:{Id}", command.Ukprn, command.Id);
                throw new InvalidOperationException("update provider location details response did not come back with success code");
            }

            return Unit.Value;
        }
    }
}
