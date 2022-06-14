using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.Standard.Queries;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Queries;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    [Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
    public class StandardsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<StandardsController> _logger;
        public StandardsController(IMediator mediator, ILogger<StandardsController> logger)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [Route("{ukprn}/standards", Name = RouteNames.ViewStandards)]
        [HttpGet]
        public async Task<IActionResult> ViewStandards()
        {
            _logger.LogInformation("Getting standards for {ukprn}", Ukprn);

            var result = await _mediator.Send(new GetStandardQuery(Ukprn));

            var model = new StandardListViewModel
            {
                BackUrl = Url.RouteUrl(RouteNames.ReviewYourDetails, new
                {
                    ukprn = Ukprn,
                }, Request.Scheme, Request.Host.Value)
            };

            if (result == null)
            {
                _logger.LogInformation("Standards data not found for {ukprn}", Ukprn);
                return View("~/Views/Standards/ViewStandards.cshtml", model);
            }

            model.Standards = result.Standards.Select(c => (StandardViewModel)c).ToList();

            foreach (var standard in model.Standards)
            {

                standard.StandardUrl = Url.RouteUrl(RouteNames.ViewStandardDetails, new {Ukprn, larsCode = standard.LarsCode});
            }

            return View("~/Views/Standards/ViewStandards.cshtml", model);
        }

        [Route("{ukprn}/standards/{larsCode}", Name = RouteNames.ViewStandardDetails)]
        [HttpGet]
        public async Task<IActionResult> ViewStandard(int larsCode)
        {
            _logger.LogInformation("Getting Course details for ukprn {ukprn} LarsCode {larsCode}", Ukprn, larsCode);

            var result = await _mediator.Send(new GetStandardDetailsQuery(Ukprn, larsCode));

            if (result == null)
            {
                throw new InvalidOperationException();
            }

            var standardDetails = result.StandardDetails;

            var model = (StandardDetailsViewModel)standardDetails;
            model.BackUrl = Url.RouteUrl(RouteNames.ViewStandards, new
            {
                ukprn = Ukprn,
            });

            model.EditContactDetailsUrl = Url.RouteUrl(RouteNames.GetCourseContactDetails, new { Ukprn, larsCode });

            model.ConfirmRegulatedStandardUrl = Url.RouteUrl(RouteNames.ConfirmRegulatedStandard, new { Ukprn, larsCode }, Request.Scheme, Request.Host.Value);

            return View("~/Views/Standards/ViewStandardDetails.cshtml", model);
        }
    }
}
