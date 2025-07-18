using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Commands.DeleteProviderLocation;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetProviderLocationDetails;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetProviderLocationDetailsForDeletion;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderLocations;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers;


[Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
[Route("providers/{ukprn}/locations/{id}")]
public class ProviderLocationDeleteController : ControllerBase
{
    public const string ViewPath = "~/Views/ProviderLocations/ConfirmDeleteProviderLocation.cshtml";
    public const string LocationNotDeletedViewPath = "~/Views/ProviderLocations/LocationNotDeleted.cshtml";
    public const string LocationDeletionConfirmedViewPath = "~/Views/ProviderLocations/LocationDeletedConfirmation.cshtml";
    public const string PageNotFoundPath = "~/Views/Error/PageNotFound.cshtml";
    private readonly IMediator _mediator;
    private readonly ILogger<ProviderLocationDeleteController> _logger;

    public ProviderLocationDeleteController(IMediator mediator, ILogger<ProviderLocationDeleteController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [Route("delete-location", Name = RouteNames.GetConfirmDeleteLocation)]
    [HttpGet]
    public async Task<IActionResult> DeleteProviderLocationConfirmation(int ukprn, Guid id)
    {
        _logger.LogInformation("Getting Location information for ukprn {Ukprn} id {Id}", ukprn, id);

        var result = await _mediator.Send(new GetProviderLocationDetailsForDeletionQuery(ukprn, id));

        if (result == null)
        {
            _logger.LogInformation("provider location not found for ukprn {Ukprn} id {Id}", ukprn, id);
            return View(PageNotFoundPath);
        }

        var standardWithoutOtherLocations = result.ProviderLocation.Standards.Any(standard => !standard.HasOtherVenues);

        if (standardWithoutOtherLocations)
        {
            TempData.Remove(TempDataKeys.ProviderLocationTempDateKey);
            TempData.Add(TempDataKeys.ProviderLocationTempDateKey, JsonSerializer.Serialize(result));
            _logger.LogInformation("provider location for ukprn {Ukprn} id {Id} has one or more standards without other location", ukprn, id);
            return RedirectToRoute(RouteNames.DeleteLocationDenied, new { ukprn, id });
        }

        var model = (ProviderLocationConfirmDeleteViewModel)result.ProviderLocation;
        model.TrainingVenuesUrl = Url.RouteUrl(RouteNames.GetProviderLocations, new { ukprn = Ukprn, Id = id });

        return View(ViewPath, model);
    }

    [Route("delete-location", Name = RouteNames.GetConfirmDeleteLocation)]
    [HttpPost]
    public async Task<IActionResult> PostDeleteProviderLocation(int ukprn, Guid id)
    {
        var command = new DeleteProviderLocationCommand(ukprn, id, UserId, UserDisplayName);
        await _mediator.Send(command);

        TempData.Remove(TempDataKeys.ProviderLocationDeletedBannerTempDateKey);
        TempData.Add(TempDataKeys.ProviderLocationDeletedBannerTempDateKey, true);

        return RedirectToRoute(RouteNames.ConfirmDeleteLocationDone, new { ukprn, id });
    }

    [Route("delete-location-complete", Name = RouteNames.ConfirmDeleteLocationDone)]
    [HttpGet]
    public IActionResult DeleteProviderLocationConfirmed(int ukprn, Guid id)
    {
        TempData.TryGetValue(TempDataKeys.ProviderLocationDeletedBannerTempDateKey, out var result);

        if (result == null)
        {
            return RedirectToRoute(RouteNames.GetProviderLocations, new { ukprn });
        }

        var model = new ProviderLocationDeletedConfirmedViewModel
        {
            TrainingVenuesUrl = Url.RouteUrl(RouteNames.GetProviderLocations, new { ukprn })
        };

        return View(LocationDeletionConfirmedViewPath, model);
    }

    [Route("location-not-deleted", Name = RouteNames.DeleteLocationDenied)]
    [HttpGet]
    public IActionResult ProviderLocationNotDeleted(int ukprn, Guid id)
    {
        TempData.TryGetValue(TempDataKeys.ProviderLocationTempDateKey, out var getProviderLocationDetailsQueryResult);

        if (getProviderLocationDetailsQueryResult == null)
        {
            _logger.LogInformation("provider location not found for ukprn {Ukprn} id {Id}", ukprn, id);
            return View(PageNotFoundPath);
        }

        var result =
            JsonSerializer.Deserialize<GetProviderLocationDetailsQueryResult>(getProviderLocationDetailsQueryResult.ToString()!);

        var model = (ProviderLocationNotDeletedViewModel)result!.ProviderLocation;
        foreach (var standard in model.StandardsWithoutOtherVenues)
        {
            standard.StandardUrl = Url.RouteUrl(RouteNames.GetStandardDetails, new { Ukprn, larsCode = standard.LarsCode });
        }

        return View(LocationNotDeletedViewPath, model);
    }
}