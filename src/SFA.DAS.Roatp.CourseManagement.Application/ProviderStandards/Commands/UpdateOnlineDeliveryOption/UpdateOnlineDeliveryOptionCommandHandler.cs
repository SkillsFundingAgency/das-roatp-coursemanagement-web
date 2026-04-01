using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.UpdateOnlineDeliveryOption;
public class UpdateOnlineDeliveryOptionCommandHandler(ILogger<UpdateOnlineDeliveryOptionCommand> _logger, IApiClient _apiClient) : IRequestHandler<UpdateOnlineDeliveryOptionCommand, Unit>
{
    public async Task<Unit> Handle(UpdateOnlineDeliveryOptionCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Update online delivery option information request for ukprn:{Ukprn} LarsCode:{LarsCode}", command.Ukprn, command.LarsCode);

        var statusCode = await _apiClient.Post($"providers/{command.Ukprn}/courses/{command.LarsCode}/update-online-delivery-option", new { command.UserId, command.UserDisplayName, command.HasOnlineDeliveryOption });

        if (statusCode != System.Net.HttpStatusCode.NoContent)
        {
            _logger.LogError("Failed to update online delivery option information request for ukprn:{Ukprn} LarsCode:{LarsCode}", command.Ukprn, command.LarsCode);
            throw new InvalidOperationException("Update online delivery option response did not come back with success code");
        }

        return Unit.Value;
    }
}
