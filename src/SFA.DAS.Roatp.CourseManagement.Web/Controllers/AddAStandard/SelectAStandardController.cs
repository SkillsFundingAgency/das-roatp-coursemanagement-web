using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAvailableProviderStandards;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAStandard
{
    [DasAuthorize(new[] { "ProviderFeature.CourseManagement" }, Policy = nameof(PolicyNames.HasProviderAccount))]
    public class SelectAStandardController : ControllerBase
    {
        public const string ViewPath = "~/Views/AddAStandard/SelectAStandard.cshtml";
        private readonly ILogger<SelectAStandardController> _logger;
        private readonly IMediator _mediator;

        public SelectAStandardController(ILogger<SelectAStandardController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [Route("{ukprn}/standards/new-standard", Name = RouteNames.GetAddStandardSelectStandard)]
        [HttpGet]
        public async Task<IActionResult> SelectAStandard()
        {
            var model = await GetModel();
            return View(ViewPath, model);
        }

        [Route("{ukprn}/standards/new-standard", Name = RouteNames.PostAddStandardSelectStandard)]
        [HttpPost]
        public async Task<IActionResult> SubmitAStandard(SelectAStandardSubmitModel submitModel)
        {
            if (!ModelState.IsValid)
            {
                var model = await GetModel();
                return View(ViewPath, model);
            }
            _logger.LogInformation("Begin of jounrey for ukprn: {ukprn} to add standard {larscode}", Ukprn, submitModel.SelectedLarsCode);

            return Ok();
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
