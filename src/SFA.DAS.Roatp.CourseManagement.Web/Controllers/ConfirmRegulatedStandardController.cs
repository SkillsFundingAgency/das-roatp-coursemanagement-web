using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.Standard.Queries;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using System;
using System.Threading.Tasks;


namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    [Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
    public class ConfirmRegulatedStandardController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<EditCourseContactDetailsController> _logger;

        public ConfirmRegulatedStandardController(IMediator mediator, ILogger<EditCourseContactDetailsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        [Route("{ukprn}/standards/{larsCode}/confirm-regulated-standard", Name = RouteNames.ConfirmRegulatedStandard)]
        [HttpGet]
        public async Task<IActionResult> ConfirmRegulatedStandard([FromRoute] int larsCode)
        {
            var ukprn = Ukprn;
            _logger.LogInformation("Getting Course details for ukprn {ukprn} LarsCode {larsCode}", ukprn, larsCode);

            var result = await _mediator.Send(new GetStandardDetailsQuery(ukprn, larsCode));

            if (result == null || result?.StandardDetails == null)
            {
                _logger.LogError("Standard details not found for ukprn {ukprn} and larscode {larsCode}", ukprn, larsCode);
                throw new InvalidOperationException($"Standard details not found for ukprn {ukprn} and larscode {larsCode}");
            }

            var model = (ConfirmRegulatedStandardViewModel)result.StandardDetails;
            model.BackLink = model.CancelLink = GetStandardDetailsUrl(ukprn, larsCode);

            return View("~/Views/Standards/ConfirmRegulatedStandard.cshtml", model);
        }
        [Route("{ukprn}/standards/{larsCode}/confirm-regulated-standard", Name = RouteNames.PostConfirmRegulatedStandard)]
        [HttpPost]
        public IActionResult SubmitConfirmRegulatedStandard(ConfirmRegulatedStandardViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.BackLink = model.CancelLink = GetStandardDetailsUrl(Ukprn, model.LarsCode);
                return View("~/Views/Standards/ConfirmRegulatedStandard.cshtml", model);
            }

            return RedirectToRoute(RouteNames.ViewStandardDetails, new { Ukprn, model.LarsCode });
        }
        private string GetStandardDetailsUrl(int ukprn, int larsCode) => Url.RouteUrl(RouteNames.ViewStandardDetails, new { ukprn, larsCode });
    }
}
