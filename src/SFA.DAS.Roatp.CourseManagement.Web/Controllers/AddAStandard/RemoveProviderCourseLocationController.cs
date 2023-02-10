using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAStandard
{
    [DasAuthorize(Policy = nameof(PolicyNames.HasProviderAccount))]
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
}
