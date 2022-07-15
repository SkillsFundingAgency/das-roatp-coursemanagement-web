using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Application.Standards.Commands.DeleteCourseLocations
{
    public class DeleteProviderCourseLocationCommandHandler : IRequestHandler<DeleteProviderCourseLocationCommand, Unit>
    {
        private readonly ILogger<DeleteProviderCourseLocationCommandHandler> _logger;
        private readonly IApiClient _apiClient;

        public DeleteProviderCourseLocationCommandHandler(ILogger<DeleteProviderCourseLocationCommandHandler> logger, IApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        public async Task<Unit> Handle(DeleteProviderCourseLocationCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Command triggered to remove course location for ukprn:{ukprn} LarsCode:{larscode} from user:{userid}", request.Ukprn, request.LarsCode, request.UserId);

            var statusCode = await _apiClient.Post($"providers/{request.Ukprn}/courses/{request.LarsCode}/location/{request.Id}", request);

            if (statusCode != HttpStatusCode.NoContent)
            {
                _logger.LogError("Failed to remove regional location for ukprn:{ukprn} LarsCode:{larscode} from user:{userid}", request.Ukprn, request.LarsCode, request.UserId);
                var exception = new InvalidOperationException("remove regional location response did not come back with success code");
                throw exception;
            }

            return Unit.Value;
        }
    }
}
