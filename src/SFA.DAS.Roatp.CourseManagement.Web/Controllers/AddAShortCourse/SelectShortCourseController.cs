using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderContact.Queries;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAvailableProviderStandards;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Common;
using SFA.DAS.Roatp.CourseManagement.Web.Extensions;
using SFA.DAS.Roatp.CourseManagement.Web.Factories;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAShortCourse;

[AuthorizeCourseType(CourseType.ShortCourse)]
[Route("{ukprn}/courses/{learningType}/new/select-course", Name = RouteNames.SelectShortCourse)]
public class SelectShortCourseController(IMediator _mediator, ISessionService _sessionService, IValidator<SelectShortCourseSubmitModel> _validator, IBreadcrumbsViewModelFactory _breadcrumbsFactory) : ControllerBase
{
    public const string ViewPath = "~/Views/ShortCourses/AddAShortCourse/SelectShortCourse.cshtml";
    public const string ShortCoursesNotAvailableViewPath = "~/Views/ShutterPages/ShortCoursesNotAvailable.cshtml";

    [HttpGet]
    [ClearSession(nameof(ShortCourseSessionModel))]
    public async Task<IActionResult> SelectShortCourse(LearningType learningType)
    {
        var viewModel = await GetModel(learningType);

        if (viewModel == null)
        {
            return RedirectToRoute(RouteNames.ShortCoursesNotAvailable, new { ukprn = Ukprn, learningType });
        }

        return View(ViewPath, viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> SelectShortCourse(SelectShortCourseSubmitModel submitModel, LearningType learningType)
    {
        var validatedResult = _validator.Validate(submitModel);

        if (!validatedResult.IsValid) ModelState.AddValidationErrors(validatedResult.Errors);

        if (!ModelState.IsValid)
        {
            var viewModel = await GetModel(learningType);

            return View(ViewPath, viewModel);

        }

        var sessionModel = new ShortCourseSessionModel();

        sessionModel.LarsCode = submitModel.SelectedLarsCode;

        var providerContactResponse = await _mediator.Send(new GetLatestProviderContactQuery(Ukprn));

        if (providerContactResponse != null)
        {
            sessionModel.SavedProviderContactModel = new ProviderContactModel
            {
                EmailAddress = providerContactResponse.EmailAddress,
                PhoneNumber = providerContactResponse.PhoneNumber
            };
        }

        _sessionService.Set(sessionModel);

        return RedirectToRoute(RouteNames.ConfirmShortCourse, new { ukprn = Ukprn, learningType });
    }

    private async Task<SelectShortCourseViewModel> GetModel(LearningType learningType)
    {
        var result = await _mediator.Send(new GetAvailableProviderStandardsQuery(Ukprn, CourseType.ShortCourse));

        if (result.AvailableCourses.Count == 0)
        {
            return null;
        }

        var model = new SelectShortCourseViewModel()
        {
            ShortCourses = result.AvailableCourses.OrderBy(c => c.Title).Select(s => new SelectListItem($"{s.Title} (Level {s.Level})", s.LarsCode.ToString())),
            LearningType = learningType
        };

        return model;
    }

    [HttpGet("courses-not-available", Name = RouteNames.ShortCoursesNotAvailable)]
    public IActionResult CoursesNotAvailable(LearningType learningType)
    {
        var breadcrumbModel = new BreadcrumbsViewModelFactoryModel
        {
            Ukprn = Ukprn,
            BreadcrumbNames =
            [
                BreadcrumbNames.ProviderDashboard,
                BreadcrumbNames.ManageYourCourses
            ]
        };

        var viewModel = new ShortCourseShutterPagesViewModel
        {
            LearningType = learningType,
            Breadcrumbs = _breadcrumbsFactory.Create(Url, breadcrumbModel)
        };

        return View(ShortCoursesNotAvailableViewPath, viewModel);
    }
}