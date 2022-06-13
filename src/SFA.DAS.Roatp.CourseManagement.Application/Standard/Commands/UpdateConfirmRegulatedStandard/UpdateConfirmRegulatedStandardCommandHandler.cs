using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Application.Standard.Commands.UpdateConfirmRegulatedStandard
{
    public class UpdateConfirmRegulatedStandardCommandHandler : IRequestHandler<UpdateConfirmRegulatedStandardCommand, Unit>
    {
        private readonly ILogger<UpdateConfirmRegulatedStandardCommandHandler> _logger;
        private readonly IApiClient _apiClient;

        public UpdateConfirmRegulatedStandardCommandHandler(ILogger<UpdateConfirmRegulatedStandardCommandHandler> logger, IApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }


        public async Task<Unit> Handle(UpdateConfirmRegulatedStandardCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Update confirm regulated standard information request for ukprn:{ukprn} LarsCode:{larscode}", command.Ukprn, command.LarsCode);

            var statusCode = await _apiClient.Post<UpdateConfirmRegulatedStandardCommand>($"providers/{command.Ukprn}/courses/{command.LarsCode}/update-approved-by-regulator", command);

            if (statusCode != System.Net.HttpStatusCode.NoContent)
            {
                _logger.LogError("Failed to confirm regulated standard information request for ukprn:{ukprn} LarsCode:{larscode}", command.Ukprn, command.LarsCode);
                throw new InvalidOperationException("Update confirm regulated standard response did not come back with success code");
            }

            return Unit.Value;
        }
    }
}
