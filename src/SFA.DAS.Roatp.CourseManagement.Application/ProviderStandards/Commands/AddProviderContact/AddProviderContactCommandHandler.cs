using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.AddProviderContact;
public class AddProviderContactCommandHandler : IRequestHandler<AddProviderContactCommand>
{
    private readonly IApiClient _apiClient;
    private readonly ILogger<AddProviderContactCommandHandler> _logger;

    public AddProviderContactCommandHandler(ILogger<AddProviderContactCommandHandler> logger, IApiClient apiClient)
    {
        _logger = logger;
        _apiClient = apiClient;
    }

    public async Task Handle(AddProviderContactCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Command triggered to create provider contact for ukprn: {Ukprn} by user:{UserId}", request.Ukprn, request.UserId);

        var statusCode = await _apiClient.Post($"providers/{request.Ukprn}/contact", request);

        if (statusCode != HttpStatusCode.Created)
        {
            _logger.LogError("Failed to create provider contact for ukprn:{Ukprn} by user:{UserId}", request.Ukprn, request.UserId);
            var exception = new System.InvalidOperationException($"Response to create provider contact for ukprn:{request.Ukprn} did not come back with success code");
            throw exception;
        }
    }
}
