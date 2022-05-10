using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
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
                standard.StandardUrl = Url.RouteUrl(RouteNames.ViewStandardDetails, new
                        {
                        ukprn,
                        larsCode=standard.LarsCode
                        }
                    );
            }

            return View("~/Views/Standards/ViewStandards.cshtml", model);
        }

        [Route("{ukprn}/standards/{larsCode}", Name = RouteNames.ViewStandardDetails)]
        [HttpGet]
        public async Task<IActionResult> ViewStandard(int ukprn,int larsCode)
        {
            var ukprnFromContext = HttpContext.User.FindFirst(c => c.Type.Equals(ProviderClaims.ProviderUkprn)).Value;
            if (ukprn.ToString() != ukprnFromContext)
            {
                _logger.LogWarning("An attempt has been made to get the provider course details for ukprn {ukprnFromContext} and larsCode {larsCode} using different ukprn [{ukprn}]", ukprnFromContext,larsCode,ukprn);
                return null;
            }

            _logger.LogInformation("Getting Course details for ukprn {ukprn} LarsCode {larsCode}", ukprn, larsCode);

            var result = await _mediator.Send(new GetStandardDetailsQuery(ukprn,larsCode));

            if (result==null)
            {
                _logger.LogWarning("Provider course details not found for ukprn {ukprn} and LarsCode {larsCode}", ukprn, larsCode);
                return null;
            }

            var standardDetails = result.StandardDetails;

            var model = new StandardDetailsViewModel
            {
                BackUrl = Url.RouteUrl(RouteNames.ViewStandards, new
                {
                    ukprn = ukprn,
                }, Request.Scheme, Request.Host.Value),
                CourseName = standardDetails.CourseName,
                Level = standardDetails.Level,
                IFateReferenceNumber = standardDetails.IFateReferenceNumber,
                LarsCode = standardDetails.LarsCode,
                RegulatorName = standardDetails.RegulatorName,
                Sector = standardDetails.Sector,
                Version = standardDetails.Version
            };

            return View("~/Views/Standards/ViewStandardDetails.cshtml", model);
        }
    }
}
