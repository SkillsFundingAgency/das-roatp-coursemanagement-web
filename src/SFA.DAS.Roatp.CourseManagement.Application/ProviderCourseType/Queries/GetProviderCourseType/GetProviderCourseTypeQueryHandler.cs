using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderContact.Queries;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderCourseType.Queries.GetProviderCourseType;
public class GetProviderCourseTypeQueryHandler(IApiClient _apiClient, ILogger<GetLatestProviderContactQueryHandler> _logger) : IRequestHandler<GetProviderCourseTypeQuery, List<CourseTypeModel>>
{
    public async Task<List<CourseTypeModel>> Handle(GetProviderCourseTypeQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get Provider Course Type request received for ukprn {Ukprn}", request.Ukprn);

        var providerCourseType = await _apiClient.Get<List<CourseTypeModel>>($"providers/{request.Ukprn}/course-types");

        return providerCourseType;
    }
}
