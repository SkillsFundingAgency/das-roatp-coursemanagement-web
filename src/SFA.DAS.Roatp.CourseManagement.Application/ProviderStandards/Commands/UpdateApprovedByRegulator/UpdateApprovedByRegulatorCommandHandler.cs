using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.UpdateApprovedByRegulator
{
    public class UpdateApprovedByRegulatorCommandHandler : IRequestHandler<UpdateApprovedByRegulatorCommand, Unit>
    {
        private readonly ILogger<UpdateApprovedByRegulatorCommandHandler> _logger;
        private readonly IApiClient _apiClient;

        public UpdateApprovedByRegulatorCommandHandler(ILogger<UpdateApprovedByRegulatorCommandHandler> logger, IApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }


        public async Task<Unit> Handle(UpdateApprovedByRegulatorCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Update approved by regulator standard information request for ukprn:{ukprn} LarsCode:{larscode}", command.Ukprn, command.LarsCode);

            var statusCode = await _apiClient.Post($"providers/{command.Ukprn}/courses/{command.LarsCode}/update-approved-by-regulator", command);

            if (statusCode != System.Net.HttpStatusCode.NoContent)
            {
                _logger.LogError("Failed to update approved by regulator standar information request for ukprn:{ukprn} LarsCode:{larscode}", command.Ukprn, command.LarsCode);
                throw new InvalidOperationException("Update approved by regulator standard response did not come back with success code");
            }

            return Unit.Value;
        }
    }
}
