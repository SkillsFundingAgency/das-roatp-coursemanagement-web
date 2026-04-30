using System.Linq;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Extensions;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderContact;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddProviderContact;

[Route("{ukprn}/contact-select-standards", Name = RouteNames.AddProviderContactSelectStandardsForUpdate)]
public class SelectStandardsForUpdateController(ISessionService _sessionService, IValidator<AddProviderContactStandardsSubmitViewModel> _validator) : ControllerBase
{
    public const string ViewPath = "~/Views/AddProviderContact/SelectStandards.cshtml";

    [HttpGet]
    public IActionResult SelectStandards(int ukprn)
    {
        var sessionModel = _sessionService.Get<ProviderContactSessionModel>();
        if (sessionModel == null) return RedirectToRoute(RouteNames.ReviewYourDetails, new { ukprn = Ukprn });

        var model = GetModel(sessionModel);

        return View(ViewPath, model);
    }

    [HttpPost]
    public IActionResult PostStandards(int ukprn, AddProviderContactStandardsSubmitViewModel submitModel)
    {

        var sessionModel = _sessionService.Get<ProviderContactSessionModel>();
        if (sessionModel == null) return RedirectToRoute(RouteNames.ReviewYourDetails, new { ukprn = Ukprn });

        foreach (var standard in sessionModel.Standards)
        {
            standard.IsSelected = submitModel.SelectedProviderCourseIds.Contains(standard.ProviderCourseId);
        }

        _sessionService.Set(sessionModel);

        var validatedResult = _validator.Validate(submitModel);

        if (!validatedResult.IsValid)
        {
            var viewModel = GetModel(sessionModel);

            ModelState.AddValidationErrors(validatedResult.Errors);

            return View(ViewPath, viewModel);
        }

        return RedirectToRoute(RouteNames.AddProviderContactCheckStandards, new { ukprn = Ukprn });
    }

    private static AddProviderContactStandardsViewModel GetModel(ProviderContactSessionModel sessionModel)
    {
        var standards = sessionModel.Standards.OrderBy(x => x.CourseName).ThenBy(x => x.Level).Select(x => new ProviderContactStandardModel
        {
            ProviderCourseId = x.ProviderCourseId,
            CourseName = $"{x.CourseName} (level {x.Level})",
            Level = x.Level,
            CourseType = x.CourseType,
            IsSelected = x.IsSelected,
        }).ToList();

        return new AddProviderContactStandardsViewModel
        {
            Standards = standards.Where(x => x.CourseType == CourseType.Apprenticeship).ToList(),
            ApprenticeshipUnits = standards.Where(x => x.CourseType == CourseType.ShortCourse).ToList(),
            ShowStandards = standards.Any(x => x.CourseType == CourseType.Apprenticeship),
            ShowApprenticeshipUnits = standards.Any(x => x.CourseType == CourseType.ShortCourse)
        };
    }
}
