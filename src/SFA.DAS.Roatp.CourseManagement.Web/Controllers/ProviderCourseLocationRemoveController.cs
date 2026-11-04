using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Commands.DeleteCourseLocations;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;


namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers;

[AuthorizeCourseType(CourseType.Apprenticeship)]
[CheckCourseType(CourseType.Apprenticeship)]
[Route("{ukprn}/standards/{larsCode}/providerlocations/{id}/remove-providerlocation")]
public class ProviderCourseLocationRemoveController : ControllerBase
{
    private const string ViewName = "~/Views/ProviderCourseLocations/RemoveProviderCourseLocation.cshtml";
    private readonly IMediator _mediator;
    private readonly ILogger<ProviderCourseLocationRemoveController> _logger;

    public ProviderCourseLocationRemoveController(IMediator mediator, ILogger<ProviderCourseLocationRemoveController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet(Name = RouteNames.GetRemoveProviderCourseLocation)]
    public async Task<IActionResult> GetProviderCourseLocation(string larsCode, Guid id)
    {
        _logger.LogInformation("Getting Provider Course Location for ukprn {Ukprn} ", Ukprn);

        var result = await _mediator.Send(new GetProviderCourseLocationsQuery(Ukprn, larsCode));

        if (result == null)
        {
            var message = $"Provider Course Location not found for ukprn {Ukprn} and larscode {larsCode}";
            _logger.LogError("Provider Course Location not found for ukprn {Ukprn} and larscode {LarsCode}", Ukprn, larsCode);
            throw new InvalidOperationException(message);
        }

        ProviderCourseLocation pcl = result.ProviderCourseLocations.Find(l => l.Id == id);
        if (pcl == null) return NotFound();

        ProviderCourseLocationRemoveViewModel model = new() { Id = id, LarsCode = larsCode, Ukprn = Ukprn };
        model.DeliveryMethod.HasBlockReleaseDeliveryOption = pcl.HasBlockReleaseDeliveryOption;
        model.DeliveryMethod.HasDayReleaseDeliveryOption = pcl.HasDayReleaseDeliveryOption;
        model.LocationName = pcl.LocationName;
        model.IsOnlyLocation = result.ProviderCourseLocations.Count == 1;

        return View(ViewName, model);
    }

    [HttpPost(Name = RouteNames.PostRemoveProviderCourseLocation)]
    public async Task<IActionResult> RemoveProviderCourseLocation(ProviderCourseLocationRemoveViewModel model)
    {
        if (model.IsOnlyLocation)
        {
            ModelState.AddModelError("LocationName", "You cannot remove the only location for a standard.");
            return View(ViewName, model);
        }
        var command = new DeleteProviderCourseLocationCommand(Ukprn, model.LarsCode, model.Id, UserId, UserDisplayName);
        await _mediator.Send(command);

        return RedirectToRoute(RouteNames.GetProviderCourseLocations, new { Ukprn, larsCode = model.LarsCode });
    }
}
