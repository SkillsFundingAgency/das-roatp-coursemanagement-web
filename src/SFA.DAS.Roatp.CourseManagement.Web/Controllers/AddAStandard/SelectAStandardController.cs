using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAvailableProviderStandards;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Queries.GetStandardInformation;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAStandard
{
    [Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
    public class SelectAStandardController : ControllerBase
    {
        public const string ViewPath = "~/Views/AddAStandard/SelectAStandard.cshtml";
        private readonly ILogger<SelectAStandardController> _logger;
        private readonly IMediator _mediator;
        private readonly ISessionService _sessionService;

        public SelectAStandardController(ILogger<SelectAStandardController> logger, IMediator mediator, ISessionService sessionService)
        {
            _logger = logger;
            _mediator = mediator;
            _sessionService = sessionService;
        }

        [Route("{ukprn}/standards/add/select-standard", Name = RouteNames.GetAddStandardSelectStandard)]
        [HttpGet]
        [ClearSession(nameof(StandardSessionModel))]
        public async Task<IActionResult> SelectAStandard()
        {
            var model = await GetModel();
            return View(ViewPath, model);
        }

        [Route("{ukprn}/standards/add/select-standard", Name = RouteNames.PostAddStandardSelectStandard)]
        [HttpPost]
        public async Task<IActionResult> SubmitAStandard(SelectAStandardSubmitModel submitModel)
        {
            if (!ModelState.IsValid)
            {
                var model = await GetModel();
                return View(ViewPath, model);
            }
            _logger.LogInformation("Begin of journey for ukprn: {ukprn} to add standard {larscode}", Ukprn, submitModel.SelectedLarsCode);

            var sessionModel = new StandardSessionModel { LarsCode = submitModel.SelectedLarsCode };
            _sessionService.Set(sessionModel);

            var standardInformation = await _mediator.Send(new GetStandardInformationQuery(submitModel.SelectedLarsCode));

            if (standardInformation.Regulated && standardInformation.IsRegulatedForProvider)
            {
                return RedirectToRouteWithUkprn(RouteNames.GetAddStandardConfirmRegulatedStandard);
            }

            _logger.LogInformation("Add standard: A non-regulated standard larscode:{larscode} is being added for ukprn:{ukprn} by {userid}", Ukprn, sessionModel.LarsCode, UserId);

            return RedirectToRouteWithUkprn(RouteNames.GetAddStandardConfirmNonRegulatedStandard);
        }

        private async Task<SelectAStandardViewModel> GetModel()
        {
            var result = await _mediator.Send(new GetAvailableProviderStandardsQuery(Ukprn));
            var model = new SelectAStandardViewModel();
            model.Standards = result.AvailableCourses.OrderBy(c => c.Title).Select(s => new SelectListItem($"{s.Title} (Level {s.Level})", s.LarsCode.ToString()));
            model.CancelLink = Url.RouteUrl(RouteNames.ViewStandards, new { Ukprn });
            return model;
        }
    }
}
