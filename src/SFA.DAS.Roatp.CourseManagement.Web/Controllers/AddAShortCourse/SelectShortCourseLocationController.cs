using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAllProviderLocations;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Session;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAShortCourse;

[Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
[Route("{ukprn}/courses/{courseType}/new/select-course-location", Name = RouteNames.SelectShortCourseLocation)]
public class SelectShortCourseLocationController(IMediator _mediator, ISessionService _sessionService, ILogger<SelectShortCourseLocationController> _logger) : ControllerBase
{
    public const string ViewPath = "~/Views/AddAShortCourse/SelectShortCourseLocationView.cshtml";

    [HttpGet]
    public IActionResult SelectShortCourseLocation(CourseType courseType)
    {
        var sessionModel = _sessionService.Get<ShortCourseSessionModel>();

        if (sessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

        var model = new SelectShortCourseLocationViewModel()
        {
            ShortCourseLocations = sessionModel.ShortCourseLocations,
            CourseType = courseType,
        };

        return View(ViewPath, model);
    }

    [HttpPost]
    public async Task<IActionResult> SelectShortCourseLocation(SelectShortCourseLocationSubmitModel submitModel, CourseType courseType)
    {
        var sessionModel = _sessionService.Get<ShortCourseSessionModel>();

        if (sessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

        if (!ModelState.IsValid)
        {
            var model = new SelectShortCourseLocationViewModel()
            {
                ShortCourseLocations = submitModel.ShortCourseLocations,
                CourseType = courseType,
            };

            return View(ViewPath, model);
        }

        sessionModel.ShortCourseLocations = submitModel.ShortCourseLocations;

        sessionModel.HasOnlineDeliveryOption = submitModel.ShortCourseLocations.Contains(ShortCourseLocationOption.Online);

        var providerLocations = new List<ProviderLocation>();

        if (submitModel.ShortCourseLocations.Contains(ShortCourseLocationOption.ProviderLocation))
        {
            providerLocations = await GetProviderLocations();
        }

        sessionModel.ProviderLocations = providerLocations.OrderBy(l => l.LocationName).ToList();

        //sessionModel.ShortCourseTrainingVenues = providerLocations.Select(p => (ShortCourseTrainingVenueSessionModel)p).ToList();

        _sessionService.Set(sessionModel);

        if (providerLocations.Count > 0 && submitModel.ShortCourseLocations.Contains(ShortCourseLocationOption.ProviderLocation))
        {
            return RedirectToRoute(RouteNames.SelectShortCourseTrainingVenue, new { ukprn = Ukprn, courseType });
        }

        return RedirectToRoute(RouteNames.SelectShortCourseLocation, new { ukprn = Ukprn, courseType });
    }

    private async Task<List<ProviderLocation>> GetProviderLocations()
    {
        _logger.LogInformation("Getting provider course locations for ukprn {Ukprn}", Ukprn);

        var result = await _mediator.Send(new GetAllProviderLocationsQuery(Ukprn));

        var providerLocationsResponse = result.ProviderLocations.Where(p => p.LocationType == LocationType.Provider).ToList();

        return providerLocationsResponse;
    }
}
