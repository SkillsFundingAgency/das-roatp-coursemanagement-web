using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Commands.UpdateProviderLocationDetails;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAllProviderLocations;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetProviderLocationDetails;
using SFA.DAS.Roatp.CourseManagement.Web.Extensions;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderLocations;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers;

[Route("/providers/{ukprn}/locations/{Id}/update-location-details")]
public class EditProviderLocationDetailsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<EditProviderLocationDetailsController> _logger;
    private readonly IValidator<ProviderLocationDetailsSubmitModel> _validator;
    public const string LocationNameNotAvailable = "A location with this name already exists";
    public EditProviderLocationDetailsController(IMediator mediator, ILogger<EditProviderLocationDetailsController> logger, IValidator<ProviderLocationDetailsSubmitModel> validator)
    {
        _logger = logger;
        _mediator = mediator;
        _validator = validator;
    }

    [HttpGet(Name = RouteNames.GetUpdateProviderLocationDetails)]
    public async Task<IActionResult> GetProviderLocationDetails([FromRoute] Guid Id)
    {
        _logger.LogInformation("Getting Provider Location Details for {Ukprn}, {Id}", Ukprn, Id);

        var model = await BuildViewModel(Id);
        return View("~/Views/EditProviderLocation/EditProviderLocationsDetails.cshtml", model);
    }

    private async Task<ProviderLocationViewModel> BuildViewModel(Guid Id)
    {
        var result = await _mediator.Send(new GetProviderLocationDetailsQuery(Ukprn, Id));
        if (result == null || result.ProviderLocation == null)
        {
            _logger.LogInformation("Provider Location Details not found for {Ukprn} and {Id}", Ukprn, Id);
            return new ProviderLocationViewModel
            {
                TrainingVenuesUrl = Url.RouteUrl(RouteNames.GetProviderLocations, new { ukprn = Ukprn }),
            };
        }
        var model = (ProviderLocationViewModel)result.ProviderLocation;
        model.TrainingVenuesUrl = Url.RouteUrl(RouteNames.GetProviderLocations, new { ukprn = Ukprn });
        return model;
    }

    [HttpPost(Name = RouteNames.PostUpdateProviderLocationDetails)]
    public async Task<IActionResult> UpdateProviderLocationDetails(ProviderLocationDetailsSubmitModel model, [FromRoute] Guid Id)
    {
        var validatedResult = _validator.Validate(model);

        if (validatedResult.IsValid) await CheckIfNameIsAvailable(model.LocationName, Id);

        if (!validatedResult.IsValid)
        {
            _logger.LogInformation("Provider Location Details are Invalid to update for {Ukprn} Id:{Id}", Ukprn, Id);
            var viewmodel = await BuildViewModel(Id);
            ModelState.AddValidationErrors(validatedResult.Errors);
            return View("~/Views/EditProviderLocation/EditProviderLocationsDetails.cshtml", viewmodel);
        }

        _logger.LogInformation("Updating Provider Location details for {Ukprn}, {Id}", Ukprn, Id);

        var command = new UpdateProviderLocationDetailsCommand
        {
            Ukprn = Ukprn,
            Id = Id,
            UserId = UserId,
            UserDisplayName = UserDisplayName,
            LocationName = model.LocationName
        };

        await _mediator.Send(command);

        return RedirectToRoute(RouteNames.GetProviderLocationDetails, new { ukprn = Ukprn, Id });
    }

    private async Task CheckIfNameIsAvailable(string locationName, Guid Id)
    {
        var locations = await _mediator.Send(new GetAllProviderLocationsQuery(Ukprn));
        if (locations.ProviderLocations.Where(a => a.NavigationId != Id).Any(l => l.LocationName.ToLower() == locationName.Trim().ToLower()))
        {
            ModelState.AddModelError("LocationName", LocationNameNotAvailable);
        }
    }
}
