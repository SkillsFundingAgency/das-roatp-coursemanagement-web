using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Commands.UpdateProviderLocationDetails;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAllProviderLocations;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetProviderLocationDetails;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderLocations;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    [DasAuthorize(new[] { "ProviderFeature.CourseManagement" }, Policy = nameof(PolicyNames.HasProviderAccount) )]
    public class EditProviderLocationDetailsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<EditProviderLocationDetailsController> _logger;
        public const string LocationNameNotAvailable = "A location with this name already exists";
        public EditProviderLocationDetailsController(IMediator mediator, ILogger<EditProviderLocationDetailsController> logger)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [Route("/providers/{ukprn}/locations/{Id}/update-location-details", Name = RouteNames.GetUpdateProviderLocationDetails)]
        [HttpGet]
        public async Task<IActionResult> GetProviderLocationDetails([FromRoute] Guid Id)
        {
           _logger.LogInformation("Getting Provider Location Details for {ukprn}, {Id}", Ukprn, Id);

           var model = await BuildViewModel(Id);
           return View("~/Views/EditProviderLocation/EditProviderLocationsDetails.cshtml", model);
        }

        private async Task<ProviderLocationViewModel> BuildViewModel(Guid Id)
        {
            var result = await _mediator.Send(new GetProviderLocationDetailsQuery(Ukprn, Id));
            if (result == null || result.ProviderLocation == null)
            {
                _logger.LogInformation("Provider Location Details not found for {ukprn} and {id}", Ukprn, Id);
                return new ProviderLocationViewModel
                {
                    BackUrl = Url.RouteUrl(RouteNames.GetProviderLocations, new { ukprn = Ukprn }),
                    CancelUrl = Url.RouteUrl(RouteNames.GetProviderLocations, new { ukprn = Ukprn })
                };
            }
            var model = (ProviderLocationViewModel)result.ProviderLocation;
            model.BackUrl = model.CancelUrl = Url.RouteUrl(RouteNames.GetProviderLocations, new { ukprn = Ukprn });
            return model;
        }

        [Route("/providers/{ukprn}/locations/{Id}/update-location-details", Name = RouteNames.PostUpdateProviderLocationDetails)]
        [HttpPost]
        public async Task<IActionResult> UpdateProviderLocationDetails(ProviderLocationDetailsSubmitModel model, [FromRoute] Guid Id)
        {
           if (ModelState.IsValid) await CheckIfNameIsAvailable(model.LocationName, Id);

            if (!ModelState.IsValid)
            {
                _logger.LogInformation("Provider Location Details are Invalid to updatd for ukprn:{ukprn} Id:{id}", Ukprn, Id);
                var viewmodel = await BuildViewModel(Id);
                return View("~/Views/EditProviderLocation/EditProviderLocationsDetails.cshtml", viewmodel);
            }

            _logger.LogInformation("Updating Provider Location details for {ukprn}, {Id}", Ukprn, Id);

            var command = new UpdateProviderLocationDetailsCommand
            {
                Ukprn = Ukprn,
                Id = Id,
                UserId = UserId,
                LocationName = model.LocationName,
                Website = model.Website,
                Email = model.EmailAddress,
                Phone = model.PhoneNumber
            };

            await _mediator.Send(command);

            return RedirectToRoute(RouteNames.GetProviderLocationDetails, new { ukprn = Ukprn, Id });
        }

        private async Task CheckIfNameIsAvailable(string locationName, Guid Id)
        {
            var locations = await _mediator.Send(new GetAllProviderLocationsQuery(Ukprn));
            if (locations.ProviderLocations.Where(a=>a.NavigationId != Id).Any(l => l.LocationName.ToLower() == locationName.Trim().ToLower()))
            {
                ModelState.AddModelError("LocationName", LocationNameNotAvailable);
            }
        }
    }
}
