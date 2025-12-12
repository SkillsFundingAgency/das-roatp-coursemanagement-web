using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAStandard
{
    public abstract class AddAStandardControllerBase : ControllerBase
    {
        protected readonly ISessionService _sessionService;
        protected AddAStandardControllerBase(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        protected (StandardSessionModel, IActionResult) GetSessionModelWithEscapeRoute(ILogger logger)
        {
            var sessionModel = _sessionService.Get<StandardSessionModel>();
            if (sessionModel == null || string.IsNullOrWhiteSpace(sessionModel.LarsCode) || (int.TryParse(sessionModel.LarsCode, out var parsedLarsCode) && parsedLarsCode <= 0))
            {
                logger.LogInformation("Session model or larscode is missing, escape route set to review your details.");
                return (null, RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails));
            }
            return (sessionModel, null);
        }
    }
}
