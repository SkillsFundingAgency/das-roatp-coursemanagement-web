using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderCourseForecasts.Commands.UpsertProviderCourseForecasts;

public class UpsertProviderCourseForecastsCommandHandler(IApiClient _apiClient) : IRequestHandler<UpsertProviderCourseForecastsCommand>
{
    public async Task Handle(UpsertProviderCourseForecastsCommand request, CancellationToken cancellationToken)
    {
        await _apiClient.Post($"providers/{request.Ukprn}/courses/{request.LarsCode}/forecasts", request.Forecasts);
    }
}
