using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAllProviderStandards;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderContact;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddProviderContact;

[Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
[Route("{ukprn}/add-provider-contact", Name = RouteNames.AddProviderContact)]
public class AddProviderContactController(IMediator _mediator, ISessionService _sessionService) : ControllerBase
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
    public async Task<IActionResult> PostProviderContact(int ukprn, AddProviderContactSubmitViewModel submitViewModel)
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


        GetAllProviderStandardsQueryResult standardsResult = await _mediator.Send(new GetAllProviderStandardsQuery(ukprn));
        if (standardsResult.Standards != null)
        {
            sessionModel.Standards = standardsResult.Standards.Select(x => (ProviderContactStandardModel)x).ToList();
        }

        _sessionService.Set(sessionModel);

        if (sessionModel.Standards is { Count: > 0 } && !string.IsNullOrEmpty(submitViewModel.EmailAddress) && !string.IsNullOrEmpty(submitViewModel.PhoneNumber))
        {
            return RedirectToRoute(RouteNames.ConfirmUpdateStandardsFromProviderContactEmailPhone, new { ukprn = Ukprn });
        }

        return RedirectToRoute(RouteNames.AddProviderContact, new { ukprn = Ukprn });
    }
}

