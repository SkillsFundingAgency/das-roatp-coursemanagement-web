using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using System;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAShortCourse;

[Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
[Route("{ukprn}/courses/{courseType}/new/select-course-locations", Name = RouteNames.SelectShortCourseLocationOption)]
public class SelectShortCourseLocationOptionsController(ISessionService _sessionService) : ControllerBase
{
    public const string ViewPath = "~/Views/AddAShortCourse/SelectShortCourseLocationOptionsView.cshtml";

    [HttpGet]
    public IActionResult SelectShortCourseLocation(CourseType courseType)
    {
        var sessionModel = _sessionService.Get<ShortCourseSessionModel>();

        if (sessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

        List<ShortCourseLocationOptionModel> locationOptions = new List<ShortCourseLocationOptionModel>();

        foreach (var locationOption in Enum.GetValues<ShortCourseLocationOption>())
        {
            locationOptions.Add(new ShortCourseLocationOptionModel { LocationOption = locationOption, IsSelected = sessionModel.LocationOptions.Contains(locationOption) });
        }

        var model = new SelectShortCourseLocationOptionsViewModel()
        {
            LocationOptions = locationOptions,
            CourseType = courseType,
        };

        return View(ViewPath, model);
    }

    [HttpPost]
    public IActionResult SelectShortCourseLocation(SelectShortCourseLocationOptionsSubmitModel submitModel, CourseType courseType)
    {
        var sessionModel = _sessionService.Get<ShortCourseSessionModel>();

        if (sessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

        if (!ModelState.IsValid)
        {
            List<ShortCourseLocationOptionModel> locationOptions = new List<ShortCourseLocationOptionModel>();

            foreach (var locationOption in Enum.GetValues<ShortCourseLocationOption>())
            {
                locationOptions.Add(new ShortCourseLocationOptionModel { LocationOption = locationOption, IsSelected = false });
            }

            var model = new SelectShortCourseLocationOptionsViewModel()
            {
                LocationOptions = locationOptions,
                CourseType = courseType,
            };

            return View(ViewPath, model);
        }

        sessionModel.LocationOptions = submitModel.SelectedLocationOptions;

        sessionModel.HasOnlineDeliveryOption = submitModel.SelectedLocationOptions.Contains(ShortCourseLocationOption.Online);

        foreach (var trainingVenue in sessionModel.TrainingVenues)
        {
            trainingVenue.IsSelected = false;
        }

        _sessionService.Set(sessionModel);

        if (submitModel.SelectedLocationOptions.Contains(ShortCourseLocationOption.ProviderLocation))
        {
            return RedirectToRoute(RouteNames.SelectShortCourseTrainingVenue, new { ukprn = Ukprn, courseType });
        }

        return RedirectToRoute(RouteNames.SelectShortCourseLocationOption, new { ukprn = Ukprn, courseType });
    }
}
