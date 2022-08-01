using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetProviderLocationDetails;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderLocations;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    [DasAuthorize(new[] { "ProviderFeature.CourseManagement" }, Policy = nameof(PolicyNames.HasProviderAccount) )]
    public class EditProviderLocationDetailsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<EditProviderLocationDetailsController> _logger;
        public EditProviderLocationDetailsController(IMediator mediator, ILogger<EditProviderLocationDetailsController> logger)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [Route("/providers/{ukprn}/locations/{Id}/update", Name = RouteNames.GetProviderLocationDetailsUpdate)]
        [HttpGet]
        public async Task<IActionResult> GetProviderLocationDetails([FromRoute] Guid Id)
        {
            _logger.LogInformation("Getting Provider Location for {ukprn}, {Id}", Ukprn, Id);

            var result = await _mediator.Send(new GetProviderLocationDetailsQuery(Ukprn, Id));

            var model = new ProviderLocationViewModel
            {
                BackUrl = Url.RouteUrl(RouteNames.GetProviderLocations, new { ukprn = Ukprn })
            };

            if (result == null)
            {
                _logger.LogInformation("Provider Location Details not found for {ukprn} and {id}", Ukprn, Id);
                return View("~/Views/EditProviderLocation/ViewProviderLocationsDetails.cshtml", model);
            }

            model = (ProviderLocationViewModel)result.ProviderLocation;
            model.BackUrl = model.CancelUrl = Url.RouteUrl(RouteNames.GetProviderLocations, new { ukprn = Ukprn });
            return View("~/Views/EditProviderLocation/EditProviderLocationsDetails.cshtml", model);
        }

        [Route("/providers/{ukprn}/locations/{Id}/update", Name = RouteNames.PostProviderLocationDetailsUpdate)]
        [HttpPost]
        public async Task<IActionResult> UpdateProviderLocationDetails(ProviderLocationViewModel model)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogInformation("National delivery option was not selected ukprn:{ukprn} larscode:{larscode}", Ukprn, model.NavigationId);
                return View("~/Views/EditProviderLocation/ViewProviderLocationsDetails.cshtml", model);
            }

            _logger.LogInformation("Updating Provider Location details for {ukprn}, {Id}", Ukprn, model.NavigationId);

            //var result = await _mediator.Send(new UpdateProviderLocationDetailsQuery(Ukprn, Id));

            //var model = new ProviderLocationViewModel
            //{
            //    BackUrl = Url.RouteUrl(RouteNames.GetProviderLocations, new { ukprn = Ukprn })
            //};

            //if (result == null)
            //{
            //    _logger.LogInformation("Provider Location Details not found for {ukprn} and {id}", Ukprn, Id);
            //    return View("~/Views/EditProviderLocation/ViewProviderLocationsDetails.cshtml", model);
            //}

            return RedirectToRoute(RouteNames.GetProviderLocationDetails, new { ukprn = Ukprn, Id = model.NavigationId });
        }
    }
}
