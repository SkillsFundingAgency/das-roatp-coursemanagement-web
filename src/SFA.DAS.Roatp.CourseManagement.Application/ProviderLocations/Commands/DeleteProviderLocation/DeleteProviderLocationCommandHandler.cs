using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Commands.DeleteProviderLocation;

public class DeleteProviderLocationCommandHandler : IRequestHandler<DeleteProviderLocationCommand>
{
    private readonly ILogger<DeleteProviderLocationCommandHandler> _logger;
    private readonly IApiClient _apiClient;

    public DeleteProviderLocationCommandHandler(ILogger<DeleteProviderLocationCommandHandler> logger, IApiClient apiClient)
    {
        _logger = logger;
        _apiClient = apiClient;
    }

    public async Task Handle(DeleteProviderLocationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Command triggered to delete course locations for ukprn:{Ukprn} id:{Id} from user:{UserId}", request.Ukprn, request.Id, request.UserId);

        var statusCode = await _apiClient.Delete($"providers/{request.Ukprn}/locations/{request.Id}?userId={request.UserId}&userDisplayName={request.UserDisplayName}");

        if (statusCode != HttpStatusCode.NoContent)
        {
            _logger.LogError("Failed to delete  location for ukprn:{Ukprn} id:{Id} from user:{Userid}", request.Ukprn, request.Id, request.UserId);
            var exception = new InvalidOperationException("Delete provider location response did not come back with success code");
            throw exception;
        }
    }
}