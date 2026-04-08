using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;


namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAllProviderStandards;

public class GetAllProviderStandardsQueryHandler : IRequestHandler<GetAllProviderStandardsQuery, GetAllProviderStandardsQueryResult>
{
    private readonly ILogger<GetAllProviderStandardsQueryHandler> _logger;
    private readonly IApiClient _apiClient;
    public GetAllProviderStandardsQueryHandler(IApiClient apiClient, ILogger<GetAllProviderStandardsQueryHandler> logger)
    {
        _logger = logger;
        _apiClient = apiClient;
    }
    public async Task<GetAllProviderStandardsQueryResult> Handle(GetAllProviderStandardsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get Standards request received for Ukprn number {Ukprn}", request.Ukprn);
        var standards = await _apiClient.Get<List<Domain.ApiModels.Standard>>($"providers/{request.Ukprn}/courses?courseType={request.CourseType}");
        if (standards == null)
        {
            _logger.LogInformation("Courses data not found for {Ukprn}", request.Ukprn);
            return new GetAllProviderStandardsQueryResult();
        }

        return new GetAllProviderStandardsQueryResult
        {
            Standards = standards
        };
    }
}
