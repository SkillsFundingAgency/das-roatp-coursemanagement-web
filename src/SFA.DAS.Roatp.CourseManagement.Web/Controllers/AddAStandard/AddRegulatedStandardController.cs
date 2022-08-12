using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Queries.GetStandardInformation;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAStandard
{
    [DasAuthorize(new[] { "ProviderFeature.CourseManagement" }, Policy = nameof(PolicyNames.HasProviderAccount))]
    public class AddRegulatedStandardController : AddAStandardControllerBase
    {
        public const string ViewPath = "~/Views/AddAStandard/ConfirmRegulatedStandard.cshtml";

        private readonly IMediator _mediator;
        private readonly ILogger<AddRegulatedStandardController> _logger;

        public AddRegulatedStandardController(ISessionService sessionService, IMediator mediator, ILogger<AddRegulatedStandardController> logger) : base(sessionService)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        [Route("{ukprn}/standards/add/confirm-regulated-standard", Name = RouteNames.GetAddStandardConfirmRegulatedStandard)]
        public async Task<IActionResult> GetConfirmationOfRegulatedStandard()
        {
            var (sessionModel, redirectResult) = GetSessionModelWithEscapeRoute(_logger);
            if (sessionModel == null) return redirectResult;

            var model = await GetViewModel(sessionModel.LarsCode);
            return View(ViewPath, model);
        }

        [HttpPost]
        [Route("{ukprn}/standards/add/confirm-regulated-standard", Name = RouteNames.PostAddStandardConfirmRegulatedStandard)]
        public async Task<IActionResult> SubmitConfirmationOfRegulatedStandard(ConfirmNewRegulatedStandardSubmitModel submitModel)
        {
            var (sessionModel, redirectResult) = GetSessionModelWithEscapeRoute(_logger);
            if (sessionModel == null) return redirectResult;

            if (!ModelState.IsValid)
            {
                var model = await GetViewModel(sessionModel.LarsCode);

                return View(ViewPath, model);
            }

            if (submitModel.IsCorrectStandard == false)
            {
                return RedirectToRouteWithUkprn(RouteNames.GetNeedApprovalToDeliverRegulatedStandard);
            }

            sessionModel.IsConfirmed = true;
            _sessionService.Set(sessionModel, Ukprn.ToString());
            return RedirectToRouteWithUkprn(RouteNames.GetAddStandardAddContactDetails);
        }

        [HttpGet]
        [Route("{ukprn}/standards/needs-approval", Name = RouteNames.GetNeedApprovalToDeliverRegulatedStandard)]
        public ActionResult NeedConfirmationOfRegulatedStandard()
        {
            var model = new NeedApprovalForRegulatedStandardViewModel
            {
                SelectAStandardLink = GetUrlWithUkprn(RouteNames.GetAddStandardSelectStandard)
            };

            return View("~/Views/AddAStandard/NeedApprovalForRegulatedStandard.cshtml", model);
        }

        private async Task<ConfirmNewRegulatedStandardViewModel> GetViewModel(int larsCode)
        {
            var standardInfo = await _mediator.Send(new GetStandardInformationQuery(larsCode));
            var model = new ConfirmNewRegulatedStandardViewModel()
            {
                StandardInformation = standardInfo,
                CancelLink = GetUrlWithUkprn(RouteNames.ViewStandards)
            };
            return model;
        }
    }
}
