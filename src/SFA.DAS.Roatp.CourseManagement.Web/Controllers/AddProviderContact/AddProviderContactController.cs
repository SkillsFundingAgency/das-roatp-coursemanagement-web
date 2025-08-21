using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderContact;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddProviderContact;

[Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
[Route("{ukprn}/add-provider-contact", Name = RouteNames.GetAddProviderContact)]
public class AddProviderContactController(ISessionService _sessionService) : ControllerBase
{
    public const string ViewPath = "~/Views/AddProviderContact/AddProviderContact.cshtml";


    [HttpGet]
    public IActionResult AddProviderContact(int ukprn)
    {
        var providerContactModel = _sessionService.Get<ProviderContactSessionModel>();

        var model = new AddProviderContactViewModel();

        if (providerContactModel != null)
        {
            model.EmailAddress = providerContactModel.EmailAddress;
            model.PhoneNumber = providerContactModel.PhoneNumber;
        }

        return View(ViewPath, model);
    }

    [HttpPost]
    public IActionResult PostProviderContact(int ukprn, AddProviderContactSubmitViewModel submitViewModel)
    {
        if (!ModelState.IsValid)
        {
            var model = new AddProviderContactViewModel
            {
                EmailAddress = submitViewModel.EmailAddress,
                PhoneNumber = submitViewModel.PhoneNumber
            };

            return View(ViewPath, model);
        }

        var sessionModel = new ProviderContactSessionModel { EmailAddress = submitViewModel.EmailAddress, PhoneNumber = submitViewModel.PhoneNumber };

        _sessionService.Set(sessionModel);
        return RedirectToRoute(RouteNames.GetAddProviderContact, new { ukprn = Ukprn });
    }
}

