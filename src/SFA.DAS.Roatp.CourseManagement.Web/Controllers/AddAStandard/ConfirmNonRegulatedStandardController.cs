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
    public class ConfirmNonRegulatedStandardController : AddAStandardControllerBase
    {
        public const string ViewPath = "~/Views/AddAStandard/ConfirmNonRegulatedStandard.cshtml";
        
        private readonly IMediator _mediator;
        private readonly ILogger<ConfirmNonRegulatedStandardController> _logger;

        public ConfirmNonRegulatedStandardController(ISessionService sessionService, IMediator mediator, ILogger<ConfirmNonRegulatedStandardController> logger) : base(sessionService)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        [Route("{ukprn}/standards/add/confirm-standard", Name = RouteNames.GetAddStandardConfirmNonRegulatedStandard)]
        public async Task<IActionResult> GetConfirmationOfStandard()
        {
            var (sessionModel, redirectResult) = GetSessionModelWithEscapeRoute(_logger);
            if (sessionModel == null) return redirectResult;

            var model = await GetViewModel(sessionModel.LarsCode);

            return View(ViewPath, model);
        }

        [HttpPost]
        [Route("{ukprn}/standards/add/confirm-standard", Name = RouteNames.PostAddStandardConfirmNonRegulatedStandard)]
        public async Task<IActionResult> SubmitConfirmationOfStandard(ConfirmNonRegulatedStandardSubmitModel submitModel)
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
                return RedirectToRouteWithUkprn(RouteNames.GetAddStandardSelectStandard);
            }

            sessionModel.IsConfirmed = true;
            sessionModel.StandardInformation = await _mediator.Send(new GetStandardInformationQuery(sessionModel.LarsCode));
            _sessionService.Set(sessionModel, Ukprn.ToString());

            _logger.LogInformation("Add standard: Non-regulated standard confirmed for ukprn:{ukprn} larscode:{larscode}", Ukprn, sessionModel.LarsCode);
           
            return RedirectToRouteWithUkprn(RouteNames.GetAddStandardAddContactDetails);
        }

        private async Task<ConfirmNonRegulatedStandardViewModel> GetViewModel(int larsCode)
        {
            var standardInfo = await _mediator.Send(new GetStandardInformationQuery(larsCode));
            var model = new ConfirmNonRegulatedStandardViewModel()
            {
                StandardInformation = standardInfo,
                CancelLink = GetUrlWithUkprn(RouteNames.ViewStandards)
            };
            return model;
        }
    }
}
