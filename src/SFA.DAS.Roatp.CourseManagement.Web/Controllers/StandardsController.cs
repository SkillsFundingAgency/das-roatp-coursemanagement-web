using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.Standard.Queries;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Queries;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    [Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
    public class StandardsController : Controller
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
            var ukprn = HttpContext.User.FindFirst(c => c.Type.Equals(ProviderClaims.ProviderUkprn)).Value;
            _logger.LogInformation("Getting standards for {ukprn}", ukprn);

            var result = await _mediator.Send(new GetStandardQuery(int.Parse(ukprn)));

            var model = new StandardListViewModel
            {
                BackUrl = Url.RouteUrl(RouteNames.ReviewYourDetails, new
                {
                    ukprn = ukprn,
                }, Request.Scheme, Request.Host.Value)
            };

            if (result == null)
            {
                _logger.LogInformation("Standards data not found for {ukprn}", ukprn);
                return View("~/Views/Standards/ViewStandards.cshtml", model);
            }

            model.Standards = result.Standards.Select(c => (StandardViewModel)c).ToList();

            foreach (var standard in model.Standards)
            {

                standard.StandardUrl = Url.RouteUrl(RouteNames.ViewStandardDetails, 
                    new
                            {
                                ukprn,
                                larsCode=standard.LarsCode,
                                providerCourseId = standard.ProviderCourseId,
                    }
                    );
            }

            return View("~/Views/Standards/ViewStandards.cshtml", model);
        }

        [Route("{ukprn}/standards/{larsCode}/providerCourseLocations/{providerCourseId}", Name = RouteNames.ViewStandardDetails)]
        [HttpGet]
        public async Task<IActionResult> ViewStandard(int larsCode, int providerCourseId)
        {
            var ukprn = HttpContext.User.FindFirst(c => c.Type.Equals(ProviderClaims.ProviderUkprn)).Value;
            _logger.LogInformation("Getting Course details for ukprn {ukprn} LarsCode {larsCode} providerCourseId {providerCourseId}", ukprn, larsCode, providerCourseId);

            var result = await _mediator.Send(new GetStandardDetailsQuery(int.Parse(ukprn),larsCode, providerCourseId));

            if (result==null)
            {
                // SHUTTER-PAGE will need redirect back to shutter page
                return null;
            }

            var standardDetails = result.StandardDetails;

            var model =  (StandardDetailsViewModel)standardDetails;
            model.BackUrl = Url.RouteUrl(RouteNames.ViewStandards, new
            {
                ukprn = ukprn,
            }, Request.Scheme, Request.Host.Value);

            return View("~/Views/Standards/ViewStandardDetails.cshtml", model);
        }
    }
}
