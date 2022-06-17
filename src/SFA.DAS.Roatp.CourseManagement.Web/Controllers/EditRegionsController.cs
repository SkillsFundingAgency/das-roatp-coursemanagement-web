using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.Regions.Queries.GetAllRegions;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    [Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
    public class EditRegionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<EditRegionsController> _logger;
        public EditRegionsController(IMediator mediator, ILogger<EditRegionsController> logger)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [Route("{ukprn}/standards/{larsCode}/regions", Name = RouteNames.GetRegions)]
        [HttpGet]
        public async Task<IActionResult> GetAllRegions()
        {
            _logger.LogInformation("Getting All Regions");

            var result = await _mediator.Send(new GetAllRegionsQuery());

            var model = new RegionsViewModel
            {
                BackUrl = Url.RouteUrl(RouteNames.ViewStandardDetails, new {ukprn = Ukprn})
            };


            if (result == null)
            {
                _logger.LogInformation("Regions not found");
                return Redirect($"Error/{HttpStatusCode.NotFound}");
            }

            model.AllRegions = result.Regions.Select(c => (RegionViewModel)c).ToList();

            return View("~/Views/Standards/EditRegions.cshtml", model);
        }
    }
}
