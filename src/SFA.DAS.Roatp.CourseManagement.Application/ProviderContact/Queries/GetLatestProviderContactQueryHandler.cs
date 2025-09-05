using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderContact.Queries;

public class GetLatestProviderContactQueryHandler(IApiClient _apiClient, ILogger<GetLatestProviderContactQueryHandler> _logger) : IRequestHandler<GetLatestProviderContactQuery, GetLatestProviderContactQueryResult>
{
    public async Task<GetLatestProviderContactQueryResult> Handle(GetLatestProviderContactQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get Provider Contact details request received for ukprn {Ukprn}", request.Ukprn);
        var standardDetails = await _apiClient.Get<Domain.ApiModels.ProviderContactModel>($"providers/{request.Ukprn}/contact");

        if (standardDetails == null)
        {
            _logger.LogInformation("Provider contact details not found for ukprn {Ukprn}", request.Ukprn);
            return null;
        }

        var result = new GetLatestProviderContactQueryResult
        {
            EmailAddress = standardDetails.EmailAddress,
            PhoneNumber = standardDetails.PhoneNumber
        };

        return result;
    }
}