using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Application.Standards.Commands.DeleteCourseLocations
{
    public class DeleteCourseLocationsCommandHandler : IRequestHandler<DeleteCourseLocationsCommand, Unit>
    {
        private readonly ILogger<DeleteCourseLocationsCommandHandler> _logger;
        private readonly IApiClient _apiClient;

        public DeleteCourseLocationsCommandHandler(ILogger<DeleteCourseLocationsCommandHandler> logger, IApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        public async Task<Unit> Handle(DeleteCourseLocationsCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Command triggered to delete course locations for ukprn:{ukprn} LarsCode:{larscode} from user:{userid}", request.Ukprn, request.LarsCode, request.UserId);

            var statusCode = await _apiClient.Post($"providers/{request.Ukprn}/courses/{request.LarsCode}/bulk-delete-course-locations", request);

            if (statusCode != HttpStatusCode.NoContent)
            {
                _logger.LogError("Failed to delete regional locations for ukprn:{ukprn} LarsCode:{larscode} from user:{userid}", request.Ukprn, request.LarsCode, request.UserId);
                var exception = new InvalidOperationException("Delete regional locations response did not come back with success code");
                throw exception;
            }

            return Unit.Value;
        }
    }
}
