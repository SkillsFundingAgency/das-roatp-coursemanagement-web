using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAShortCourse;

[AuthorizeCourseType(CourseType.ShortCourse)]
[Route("{ukprn}/courses/{apprenticeshipType}/new/select-course-locations", Name = RouteNames.SelectShortCourseLocationOption)]
public class SelectShortCourseLocationOptionsController(ISessionService _sessionService) : ControllerBase
{
    public const string ViewPath = "~/Views/AddAShortCourse/SelectShortCourseLocationOptionsView.cshtml";

    [HttpGet]
    public IActionResult SelectShortCourseLocation(ApprenticeshipType apprenticeshipType)
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
            ApprenticeshipType = apprenticeshipType,
        };

        return View(ViewPath, model);
    }

    [HttpPost]
    public IActionResult SelectShortCourseLocation(SelectShortCourseLocationOptionsSubmitModel submitModel, ApprenticeshipType apprenticeshipType)
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
                ApprenticeshipType = apprenticeshipType,
            };

            return View(ViewPath, model);
        }

        if (!sessionModel.LocationOptions.OrderBy(x => x).SequenceEqual(submitModel.SelectedLocationOptions.OrderBy(x => x)))
        {
            sessionModel.ResetModel();
        }

        sessionModel.LocationOptions = submitModel.SelectedLocationOptions;

        sessionModel.HasOnlineDeliveryOption = submitModel.SelectedLocationOptions.Contains(ShortCourseLocationOption.Online);

        _sessionService.Set(sessionModel);

        if (submitModel.SelectedLocationOptions.Contains(ShortCourseLocationOption.ProviderLocation))
        {
            return RedirectToRoute(RouteNames.SelectShortCourseTrainingVenue, new { ukprn = Ukprn, apprenticeshipType });
        }

        if (submitModel.SelectedLocationOptions.Contains(ShortCourseLocationOption.EmployerLocation))
        {

            return RedirectToRoute(RouteNames.ConfirmNationalDelivery, new { ukprn = Ukprn, apprenticeshipType });
        }

        return RedirectToRoute(RouteNames.ReviewShortCourseDetails, new { ukprn = Ukprn, apprenticeshipType });
    }
}
