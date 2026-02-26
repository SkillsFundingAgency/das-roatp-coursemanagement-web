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
    public const string ConfirmButtonText = "Confirm";
    public const string ContinueButtonText = "Continue";

    [HttpGet]
    public IActionResult SelectShortCourseLocation(ApprenticeshipType apprenticeshipType)
    {
        var sessionModel = _sessionService.Get<ShortCourseSessionModel>();

        if (sessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

        var model = GetModel(sessionModel, apprenticeshipType, true);

        return View(ViewPath, model);
    }

    [HttpPost]
    public IActionResult SelectShortCourseLocation(SelectShortCourseLocationOptionsSubmitModel submitModel, ApprenticeshipType apprenticeshipType)
    {
        var sessionModel = _sessionService.Get<ShortCourseSessionModel>();

        if (sessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

        if (!ModelState.IsValid)
        {
            var model = GetModel(sessionModel, apprenticeshipType, false);

            return View(ViewPath, model);
        }

        if (!sessionModel.HasSeenSummaryPage && !sessionModel.LocationOptions.OrderBy(x => x).SequenceEqual(submitModel.SelectedLocationOptions.OrderBy(x => x)))
        {
            sessionModel.ResetModel();
        }

        var removedOptions = sessionModel.LocationOptions.Except(submitModel.SelectedLocationOptions).ToList();

        if (sessionModel.HasSeenSummaryPage && removedOptions.Count != 0)
        {
            RemoveOptions(removedOptions, sessionModel);
        }

        sessionModel.LocationOptions = submitModel.SelectedLocationOptions;

        sessionModel.HasOnlineDeliveryOption = submitModel.SelectedLocationOptions.Contains(ShortCourseLocationOption.Online);

        _sessionService.Set(sessionModel);

        if (submitModel.SelectedLocationOptions.Contains(ShortCourseLocationOption.ProviderLocation) && sessionModel.IsProviderInfoMissing())
        {
            return RedirectToRoute(RouteNames.SelectShortCourseTrainingVenue, new { ukprn = Ukprn, apprenticeshipType });
        }

        if (submitModel.SelectedLocationOptions.Contains(ShortCourseLocationOption.EmployerLocation) && sessionModel.IsEmployerInfoMissing())
        {
            return RedirectToRoute(RouteNames.ConfirmNationalDelivery, new { ukprn = Ukprn, apprenticeshipType });
        }

        if (sessionModel.HasNationalDeliveryOption == false && sessionModel.IsEmployerRegionsMissing())
        {
            return RedirectToRoute(RouteNames.SelectShortCourseRegions, new { ukprn = Ukprn, apprenticeshipType });
        }

        return RedirectToRoute(RouteNames.ReviewShortCourseDetails, new { ukprn = Ukprn, apprenticeshipType });
    }

    private static SelectShortCourseLocationOptionsViewModel GetModel(ShortCourseSessionModel sessionModel, ApprenticeshipType apprenticeshipType, bool setIsSelected)
    {
        List<ShortCourseLocationOptionModel> locationOptions = new List<ShortCourseLocationOptionModel>();

        foreach (var locationOption in Enum.GetValues<ShortCourseLocationOption>())
        {
            if (setIsSelected)
            {
                locationOptions.Add(new ShortCourseLocationOptionModel { LocationOption = locationOption, IsSelected = sessionModel.LocationOptions.Contains(locationOption) });
            }
            else
            {
                locationOptions.Add(new ShortCourseLocationOptionModel { LocationOption = locationOption, IsSelected = false });
            }
        }

        return new SelectShortCourseLocationOptionsViewModel()
        {
            LocationOptions = locationOptions,
            ApprenticeshipType = apprenticeshipType,
            SubmitButtonText = sessionModel.HasSeenSummaryPage ? ConfirmButtonText : ContinueButtonText
        };
    }

    private static void RemoveOptions(List<ShortCourseLocationOption> options, ShortCourseSessionModel sessionModel)
    {
        if (options.Contains(ShortCourseLocationOption.ProviderLocation))
        {
            sessionModel.ResetProviderOptionModel();
        }
        if (options.Contains(ShortCourseLocationOption.EmployerLocation))
        {
            sessionModel.ResetEmployerOptionModel();
        }
        if (options.Contains(ShortCourseLocationOption.Online))
        {
            sessionModel.ResetOnlineOptionModel();
        }
    }
}
