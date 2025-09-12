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
    public const string ViewPathEmailAndPhone = "~/Views/AddProviderContact/UpdateStandardsPhoneAndEmail.cshtml";
    public const string ViewPathEmail = "~/Views/AddProviderContact/UpdateStandardsEmailOnly.cshtml";
    public const string ViewPathPhone = "~/Views/AddProviderContact/UpdateStandardsPhoneOnly.cshtml";


    [HttpGet]
    public IActionResult ConfirmUpdateStandards(int ukprn)
    {
        var sessionModel = _sessionService.Get<ProviderContactSessionModel>();

        if (sessionModel == null) return RedirectToRoute(RouteNames.ReviewYourDetails, new { ukprn = Ukprn });

        var model = new ConfirmUpdateStandardsViewModel
        {
            EmailAddress = sessionModel.EmailAddress,
            PhoneNumber = sessionModel.PhoneNumber,
            HasOptedToUpdateExistingStandards = sessionModel.UpdateExistingStandards
        };

        var viewPath = GetViewPathFromEmailAndPhone(model);

        return View(viewPath, model);
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
            var viewPath = GetViewPathFromEmailAndPhone(model);
            return View(viewPath, model);
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

    private static string GetViewPathFromEmailAndPhone(ConfirmUpdateStandardsViewModel model)
    {
        var viewPath = ViewPathEmailAndPhone;

        if (!string.IsNullOrEmpty(model.EmailAddress) && !string.IsNullOrEmpty(model.PhoneNumber))
        {
            return ViewPathEmailAndPhone;
        }

        if (!string.IsNullOrEmpty(model.EmailAddress))
        {
            viewPath = ViewPathEmail;
        }

        return !string.IsNullOrEmpty(model.PhoneNumber) ? ViewPathPhone : viewPath;
    }
}
