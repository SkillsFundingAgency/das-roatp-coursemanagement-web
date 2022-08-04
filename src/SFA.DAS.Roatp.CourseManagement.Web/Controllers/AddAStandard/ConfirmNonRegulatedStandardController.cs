using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Queries.GetStandardInformation;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAStandard
{
    [DasAuthorize(new[] { "ProviderFeature.CourseManagement" }, Policy = nameof(PolicyNames.HasProviderAccount))]
    public class ConfirmNonRegulatedStandardController : ControllerBase
    {
        public const string ViewPath = "~/Views/AddAStandard/ConfirmNonRegulatedStandard.cshtml";
        private readonly ISessionService _sessionService;
        private readonly IMediator _mediator;
        private readonly ILogger<ConfirmNonRegulatedStandardController> _logger;

        public ConfirmNonRegulatedStandardController(ISessionService sessionService, IMediator mediator, ILogger<ConfirmNonRegulatedStandardController> logger)
        {
            _sessionService = sessionService;
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        [Route("{ukprn}/standards/add/confirm-standard", Name = RouteNames.GetAddStandardConfirmNonRegulatedStandard)]
        public async Task<IActionResult> GetConfirmationOfStandard()
        {
            var sessionModel = _sessionService.Get<StandardSessionModel>(Ukprn.ToString());
            if (sessionModel == null || sessionModel.LarsCode == 0)
            {
                _logger.LogWarning("Confirm standard: Session model is missing, navigating back.");
                return RedirectToRouteWithUkprn(RouteNames.ViewStandards);
            }

            var model = await GetViewModel(sessionModel);

            return View(ViewPath, model);
        }

        [HttpPost]
        [Route("{ukprn}/standards/add/confirm-standard", Name = RouteNames.PostAddStandardConfirmNonRegulatedStandard)]
        public async Task<IActionResult> SubmitConfirmationOfStandard(ConfirmNonRegulatedStandardSubmitModel submitModel)
        {
            var sessionModel = _sessionService.Get<StandardSessionModel>(Ukprn.ToString());
            if (sessionModel == null || sessionModel.LarsCode == 0 || sessionModel.LarsCode != submitModel.LarsCode)
            {
                _logger.LogWarning("Confirm standard: Session model is missing, navigating back.");
                return RedirectToRouteWithUkprn(RouteNames.ViewStandards);
            }

            if (!ModelState.IsValid)
            {
                var model = await GetViewModel(sessionModel);

                return View(ViewPath, model);
            }

            if (submitModel.IsCorrectStandard == false)
            {
                return RedirectToRouteWithUkprn(RouteNames.GetAddStandardSelectStandard);
            }

            return Ok();
        }

        private async Task<ConfirmNonRegulatedStandardViewModel> GetViewModel(StandardSessionModel sessionModel)
        {
            var standardInfo = await _mediator.Send(new GetStandardInformationQuery(sessionModel.LarsCode));
            var model = new ConfirmNonRegulatedStandardViewModel()
            {
                StandardInformation = standardInfo,
                LarsCode = standardInfo.LarsCode,
                CancelLink = GetUrlWithUkprn(RouteNames.ViewStandards)
            };
            return model;
        }
    }
}
