using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.AddProviderCourse
{
    public class AddProviderCourseCommandHandler : IRequestHandler<AddProviderCourseCommand, Unit>
    {
        private readonly IApiClient _apiClient;
        private readonly ILogger<AddProviderCourseCommandHandler> _logger;

        public AddProviderCourseCommandHandler(ILogger<AddProviderCourseCommandHandler> logger, IApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        public async Task<Unit> Handle(AddProviderCourseCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Command triggered to create course: {larscode} for ukprn: {ukprn} by user:{userid}", request.LarsCode, request.Ukprn, request.UserId);

            var statusCode = await _apiClient.Post($"providers/{request.Ukprn}/courses/{request.LarsCode}/create", request);

            if (statusCode != HttpStatusCode.Created)
            {
                _logger.LogError("Failed to create provider course: {larscode} for ukprn:{ukprn} by user:{userid}", request.LarsCode, request.Ukprn, request.UserId);
                var exception = new System.InvalidOperationException($"Response to create provider course {request.LarsCode} for ukprn:{request.Ukprn} did not come back with success code");
                throw exception;
            }

            return Unit.Value;
        }
    }
}
