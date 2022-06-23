using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Application.Regions.Commands.UpdateSubRegions
{
    public class UpdateSubRegionsCommandHandler : IRequestHandler<UpdateSubRegionsCommand, Unit>
    {
        private readonly ILogger<UpdateSubRegionsCommandHandler> _logger;
        private readonly IApiClient _apiClient;

        public UpdateSubRegionsCommandHandler(ILogger<UpdateSubRegionsCommandHandler> logger, IApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }


        public async Task<Unit> Handle(UpdateSubRegionsCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Update SubRegions information request for ukprn:{ukprn} LarsCode:{larscode}", command.Ukprn, command.LarsCode);

            var statusCode = await _apiClient.Post<UpdateSubRegionsCommand>($"providers/{command.Ukprn}/courses/{command.LarsCode}/update-subregions", command);

            if (statusCode != System.Net.HttpStatusCode.NoContent)
            {
                _logger.LogError("Failed to update subregions information request for ukprn:{ukprn} LarsCode:{larscode}", command.Ukprn, command.LarsCode);
                throw new InvalidOperationException("Update subregions response did not come back with success code");
            }

            return Unit.Value;
        }
    }
}
