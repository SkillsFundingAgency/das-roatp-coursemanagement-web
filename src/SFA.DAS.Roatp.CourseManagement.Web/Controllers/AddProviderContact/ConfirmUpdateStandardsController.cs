using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderContact;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddProviderContact;

[Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
[Route("{ukprn}/update-existing-standards", Name = RouteNames.AddProviderContactConfirmUpdateStandards)]

public class ConfirmUpdateStandardsController(ISessionService _sessionService) : ControllerBase
{
    public const string ViewPath = "~/Views/AddProviderContact/UpdateStandards.cshtml";

    [HttpGet]
    public IActionResult UpdateStandardsEmailAndPhone(int ukprn)
    {
        var providerContactModel = _sessionService.Get<ProviderContactSessionModel>();

        if (providerContactModel == null) return RedirectToRoute(RouteNames.ReviewYourDetails, new { ukprn = Ukprn });

        var model = new ProviderContactUpdateStandardsViewModel
        {
            EmailAddress = providerContactModel.EmailAddress,
            PhoneNumber = providerContactModel.PhoneNumber,
            HasOptedToUpdateExistingStandards = providerContactModel.UpdateExistingStandards
        };

        return View(ViewPath, model);
    }

    [HttpPost]
    public IActionResult PostUpdateStandardsEmailAndPhone(int ukprn, ProviderContactUpdateStandardsSubmitViewModel submitViewModel)
    {
        if (!ModelState.IsValid)
        {
            var model = new ProviderContactUpdateStandardsViewModel
            {
                EmailAddress = submitViewModel.EmailAddress,
                PhoneNumber = submitViewModel.PhoneNumber
            };

            return View(ViewPath, model);
        }

        var providerContactModel = _sessionService.Get<ProviderContactSessionModel>();
        providerContactModel.UpdateExistingStandards = submitViewModel.HasOptedToUpdateExistingStandards;
        _sessionService.Set(providerContactModel);

        if (submitViewModel.HasOptedToUpdateExistingStandards is true)
        {
            return RedirectToRoute(RouteNames.AddProviderContactSelectStandardsForUpdate, new { ukprn = Ukprn });
        }

        return RedirectToRoute(RouteNames.AddProviderContactConfirmUpdateStandards, new { ukprn = Ukprn });
    }
}
