using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAllProviderStandards;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderContact;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddProviderContact;

[Route("{ukprn}/add-provider-contact", Name = RouteNames.AddProviderContactDetails)]
public class ProviderContactController(IMediator _mediator, ISessionService _sessionService) : ControllerBase
{
    public const string ViewPath = "~/Views/AddProviderContact/AddProviderContact.cshtml";

    [HttpGet]
    public IActionResult AddProviderContact(int ukprn)
    {
        var sessionModel = _sessionService.Get<ProviderContactSessionModel>();

        var model = new AddProviderContactViewModel();

        if (sessionModel != null)
        {
            model.EmailAddress = sessionModel.EmailAddress;
            model.PhoneNumber = sessionModel.PhoneNumber;
            model.ExistingContactDetailsAvailable = sessionModel.HasExistingContactDetails;
        }

        return View(ViewPath, model);
    }

    [HttpPost]
    public async Task<IActionResult> PostProviderContact(int ukprn, AddProviderContactSubmitViewModel submitViewModel)
    {
        var sessionModel = _sessionService.Get<ProviderContactSessionModel>();

        if (!ModelState.IsValid)
        {
            var model = new AddProviderContactViewModel
            {
                EmailAddress = submitViewModel.EmailAddress,
                PhoneNumber = submitViewModel.PhoneNumber,
                ExistingContactDetailsAvailable = sessionModel.HasExistingContactDetails
            };

            return View(ViewPath, model);
        }

        if (sessionModel == null) sessionModel = new ProviderContactSessionModel();

        sessionModel.EmailAddress = submitViewModel.EmailAddress;
        sessionModel.PhoneNumber = submitViewModel.PhoneNumber;

        if (sessionModel.Standards == null)
        {
            sessionModel.Standards = await GetStandards(ukprn);

            sessionModel.HasStandards = sessionModel.Standards.Count != 0;
        }

        _sessionService.Set(sessionModel);

        if (sessionModel.HasStandards)
        {
            return RedirectToRoute(RouteNames.AddProviderContactConfirmUpdateStandards, new { ukprn = Ukprn });
        }

        return RedirectToRoute(RouteNames.AddProviderContact, new { ukprn = Ukprn });
    }

    private async Task<List<ProviderContactStandardModel>> GetStandards(int ukprn)
    {
        var result = await _mediator.Send(new GetAllProviderStandardsQuery(ukprn, null));

        return result.Standards.Select(x => (ProviderContactStandardModel)x).ToList();
    }
}

