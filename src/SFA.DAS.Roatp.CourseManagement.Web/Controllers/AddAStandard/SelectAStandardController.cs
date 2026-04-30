using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderContact.Queries;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAvailableProviderStandards;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Queries.GetStandardInformation;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Extensions;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAStandard;

[AuthorizeCourseType(CourseType.Apprenticeship)]
[Route("{ukprn}/standards/add/select-standard")]
public class SelectAStandardController : ControllerBase
{
    public const string ViewPath = "~/Views/AddAStandard/SelectAStandard.cshtml";
    private readonly ILogger<SelectAStandardController> _logger;
    private readonly IMediator _mediator;
    private readonly ISessionService _sessionService;
    private readonly IValidator<SelectAStandardSubmitModel> _validator;

    public SelectAStandardController(ILogger<SelectAStandardController> logger, IMediator mediator, ISessionService sessionService, IValidator<SelectAStandardSubmitModel> validator)
    {
        _logger = logger;
        _mediator = mediator;
        _sessionService = sessionService;
        _validator = validator;
    }

    [HttpGet(Name = RouteNames.GetAddStandardSelectStandard)]
    [ClearSession(nameof(StandardSessionModel))]
    public async Task<IActionResult> SelectAStandard()
    {
        var model = await GetModel();
        return View(ViewPath, model);
    }

    [HttpPost(Name = RouteNames.PostAddStandardSelectStandard)]
    public async Task<IActionResult> SubmitAStandard(SelectAStandardSubmitModel submitModel)
    {
        var validatedResult = _validator.Validate(submitModel);

        if (!validatedResult.IsValid)
        {
            var model = await GetModel();

            ModelState.AddValidationErrors(validatedResult.Errors);

            return View(ViewPath, model);
        }
        _logger.LogInformation("Begin of journey for ukprn: {Ukprn} to add standard {LarsCode}", Ukprn, submitModel.SelectedLarsCode);

        var sessionModel = new StandardSessionModel { LarsCode = submitModel.SelectedLarsCode };


        var providerContactResponse = await _mediator.Send(new GetLatestProviderContactQuery(Ukprn));

        if (providerContactResponse != null)
        {
            sessionModel.LatestProviderContactModel = new ProviderContactModel
            {
                EmailAddress = providerContactResponse.EmailAddress,
                PhoneNumber = providerContactResponse.PhoneNumber
            };
        }

        _sessionService.Set(sessionModel);

        var standardInformation = await _mediator.Send(new GetStandardInformationQuery(submitModel.SelectedLarsCode));

        if (standardInformation.IsRegulatedForProvider)
        {
            return RedirectToRouteWithUkprn(RouteNames.GetAddStandardConfirmRegulatedStandard);
        }

        _logger.LogInformation("Add standard: A non-regulated standard larscode:{LarsCode} is being added for ukprn:{Ukprn} by {UserId}", sessionModel.LarsCode, Ukprn, UserId);

        return RedirectToRouteWithUkprn(RouteNames.GetAddStandardConfirmNonRegulatedStandard);
    }

    private async Task<SelectAStandardViewModel> GetModel()
    {
        var result = await _mediator.Send(new GetAvailableProviderStandardsQuery(Ukprn, CourseType.Apprenticeship));
        var model = new SelectAStandardViewModel();
        model.Standards = result.AvailableCourses.OrderBy(c => c.Title).Select(s => new SelectListItem($"{s.Title} (Level {s.Level})", s.LarsCode.ToString()));
        return model;
    }
}