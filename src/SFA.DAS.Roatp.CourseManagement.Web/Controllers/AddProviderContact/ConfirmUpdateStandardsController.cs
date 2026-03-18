using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderContact;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddProviderContact;

[Route("{ukprn}/update-existing-standards", Name = RouteNames.AddProviderContactConfirmUpdateStandards)]

public class ConfirmUpdateStandardsController(ISessionService _sessionService) : ControllerBase
{
    public const string ViewPath = "~/Views/AddProviderContact/UpdateStandardsContactDetails.cshtml";

    [HttpGet]
    public IActionResult ConfirmUpdateStandards(int ukprn)
    {
        var sessionModel = _sessionService.Get<ProviderContactSessionModel>();

        if (sessionModel == null) return RedirectToRoute(RouteNames.ReviewYourDetails, new { ukprn = Ukprn });

        var model = new ConfirmUpdateStandardsViewModel
        {
            EmailAddress = sessionModel.EmailAddress,
            PhoneNumber = sessionModel.PhoneNumber,
            HasOptedToUpdateExistingStandards = sessionModel.UpdateExistingStandards,
        };

        model.EmailAddressOnlyUpdate = IsEmailAddressOnlyUpdate(model);
        model.PhoneNumberOnlyUpdate = IsPhoneNumberOnlyUpdate(model);
        model.EmailAddressAndPhoneNumberUpdate = IsEmailAndPhoneNumberUpdate(model);

        return View(ViewPath, model);
    }

    [HttpPost]
    public IActionResult PostConfirmUpdateStandards(int ukprn, ConfirmUpdateStandardsSubmitViewModel submitViewModel)
    {
        if (!ModelState.IsValid)
        {
            var model = new ConfirmUpdateStandardsViewModel
            {
                EmailAddress = submitViewModel.EmailAddress,
                PhoneNumber = submitViewModel.PhoneNumber
            };

            model.EmailAddressOnlyUpdate = IsEmailAddressOnlyUpdate(model);
            model.PhoneNumberOnlyUpdate = IsPhoneNumberOnlyUpdate(model);
            model.EmailAddressAndPhoneNumberUpdate = IsEmailAndPhoneNumberUpdate(model);

            return View(ViewPath, model);
        }

        var providerContactModel = _sessionService.Get<ProviderContactSessionModel>();
        providerContactModel.UpdateExistingStandards = submitViewModel.HasOptedToUpdateExistingStandards;
        _sessionService.Set(providerContactModel);

        if (submitViewModel.HasOptedToUpdateExistingStandards is true)
        {
            return RedirectToRoute(RouteNames.AddProviderContactSelectStandardsForUpdate, new { ukprn = Ukprn });
        }

        return RedirectToRoute(RouteNames.AddProviderContact, new { ukprn = Ukprn });
    }

    private static bool IsEmailAddressOnlyUpdate(ConfirmUpdateStandardsViewModel model)
    {
        return !string.IsNullOrEmpty(model.EmailAddress) && string.IsNullOrEmpty(model.PhoneNumber);
    }

    private static bool IsPhoneNumberOnlyUpdate(ConfirmUpdateStandardsViewModel model)
    {
        return string.IsNullOrEmpty(model.EmailAddress) && !string.IsNullOrEmpty(model.PhoneNumber);
    }

    private static bool IsEmailAndPhoneNumberUpdate(ConfirmUpdateStandardsViewModel model)
    {
        return !string.IsNullOrEmpty(model.EmailAddress) && !string.IsNullOrEmpty(model.PhoneNumber);
    }
}
