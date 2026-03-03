using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAllProviderStandards;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderContact;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        var sessionModel = _sessionService.Get<ProviderContactSessionModel>();
        if (sessionModel == null) sessionModel = new ProviderContactSessionModel();

        sessionModel.EmailAddress = submitViewModel.EmailAddress;
        sessionModel.PhoneNumber = submitViewModel.PhoneNumber;

        if (sessionModel.Standards == null)
        {
            sessionModel.Standards = await GetStandards(ukprn, CourseType.Apprenticeship);

            sessionModel.HasStandards = sessionModel.Standards.Any();
        }

        if (sessionModel.ShortCourses == null)
        {
            sessionModel.ShortCourses = await GetStandards(ukprn, CourseType.ShortCourse);

            sessionModel.HasShortCourses = sessionModel.ShortCourses.Any();
        }

        _sessionService.Set(sessionModel);

        if (sessionModel.HasStandards || sessionModel.HasShortCourses)
        {
            return RedirectToRoute(RouteNames.AddProviderContactConfirmUpdateStandards, new { ukprn = Ukprn });
        }

        return RedirectToRoute(RouteNames.AddProviderContact, new { ukprn = Ukprn });
    }

    private async Task<List<ProviderContactStandardModel>> GetStandards(int ukprn, CourseType courseType)
    {
        var result = await _mediator.Send(new GetAllProviderStandardsQuery(ukprn, courseType));

        return result.Standards.Select(x => (ProviderContactStandardModel)x).ToList();
    }
}

