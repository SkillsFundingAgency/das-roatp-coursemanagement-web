using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.AddNationalLocation
{
    public class AddNationalLocationToStandardCommandHandler : IRequestHandler<AddNationalLocationToStandardCommand, Unit>
    {
        private readonly IApiClient _apiClient;
        private readonly ILogger<AddNationalLocationToStandardCommandHandler> _logger;

        public AddNationalLocationToStandardCommandHandler(IApiClient apiClient, ILogger<AddNationalLocationToStandardCommandHandler> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        public async Task<Unit> Handle(AddNationalLocationToStandardCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Command triggered to create national locations for ukprn:{ukprn} LarsCode:{larscode} from user:{userid}", request.Ukprn, request.LarsCode, request.UserId);

            var statusCode = await _apiClient.Post($"providers/{request.Ukprn}/courses/{request.LarsCode}/locations/national", request);

            if (statusCode != HttpStatusCode.NoContent)
            {
                _logger.LogError("Failed to add national location for ukprn:{ukprn} LarsCode:{larscode} from user:{userid}", request.Ukprn, request.LarsCode, request.UserId);
                var exception = new InvalidOperationException($"Response to add national location for ukprn:{request.Ukprn} and larscode: {request.LarsCode} did not come back with success code");
                throw exception;
            }

            return Unit.Value;
        }
    }
}
