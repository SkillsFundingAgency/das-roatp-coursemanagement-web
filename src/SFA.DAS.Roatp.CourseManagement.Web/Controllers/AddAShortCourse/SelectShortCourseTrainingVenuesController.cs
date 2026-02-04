using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAllProviderLocations;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAShortCourse;

[Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
[Route("{ukprn}/courses/{courseType}/new/select-training-venues", Name = RouteNames.SelectShortCourseTrainingVenue)]
public class SelectShortCourseTrainingVenuesController(ISessionService _sessionService, IMediator _mediator, ILogger<SelectShortCourseTrainingVenuesController> _logger) : ControllerBase
{
    public const string ViewPath = "~/Views/AddAShortCourse/SelectShortCourseTrainingVenuesView.cshtml";

    [HttpGet]
    public async Task<IActionResult> SelectShortCourseTrainingVenue(CourseType courseType)
    {
        var sessionModel = _sessionService.Get<ShortCourseSessionModel>();

        if (sessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

        if (!sessionModel.LocationOptions.Contains(ShortCourseLocationOption.ProviderLocation))
        {
            _logger.LogWarning("User: {UserId} unexpectedly landed on select a training venue page when location options does not contain provider.", UserId);

            return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);
        }

        var trainingVenues = await GetTrainingVenues(sessionModel);

        sessionModel.LocationsAvailable = trainingVenues.Count != 0;

        _sessionService.Set(sessionModel);

        if (!sessionModel.LocationsAvailable)
        {
            return RedirectToRoute(RouteNames.GetAddTrainingVenue, new { ukprn = Ukprn, courseType });
        }

        var model = new SelectShortCourseTrainingVenuesViewModel()
        {
            TrainingVenues = sessionModel.TrainingVenues,
            CourseType = courseType
        };

        return View(ViewPath, model);
    }

    [HttpPost]
    public IActionResult SelectShortCourseTrainingVenue(SelectShortCourseTrainingVenuesSubmitModel submitModel, CourseType courseType)
    {
        var sessionModel = _sessionService.Get<ShortCourseSessionModel>();

        if (sessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

        foreach (var trainingVenue in sessionModel.TrainingVenues)
        {
            trainingVenue.IsSelected = false;
        }

        if (!ModelState.IsValid)
        {
            var model = new SelectShortCourseTrainingVenuesViewModel()
            {
                TrainingVenues = sessionModel.TrainingVenues,
                CourseType = courseType
            };

            return View(ViewPath, model);
        }

        foreach (var selectedProviderLocationId in submitModel.SelectedProviderLocationIds)
        {
            foreach (var trainingVenue in sessionModel.TrainingVenues)
            {
                if (selectedProviderLocationId == trainingVenue.ProviderLocationId)
                {
                    trainingVenue.IsSelected = true;
                }
            }
        }

        _sessionService.Set(sessionModel);

        return RedirectToRoute(RouteNames.SelectShortCourseTrainingVenue, new { ukprn = Ukprn, courseType });
    }

    private async Task<List<TrainingVenueModel>> GetTrainingVenues(ShortCourseSessionModel sessionModel)
    {
        _logger.LogInformation("Getting provider course locations for ukprn {Ukprn}", Ukprn);

        var result = await _mediator.Send(new GetAllProviderLocationsQuery(Ukprn));

        var trainingVenues = result.ProviderLocations.Select(p => (TrainingVenueModel)p).Where(p => p.LocationType == LocationType.Provider).OrderBy(l => l.LocationName).ToList();

        sessionModel.TrainingVenues.RemoveAll(s => !trainingVenues.Any(p => p.ProviderLocationId == s.ProviderLocationId));

        sessionModel.TrainingVenues.AddRange(trainingVenues.Where(p => !sessionModel.TrainingVenues.Any(t => t.ProviderLocationId == p.ProviderLocationId)));

        return trainingVenues;
    }
}
