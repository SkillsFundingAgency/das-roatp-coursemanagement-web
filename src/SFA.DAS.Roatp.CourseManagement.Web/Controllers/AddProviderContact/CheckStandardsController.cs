using System.Collections.Generic;
using System.Linq;
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
[Route("{ukprn}/contact-check-standards", Name = RouteNames.AddProviderContactCheckStandards)]
public class CheckStandardsController(IMediator _mediator, ISessionService _sessionService) : ControllerBase
{
    public const string ViewPath = "~/Views/AddProviderContact/CheckStandards.cshtml";

    [HttpGet]
    public IActionResult CheckStandards(int ukprn)
    {
        var sessionModel = _sessionService.Get<ProviderContactSessionModel>();
        if (sessionModel == null) return RedirectToRoute(RouteNames.ReviewYourDetails, new { ukprn = Ukprn });

        var checkedStandards = StandardDescriptionListService.BuildSelectedStandardsList(sessionModel.Standards);

        var model = new ProviderContactCheckStandardsViewModel
        {
            EmailAddress = sessionModel.EmailAddress,
            PhoneNumber = sessionModel.PhoneNumber,
            CheckedStandards = checkedStandards,
            ReviewYourDetailsUrl = Url.RouteUrl(RouteNames.ReviewYourDetails, new { ukprn }),
            ChangeEmailPhoneUrl = Url.RouteUrl(RouteNames.AddProviderContactDetails, new { ukprn }),
            ChangeSelectedStandardsUrl = Url.RouteUrl(RouteNames.AddProviderContactSelectStandardsForUpdate, new { ukprn }),
            UseBulletedList = checkedStandards.Count > 1
        };

        return View(ViewPath, model);
    }

    [HttpPost]
    public async Task<IActionResult> ConfirmCheckStandards(int ukprn)
    {

        var providerContactModel = _sessionService.Get<ProviderContactSessionModel>();
        if (providerContactModel == null) return RedirectToRoute(RouteNames.ReviewYourDetails, new { ukprn = Ukprn });

        var checkedProviderCourseIds = new List<int>();
        foreach (var standard in providerContactModel.Standards.Where(s => s.IsSelected))
        {
            checkedProviderCourseIds.Add(standard.ProviderCourseId);
        }

        var command = new AddProviderContactCommand
        {
            UserId = UserId,
            UserDisplayName = UserDisplayName,
            Ukprn = ukprn,
            EmailAddress = providerContactModel.EmailAddress,
            PhoneNumber = providerContactModel.PhoneNumber,
            ProviderCourseIds = checkedProviderCourseIds
        };

        await _mediator.Send(command);

        return RedirectToRouteWithUkprn(RouteNames.AddProviderContactSaved);
    }
}
