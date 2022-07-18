using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.UpdateStandardSubRegions
{
    public class UpdateStandardSubRegionsCommandHandler : IRequestHandler<UpdateStandardSubRegionsCommand, Unit>
    {
        private readonly ILogger<UpdateStandardSubRegionsCommandHandler> _logger;
        private readonly IApiClient _apiClient;

        public UpdateStandardSubRegionsCommandHandler(ILogger<UpdateStandardSubRegionsCommandHandler> logger, IApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }


        public async Task<Unit> Handle(UpdateStandardSubRegionsCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Update SubRegions information request for ukprn:{ukprn} LarsCode:{larscode}", command.Ukprn, command.LarsCode);

            var statusCode = await _apiClient.Post<UpdateStandardSubRegionsCommand>($"providers/{command.Ukprn}/courses/{command.LarsCode}/update-standardsubregions", command);

            if (statusCode != System.Net.HttpStatusCode.NoContent)
            {
                _logger.LogError("Failed to update subregions information request for ukprn:{ukprn} LarsCode:{larscode}", command.Ukprn, command.LarsCode);
                throw new InvalidOperationException("Update subregions response did not come back with success code");
            }

            return Unit.Value;
        }
    }
}
