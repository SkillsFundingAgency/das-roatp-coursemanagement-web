using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.AddProviderContact;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderContact;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddProviderContact;

[Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
[Route("{ukprn}/update-contact-details", Name = RouteNames.AddProviderContact)]

public class ConfirmUpdateProviderContactController(IMediator _mediator, ISessionService _sessionService) : ControllerBase
{
    public const string ViewPath = "~/Views/AddProviderContact/UpdateProviderContact.cshtml";


    [HttpGet]
    public IActionResult CheckContact(int ukprn)
    {
        var sessionModel = _sessionService.Get<ProviderContactSessionModel>();
        if (sessionModel == null) return RedirectToRoute(RouteNames.ReviewYourDetails, new { ukprn = Ukprn });

        var model = new ProviderContactUpdateViewModel
        {
            EmailAddress = sessionModel.EmailAddress,
            PhoneNumber = sessionModel.PhoneNumber,
            ReviewYourDetailsUrl = Url.RouteUrl(RouteNames.ReviewYourDetails, new { ukprn }),
            ChangeEmailPhoneUrl = Url.RouteUrl(RouteNames.AddProviderContactDetails, new { ukprn }),
            ShowEmail = !string.IsNullOrEmpty(sessionModel.EmailAddress),
            ShowPhone = !string.IsNullOrEmpty(sessionModel.PhoneNumber)
        };

        return View(ViewPath, model);
    }

    [HttpPost]
    public async Task<IActionResult> ConfirmCheckContact(int ukprn)
    {

        var sessionModel = _sessionService.Get<ProviderContactSessionModel>();
        if (sessionModel == null) return RedirectToRoute(RouteNames.ReviewYourDetails, new { ukprn = Ukprn });

        var command = new AddProviderContactCommand
        {
            UserId = UserId,
            UserDisplayName = UserDisplayName,
            Ukprn = ukprn,
            EmailAddress = sessionModel.EmailAddress,
            PhoneNumber = sessionModel.PhoneNumber,
            ProviderCourseIds = new List<int>()
        };

        await _mediator.Send(command);

        return RedirectToRouteWithUkprn(RouteNames.AddProviderContactSaved);
    }
}
