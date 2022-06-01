using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Application.Standards.Commands.UpdateContactDetails
{
    public class UpdateProviderCourseContactDetailsCommandHandler : IRequestHandler<UpdateProviderCourseContactDetailsCommand, Unit>
    {
        private readonly ILogger<UpdateProviderCourseContactDetailsCommandHandler> _logger;
        private readonly IApiClient _apiClient;

        public UpdateProviderCourseContactDetailsCommandHandler(ILogger<UpdateProviderCourseContactDetailsCommandHandler> logger, IApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }


        public async Task<Unit> Handle(UpdateProviderCourseContactDetailsCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Update provider course information request for ukprn:{ukprn} LarsCode:{larscode} from user:{userid}", command.Ukprn, command.LarsCode, command.UserId);

            var statusCode = await _apiClient.Post<UpdateProviderCourseContactDetailsCommand>($"providers/{command.Ukprn}/courses/{command.LarsCode}/update-contact-details", command);

            if (statusCode != System.Net.HttpStatusCode.NoContent)
            {
                _logger.LogError("Failed to update provider course information request for ukprn:{ukprn} LarsCode:{larscode} from user:{userid}", command.Ukprn, command.LarsCode, command.UserId);
                throw new InvalidOperationException("Edit contact details response did not come back with success code");
            }

            return Unit.Value;
        }
    }
}
