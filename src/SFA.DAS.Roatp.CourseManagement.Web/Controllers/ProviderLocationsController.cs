using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderLocations;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    [Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
    public class ProviderLocationsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProviderLocationsController> _logger;
        public ProviderLocationsController(IMediator mediator, ILogger<ProviderLocationsController> logger)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [Route("{ukprn}/traininglocations", Name = RouteNames.ViewProviderLocations)]
        [HttpGet]
        public async Task<IActionResult> GetProvidersTrainingLocation()
        {
            var ukprn = HttpContext.User.FindFirst(c => c.Type.Equals(ProviderClaims.ProviderUkprn)).Value;

            _logger.LogInformation("Getting Provider Locations for {ukprn}", ukprn);

            var result = await _mediator.Send(new GetProviderLocationQuery(int.Parse(ukprn)));

            var model = new ProviderLocationListViewModel(HttpContext);
         
            if(result == null)
            {
                _logger.LogInformation("Provider Locations data not found for {ukprn}", ukprn);
                return View("~/Views/ProviderLocations/ViewProviderLocations.cshtml", model);
            }

            model.ProviderLocations = result.ProviderLocations.Select(c => (ProviderLocationViewModel)c).ToList();

            return View("~/Views/ProviderLocations/ViewProviderLocations.cshtml", model);
        }
    }
}
