using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.Standard.Queries;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Commands.UpdateContactDetails;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    [Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
    public class EditCourseContactDetailsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<EditCourseContactDetailsController> _logger;

        public EditCourseContactDetailsController(IMediator mediator, ILogger<EditCourseContactDetailsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [Route("{ukprn}/standards/{larsCode}/edit-contact-details", Name = RouteNames.GetCourseContactDetails)]
        [HttpGet]
        public async Task<IActionResult> Index([FromRoute] int larsCode)
        {
            var ukprn = int.Parse(HttpContext.User.FindFirst(c => c.Type.Equals(ProviderClaims.ProviderUkprn)).Value);

            var result = await _mediator.Send(new GetStandardDetailsQuery(ukprn, larsCode));

            if (result == null || result?.StandardDetails == null)
            {
                _logger.LogError("Standard details not found for ukprn {ukprn} and larscode {larsCode}", ukprn, larsCode);
                throw new InvalidOperationException($"Standard details not found for ukprn {ukprn} and larscode {larsCode}");
            }

            var model = (EditCourseContactDetailsViewModel)result.StandardDetails;

            model.BackLink = model.CancelLink = GetStandardDetailsUrl(ukprn, larsCode);

            return View(model);
        }

        [Route("{ukprn}/standards/{larscode}/edit-contact-details", Name = RouteNames.PostCourseContactDetails)]
        [HttpPost]
        public async Task<IActionResult> Index(EditCourseContactDetailsViewModel model)
        {
            var ukprn = int.Parse(HttpContext.User.FindFirst(c => c.Type.Equals(ProviderClaims.ProviderUkprn)).Value);
            if (!ModelState.IsValid)
            {
                model.BackLink = model.CancelLink = GetStandardDetailsUrl(ukprn, model.LarsCode);
                return View(model);
            }

            var command = (UpdateProviderCourseContactDetailsCommand)model;
            command.Ukprn = ukprn;
            command.UserId = HttpContext.User.FindFirst(c => c.Type.Equals(ProviderClaims.UserId)).Value;

            await _mediator.Send(command);

            return RedirectToRoute(RouteNames.ViewStandardDetails, new {ukprn, model.LarsCode });
        }

        private string GetStandardDetailsUrl(int ukprn, int larsCode) => Url.RouteUrl(RouteNames.ViewStandardDetails, new { ukprn, larsCode });
    }
}
