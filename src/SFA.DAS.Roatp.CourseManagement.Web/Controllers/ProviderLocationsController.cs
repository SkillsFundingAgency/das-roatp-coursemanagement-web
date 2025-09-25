using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAllProviderLocations;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderLocations;

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

        [Route("{ukprn}/manage-training-locations", Name = RouteNames.GetProviderLocations)]
        [HttpGet]
        [ClearSession(SessionKeys.SelectedPostcode)]
        public async Task<IActionResult> GetProvidersTrainingLocations()
        {
            _logger.LogInformation("Getting Provider Locations for {ukprn}", Ukprn);

            var result = await _mediator.Send(new GetAllProviderLocationsQuery(Ukprn));

            var model = new ProviderLocationListViewModel
            {
                AddTrainingLocationLink = Url.RouteUrl(RouteNames.SearchAddress, new { ukprn = Ukprn })
            };

            if (result == null)
            {
                _logger.LogInformation("Provider Locations data not found for {ukprn}", Ukprn);
                return View("~/Views/ProviderLocations/ViewProviderLocations.cshtml", model);
            }

            TempData.TryGetValue(TempDataKeys.ShowVenueAddBannerTempDataKey, out var showAddBanner);
            if (showAddBanner != null)
            {
                model.ShowNotificationBannerAddVenue = true;
                model.ManageYourStandardsUrl = GetUrlWithUkprn(RouteNames.ViewStandards);
            }


            model.ProviderLocations = result.ProviderLocations.Select(c => (ProviderLocationViewModel)c).OrderBy(l => l.LocationName).ToList();
            model.ProviderLocations.ForEach(l => l.VenueNameUrl = Url.RouteUrl(RouteNames.GetProviderLocationDetails, new { ukprn = Ukprn, Id = l.NavigationId }));

            return View("~/Views/ProviderLocations/ViewProviderLocations.cshtml", model);
        }
    }
}
