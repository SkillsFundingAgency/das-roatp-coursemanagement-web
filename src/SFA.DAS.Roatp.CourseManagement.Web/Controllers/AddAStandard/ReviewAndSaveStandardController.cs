using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAStandard
{
    [DasAuthorize(new[] { "ProviderFeature.CourseManagement" }, Policy = nameof(PolicyNames.HasProviderAccount))]
    public class ReviewAndSaveStandardController : AddAStandardControllerBase
    {
        public const string ViewPath = "~/Views/AddAStandard/ReviewAndSaveStandard.cshtml";
        private readonly ILogger<ReviewAndSaveStandardController> _logger;

        public ReviewAndSaveStandardController(ILogger<ReviewAndSaveStandardController> logger, ISessionService sessionService) : base(sessionService)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("{ukprn}/standards/add/save-standard", Name = RouteNames.GetAddStandardReviewStandard)]
        public IActionResult ReviewStandard()
        {
            var (sessionModel, redirectResult) = GetSessionModelWithEscapeRoute(_logger);
            if (sessionModel == null) return redirectResult;
            sessionModel.CancelLink = GetUrlWithUkprn(RouteNames.ViewStandards);
            return View(ViewPath, sessionModel);
        }

        [HttpPost]
        [Route("{ukprn}/standards/add/save-standard", Name = RouteNames.PostAddStandardReviewStandard)]
        public IActionResult SaveStandard()
        {
            return RedirectToRouteWithUkprn(RouteNames.ViewStandards);
        }
    }
}
