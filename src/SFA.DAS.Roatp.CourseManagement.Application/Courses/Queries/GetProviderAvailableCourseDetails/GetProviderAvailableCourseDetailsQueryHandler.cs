using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;

namespace SFA.DAS.Roatp.CourseManagement.Application.Courses.Queries.GetProviderAvailableCourseDetails;

public class GetProviderAvailableCourseDetailsQueryHandler(IApiClient _apiClient, ILogger<GetProviderAvailableCourseDetailsQueryHandler> _logger) : IRequestHandler<GetProviderAvailableCourseDetailsQuery, GetProviderAvailableCourseDetailsQueryResult>
{
    public async Task<GetProviderAvailableCourseDetailsQueryResult> Handle(GetProviderAvailableCourseDetailsQuery request, CancellationToken cancellationToken)
    {
        var result = await _apiClient.Get<GetProviderAvailableCourseDetailsQueryResult>($"providers/{request.Ukprn}/available-courses/{request.CourseType}/{request.LarsCode}");

        if (result == null)
        {
            _logger.LogInformation("Course details not found for ukprn:{Ukprn} and larscode:{Larscode}", request.Ukprn, request.LarsCode);
            return null;
        }

        return result;
    }
}
