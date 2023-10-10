using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Application.Providers.Queries.GetProviderAccount
{
    public class GetProviderAccountStatusQueryHandler : IRequestHandler<GetProviderAccountStatusQuery, GetProviderAccountStatusResult>
    {
        private readonly ILogger<GetProviderAccountStatusQueryHandler> _logger;
        private readonly IApiClient _apiClient;
        public GetProviderAccountStatusQueryHandler(IApiClient apiClient, ILogger<GetProviderAccountStatusQueryHandler> logger)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        public async Task<GetProviderAccountStatusResult> Handle(GetProviderAccountStatusQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Get Provider Account Status received for Ukprn number {ukprn}", request.Ukprn);

            var response = await _apiClient.Get<Domain.ApiModels.ProviderAccountResponse>($"providerAccounts/{request.Ukprn}");

            return new GetProviderAccountStatusResult
            {
                CanAccessService = response is {CanAccessService: true}
            };
        }
    }
}
