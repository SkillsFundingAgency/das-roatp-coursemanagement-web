using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderContact;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddProviderContact;

[Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
[Route("{ukprn}/update-existing-standards", Name = RouteNames.ConfirmUpdateStandardsFromProviderContactEmailPhone)]

public class ProviderContactUpdateStandardsController(ISessionService _sessionService) : ControllerBase
{
    public const string ViewPath = "~/Views/AddProviderContact/UpdateStandardsWithEmailAndPhone.cshtml";

    [HttpGet]
    public IActionResult UpdateStandardsEmailAndPhone(int ukprn)
    {
        var providerContactModel = _sessionService.Get<ProviderContactSessionModel>();

        if (providerContactModel == null) return RedirectToRoute(RouteNames.AddProviderContact, new { ukprn = Ukprn });

        var model = new ProviderContactUpdateStandardsViewModel
        {
            EmailAddress = providerContactModel.EmailAddress,
            PhoneNumber = providerContactModel.PhoneNumber,
            UpdateExistingStandards = providerContactModel.UpdateExistingStandards
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
        providerContactModel.UpdateExistingStandards = submitViewModel.UpdateExistingStandards;
        _sessionService.Set(providerContactModel);

        return RedirectToRoute(RouteNames.ConfirmUpdateStandardsFromProviderContactEmailPhone, new { ukprn = Ukprn });
    }
}
