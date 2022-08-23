using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Application.Standards.Commands.DeleteCourseLocations
{
    public class DeleteProviderCourseCommandHandler : IRequestHandler<DeleteProviderCourseCommand, Unit>
    {
        private readonly ILogger<DeleteProviderCourseCommandHandler> _logger;
        private readonly IApiClient _apiClient;

        public DeleteProviderCourseCommandHandler(ILogger<DeleteProviderCourseCommandHandler> logger, IApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        public async Task<Unit> Handle(DeleteProviderCourseCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Command triggered to Delete standard for ukprn:{ukprn} LarsCode:{larscode} from user:{userid}", request.Ukprn, request.LarsCode, request.UserId);

            var statusCode = await _apiClient.Post($"providers/{request.Ukprn}/courses/{request.LarsCode}/delete", request);

            if (statusCode != HttpStatusCode.NoContent)
            {
                _logger.LogError("Failed to delete standard for ukprn:{ukprn} LarsCode:{larscode} from user:{userid}", request.Ukprn, request.LarsCode, request.UserId);
                var exception = new InvalidOperationException("Delete standard response did not come back with success code");
                throw exception;
            }

            return Unit.Value;
        }
    }
}
