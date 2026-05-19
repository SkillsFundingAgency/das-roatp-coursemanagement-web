using System.Collections.Generic;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Common.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Extensions;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAShortCourse;

[AuthorizeCourseType(CourseType.ShortCourse)]
[Route("{ukprn}/courses/{learningType}/new/confirm-national", Name = RouteNames.ConfirmNationalDelivery)]
public class ConfirmNationalDeliveryController(ISessionService _sessionService, ILogger<ConfirmNationalDeliveryController> _logger, IValidator<ConfirmNationalDeliverySubmitModel> _validator) : ControllerBase
{
    public const string ViewPath = "~/Views/ShortCourses/ConfirmNationalDelivery.cshtml";

    [HttpGet]
    public IActionResult ConfirmNationalProviderDelivery(LearningType learningType)
    {
        var sessionModel = _sessionService.Get<ShortCourseSessionModel>();

        if (sessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

        if (!sessionModel.LocationOptions.Contains(ShortCourseLocationOption.EmployerLocation))
        {
            _logger.LogWarning("User: {UserId} unexpectedly landed on national delivery confirmation when location options does not contain employer.", UserId);

            return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);
        }

        var model = new ConfirmNationalDeliveryViewModel()
        {
            LearningType = learningType,
            HasNationalDeliveryOption = sessionModel.HasNationalDeliveryOption,
            SubmitButtonText = sessionModel.HasSeenSummaryPage ? ButtonText.Confirm : ButtonText.Continue,
            IsAddJourney = true,
            Route = RouteNames.ConfirmNationalDelivery
        };

        return View(ViewPath, model);
    }

    [HttpPost]
    public IActionResult ConfirmNationalProviderDelivery(ConfirmNationalDeliverySubmitModel submitModel, LearningType learningType)
    {
        var sessionModel = _sessionService.Get<ShortCourseSessionModel>();

        if (sessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

        var validatedResult = _validator.Validate(submitModel);

        if (!validatedResult.IsValid) ModelState.AddValidationErrors(validatedResult.Errors);

        if (!ModelState.IsValid)
        {
            var vieModel = new ConfirmNationalDeliveryViewModel()
            {
                LearningType = learningType,
                SubmitButtonText = sessionModel.HasSeenSummaryPage ? ButtonText.Confirm : ButtonText.Continue,
                IsAddJourney = true,
                Route = RouteNames.ConfirmNationalDelivery
            };

            return View(ViewPath, vieModel);
        }

        sessionModel.HasNationalDeliveryOption = submitModel.HasNationalDeliveryOption;

        if (submitModel.HasNationalDeliveryOption == true)
        {
            sessionModel.TrainingRegions = new List<TrainingRegionModel>();
        }
        _sessionService.Set(sessionModel);

        if (submitModel.HasNationalDeliveryOption == false && sessionModel.IsEmployerRegionsMissing())
        {
            return RedirectToRoute(RouteNames.SelectShortCourseRegions, new { ukprn = Ukprn, learningType });
        }

        return RedirectToRoute(RouteNames.ReviewShortCourseDetails, new { ukprn = Ukprn, learningType });
    }
}
