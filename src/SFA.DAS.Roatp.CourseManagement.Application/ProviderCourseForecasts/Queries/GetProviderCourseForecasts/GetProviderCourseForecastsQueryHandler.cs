using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderCourseForecasts.Queries.GetProviderCourseForecasts;

public class GetProviderCourseForecastsQueryHandler(IApiClient _apiClient, ILogger<GetProviderCourseForecastsQueryHandler> _logger) : IRequestHandler<GetProviderCourseForecastsQuery, GetProviderCourseForecastsQueryResult>
{
    public async Task<GetProviderCourseForecastsQueryResult> Handle(GetProviderCourseForecastsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get Provider Course Forecasts request received for ukprn {Ukprn} and lars code {LarsCode}", request.Ukprn, request.LarsCode);
        var forecasts = await _apiClient.Get<GetProviderCourseForecastsQueryResult>($"providers/{request.Ukprn}/courses/{request.LarsCode}/forecasts");
        return forecasts;
    }
}
