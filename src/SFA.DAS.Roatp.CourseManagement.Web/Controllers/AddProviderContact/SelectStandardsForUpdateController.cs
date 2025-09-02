using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderContact;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddProviderContact;

[Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
[Route("{ukprn}/contact-select-standards", Name = RouteNames.AddProviderContactSelectStandardsForUpdate)]
public class SelectStandardsForUpdateController(ISessionService _sessionService) : ControllerBase
{
    public const string ViewPath = "~/Views/AddProviderContact/SelectStandards.cshtml";

    [HttpGet]
    public IActionResult SelectStandards(int ukprn)
    {
        var sessionModel = _sessionService.Get<ProviderContactSessionModel>();
        if (sessionModel == null) return RedirectToRoute(RouteNames.ReviewYourDetails, new { ukprn = Ukprn });

        var model = new AddProviderContactStandardsViewModel
        {
            Standards = sessionModel.Standards
        };

        return View(ViewPath, model);
    }

    [HttpPost]
    public IActionResult PostStandards(int ukprn, AddProviderContactStandardsSubmitViewModel submitModel)
    {

        var sessionModel = _sessionService.Get<ProviderContactSessionModel>();
        if (sessionModel == null) return RedirectToRoute(RouteNames.ReviewYourDetails, new { ukprn = Ukprn });

        foreach (var standard in sessionModel.Standards)
        {
            standard.IsSelected = false;
        }

        foreach (var submittedProviderCourseId in submitModel.SelectedProviderCourseIds)
        {
            foreach (var standard in sessionModel.Standards)
            {
                if (submittedProviderCourseId == standard.ProviderCourseId)
                {
                    standard.IsSelected = true;
                }
            }
        }

        _sessionService.Set(sessionModel);

        if (!ModelState.IsValid)
        {
            var viewModel = new AddProviderContactStandardsViewModel
            {
                Standards = sessionModel.Standards
            };

            return View(ViewPath, viewModel);
        }


        var model = new AddProviderContactStandardsViewModel
        {
            Standards = sessionModel.Standards
        };

        return View(ViewPath, model);
    }
}
