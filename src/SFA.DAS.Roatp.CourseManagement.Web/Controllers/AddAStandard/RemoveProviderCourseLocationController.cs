using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using System;
using System.Linq;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAStandard;

[AuthorizeCourseType(CourseType.Apprenticeship)]
public class RemoveProviderCourseLocationController : AddAStandardControllerBase
{
    private readonly ILogger<RemoveProviderCourseLocationController> _logger;
    public RemoveProviderCourseLocationController(ISessionService sessionService, ILogger<RemoveProviderCourseLocationController> logger) : base(sessionService)
    {
        _logger = logger;
    }

    [HttpGet]
    [Route("{ukprn}/standards/remove/locations/{providerLocationId}", Name = RouteNames.GetAddStandardRemoveProviderCourseLocation)]
    public IActionResult RemoveProviderCourseLocation(Guid providerLocationId)
    {
        var (sessionModel, redirectResult) = GetSessionModelWithEscapeRoute(_logger);
        if (sessionModel == null) return redirectResult;

        if (sessionModel.CourseLocations.Any(x => x.ProviderLocationId == providerLocationId))
        {
            var courseLocationToRemove =
                sessionModel.CourseLocations.First(x => x.ProviderLocationId == providerLocationId);
            sessionModel.CourseLocations.Remove(courseLocationToRemove);
            _sessionService.Set(sessionModel);
        }

        return RedirectToRouteWithUkprn(RouteNames.GetNewStandardViewTrainingLocationOptions);
    }
}
