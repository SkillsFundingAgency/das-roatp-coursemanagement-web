using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderCourseLocations;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    [Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
    public class ProviderCourseLocationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProviderCourseLocationController> _logger;

        public ProviderCourseLocationController(IMediator mediator, ILogger<ProviderCourseLocationController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [Route("{ukprn}/standards/{larsCode}/providercourselocations", Name = RouteNames.GetProviderCourseLocations)]
        [HttpGet]
        public async Task<IActionResult> GetProviderCourseLocations([FromRoute] int larsCode)
        {
            _logger.LogInformation("Getting Provider Course Location for ukprn {ukprn} ", Ukprn);

            var result = await _mediator.Send(new GetProviderCourseLocationsQuery(Ukprn, larsCode));

            if (result == null)
            {
                var message = $"Provider Course Location not found for ukprn {Ukprn} and larscode {larsCode}";
                _logger.LogError(message);
                throw new InvalidOperationException(message);
            }

            var model = new ProviderCourseLocationListViewModel();
            model.ProviderCourseLocations = result.ProviderCourseLocations.Select(x => (ProviderCourseLocationViewModel)x).ToList();
            model.BackLink = model.CancelLink = GetStandardDetailsUrl(model.LarsCode);

            return View("~/Views/Standards/ProviderCourseLocations.cshtml", model);
        }
    }
}
