using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAvailableProviderStandards;

public class GetAvailableProviderStandardsQueryHandler : IRequestHandler<GetAvailableProviderStandardsQuery, GetAvailableProviderStandardsQueryResult>
{
    private readonly IApiClient _apiClient;
    private readonly ILogger<GetAvailableProviderStandardsQueryHandler> _logger;

    public GetAvailableProviderStandardsQueryHandler(IApiClient apiClient, ILogger<GetAvailableProviderStandardsQueryHandler> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    public async Task<GetAvailableProviderStandardsQueryResult> Handle(GetAvailableProviderStandardsQuery request, CancellationToken cancellationToken)
    {
        var result = await _apiClient.Get<GetAvailableProviderStandardsQueryResult>($"providers/{request.Ukprn}/available-courses/{request.CourseType}");
        _logger.LogInformation("Found {count} available courses for ukprn: {ukprn}", result.AvailableCourses.Count, request.Ukprn);
        return result;
    }
}