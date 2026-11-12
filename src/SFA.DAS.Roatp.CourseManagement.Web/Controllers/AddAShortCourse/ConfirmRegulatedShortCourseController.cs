using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Application.Courses.Queries.GetProviderAvailableCourseDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Extensions;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAShortCourse;

[AuthorizeCourseType(CourseType.ShortCourse)]
[Route("{ukprn}/courses/{learningType}/new/confirm-regulated-course", Name = RouteNames.ConfirmRegulatedShortCourse)]
public class ConfirmRegulatedShortCourseController(IMediator _mediator, ISessionService _sessionService, IValidator<ConfirmRegulatedShortCourseSubmitModel> _validator) : ControllerBase
{
    public const string ViewPath = "~/Views/ShortCourses/AddAShortCourse/ConfirmRegulatedShortCourse.cshtml";

    [HttpGet]
    public async Task<IActionResult> ConfirmRegulatedShortCourse(LearningType learningType)
    {
        var sessionModel = _sessionService.Get<ShortCourseSessionModel>();

        if (sessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

        var model = await GetViewModel(Ukprn, sessionModel.LarsCode, learningType);

        sessionModel.ShortCourseInformation = model.ShortCourseInformation;

        _sessionService.Set(sessionModel);

        return View(ViewPath, model);
    }

    [HttpPost]
    public IActionResult ConfirmRegulatedShortCourse(ConfirmRegulatedShortCourseSubmitModel submitModel, LearningType learningType)
    {
        var sessionModel = _sessionService.Get<ShortCourseSessionModel>();

        if (sessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

        var validatedResult = _validator.Validate(submitModel);

        if (!validatedResult.IsValid) ModelState.AddValidationErrors(validatedResult.Errors);

        if (!ModelState.IsValid)
        {
            var model = new ConfirmRegulatedShortCourseViewModel()
            {
                ShortCourseInformation = sessionModel.ShortCourseInformation,
                LearningType = learningType
            };

            return View(ViewPath, model);
        }

        if (submitModel.IsApprovedByRegulator == false)
        {
            return RedirectToRoute(RouteNames.ConfirmRegulatedShortCourse, new { ukprn = Ukprn, learningType });
        }

        if (sessionModel.SavedProviderContactModel == null || (sessionModel.SavedProviderContactModel.EmailAddress == null && sessionModel.SavedProviderContactModel.PhoneNumber == null))
        {
            return RedirectToRoute(RouteNames.AddShortCourseContactDetails, new { ukprn = Ukprn, learningType });
        }

        return RedirectToRoute(RouteNames.ConfirmSavedContactDetailsForShortCourse, new { ukprn = Ukprn, learningType });
    }

    private async Task<ConfirmRegulatedShortCourseViewModel> GetViewModel(int ukprn, string larsCode, LearningType learningType)
    {
        var shortCourseInfo = await _mediator.Send(new GetProviderAvailableCourseDetailsQuery(ukprn, CourseType.ShortCourse, larsCode));
        var model = new ConfirmRegulatedShortCourseViewModel()
        {
            ShortCourseInformation = shortCourseInfo,
            LearningType = learningType
        };
        return model;
    }
}
