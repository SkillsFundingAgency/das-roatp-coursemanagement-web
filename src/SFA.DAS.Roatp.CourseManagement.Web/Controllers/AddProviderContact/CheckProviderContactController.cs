using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderContact.Queries;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderContact;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddProviderContact;

[Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
[Route("{ukprn}/check-provider-contact", Name = RouteNames.CheckProviderContactDetails)]
public class CheckProviderContactController(IMediator _mediator, ISessionService _sessionService) : ControllerBase
{
    public const string ViewPath = "~/Views/AddProviderContact/CheckProviderContact.cshtml";
    [HttpGet]
    public async Task<IActionResult> CheckProviderContact(int ukprn)
    {
        var response = await _mediator.Send(new GetLatestProviderContactQuery(ukprn));

        if (response == null)
        {
            return RedirectToRoute(RouteNames.AddProviderContactDetails, new { ukprn = Ukprn });
        }

        var sessionModel = new ProviderContactSessionModel
        {
            EmailAddress = response.EmailAddress,
            PhoneNumber = response.PhoneNumber
        };

        _sessionService.Set(sessionModel);

        var model = new CheckProviderContactViewModel
        {
            EmailAddress = sessionModel.EmailAddress,
            PhoneNumber = sessionModel.PhoneNumber,
            ChangeProviderContactDetailsLink = Url.RouteUrl(RouteNames.AddProviderContactDetails, new { ukprn })
        };

        return View(ViewPath, model);
    }
}
