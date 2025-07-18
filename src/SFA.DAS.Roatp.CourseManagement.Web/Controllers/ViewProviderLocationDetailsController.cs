using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetProviderLocationDetails;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderLocations;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    [Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
    public class ViewProviderLocationDetailsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ViewProviderLocationDetailsController> _logger;
        public ViewProviderLocationDetailsController(IMediator mediator, ILogger<ViewProviderLocationDetailsController> logger)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [Route("/providers/{ukprn}/locations/{Id}", Name = RouteNames.GetProviderLocationDetails)]
        [HttpGet]
        public async Task<IActionResult> GetProviderLocationDetails([FromRoute] Guid Id)
        {
            _logger.LogInformation("Getting Provider Location for {Ukprn}, {Id}", Ukprn, Id);

            var result = await _mediator.Send(new GetProviderLocationDetailsQuery(Ukprn, Id));

            var model = new ProviderLocationViewModel
            {
                BackUrl = Url.RouteUrl(RouteNames.GetProviderLocations, new { ukprn = Ukprn })
            };

            if (result == null || result.ProviderLocation == null)
            {
                _logger.LogInformation("Provider Location Details not found for {Ukprn} and {Id}", Ukprn, Id);
                return View("~/Views/EditProviderLocation/ViewProviderLocationsDetails.cshtml", model);
            }

            model = (ProviderLocationViewModel)result.ProviderLocation;
            model.BackUrl = Url.RouteUrl(RouteNames.GetProviderLocations, new { ukprn = Ukprn });
            model.UpdateContactDetailsUrl = Url.RouteUrl(RouteNames.GetUpdateProviderLocationDetails, new { ukprn = Ukprn, Id });

            foreach (var standard in model.Standards)
            {
                standard.StandardUrl = Url.RouteUrl(RouteNames.GetStandardDetails,
                    new { Ukprn, larsCode = standard.LarsCode });
            }

            return View("~/Views/EditProviderLocation/ViewProviderLocationsDetails.cshtml", model);
        }
    }
}
