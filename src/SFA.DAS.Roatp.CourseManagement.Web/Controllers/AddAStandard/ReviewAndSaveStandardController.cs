using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.AddProviderCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAStandard
{
    [DasAuthorize( Policy = nameof(PolicyNames.HasProviderAccount))]
    public class ReviewAndSaveStandardController : AddAStandardControllerBase
    {
        public const string ViewPath = "~/Views/AddAStandard/ReviewAndSaveStandard.cshtml";
        private readonly ILogger<ReviewAndSaveStandardController> _logger;
        private readonly IMediator _mediator;

        public ReviewAndSaveStandardController(ILogger<ReviewAndSaveStandardController> logger, IMediator mediator, ISessionService sessionService) : base(sessionService)
        {
            _logger = logger;
            _mediator = mediator;
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
        public async Task<IActionResult> SaveStandard()
        {
            var (sessionModel, redirectResult) = GetSessionModelWithEscapeRoute(_logger);
            if (sessionModel == null) return redirectResult;

            AddProviderCourseCommand command = sessionModel;
            command.UserId = UserId;
            command.UserDisplayName = UserDisplayName;
            command.Ukprn = Ukprn;

            await _mediator.Send(command);
            TempData.Add(TempDataKeys.ShowStandardAddBannerTempDataKey, true);

            return RedirectToRouteWithUkprn(RouteNames.ViewStandards);
        }
    }
}
