using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAllProviderLocations;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderLocations;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    [Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
    public class ProviderLocationsController : ControllerBase
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
            _logger.LogInformation("Getting Provider Locations for {ukprn}", Ukprn);

            var result = await _mediator.Send(new GetAllProviderLocationsQuery(Ukprn));

            var model = new ProviderLocationListViewModel
            {
                BackUrl = Url.RouteUrl(RouteNames.ReviewYourDetails, new {ukprn = Ukprn})
            };


            if (result == null)
            {
                _logger.LogInformation("Provider Locations data not found for {ukprn}", Ukprn);
                return View("~/Views/ProviderLocations/ViewProviderLocations.cshtml", model);
            }

            model.ProviderLocations = result.ProviderLocations.Select(c => (ProviderLocationViewModel)c).ToList();
            foreach(var location in model.ProviderLocations)
            {
                location.RemoveUrl = Url.RouteUrl(RouteNames.GetRemoveTrainingLocation, new { ukprn = Ukprn, providerLocationId = location.ProviderLocationId });
            }
            return View("~/Views/ProviderLocations/ViewProviderLocations.cshtml", model);
        }
    }
}
