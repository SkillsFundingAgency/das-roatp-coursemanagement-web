using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Queries.GetStandardInformation;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Extensions;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAStandard;

[AuthorizeCourseType(CourseType.Apprenticeship)]
[Route("{ukprn}/standards")]
public class ConfirmRegulatedStandardController : AddAStandardControllerBase
{
    public const string ViewPath = "~/Views/AddAStandard/ConfirmRegulatedStandard.cshtml";

    private readonly IMediator _mediator;
    private readonly ILogger<ConfirmRegulatedStandardController> _logger;
    private readonly IValidator<ConfirmNewRegulatedStandardSubmitModel> _validator;

    public ConfirmRegulatedStandardController(ISessionService sessionService, IMediator mediator, ILogger<ConfirmRegulatedStandardController> logger, IValidator<ConfirmNewRegulatedStandardSubmitModel> validator) : base(sessionService)
    {
        _mediator = mediator;
        _logger = logger;
        _validator = validator;
    }

    [HttpGet("add/confirm-regulated-standard", Name = RouteNames.GetAddStandardConfirmRegulatedStandard)]
    public async Task<IActionResult> GetConfirmationOfRegulatedStandard()
    {
        var (sessionModel, redirectResult) = GetSessionModelWithEscapeRoute(_logger);
        if (sessionModel == null) return redirectResult;

        var model = await GetViewModel(sessionModel.LarsCode);
        return View(ViewPath, model);
    }

    [HttpPost("add/confirm-regulated-standard", Name = RouteNames.PostAddStandardConfirmRegulatedStandard)]
    public async Task<IActionResult> SubmitConfirmationOfRegulatedStandard(ConfirmNewRegulatedStandardSubmitModel submitModel)
    {
        var (sessionModel, redirectResult) = GetSessionModelWithEscapeRoute(_logger);
        if (sessionModel == null) return redirectResult;

        var validatedResult = _validator.Validate(submitModel);

        if (!validatedResult.IsValid)
        {
            var model = await GetViewModel(sessionModel.LarsCode);

            ModelState.AddValidationErrors(validatedResult.Errors);

            return View(ViewPath, model);
        }

        if (submitModel.IsApprovedByRegulator == false)
        {
            return RedirectToRouteWithUkprn(RouteNames.GetNeedApprovalToDeliverRegulatedStandard);
        }

        sessionModel.IsConfirmed = true;
        sessionModel.StandardInformation = await _mediator.Send(new GetStandardInformationQuery(sessionModel.LarsCode));
        _sessionService.Set(sessionModel);

        if (sessionModel.LatestProviderContactModel != null)
        {
            return RedirectToRouteWithUkprn(RouteNames.GetAddStandardUseSavedContactDetails);
        }

        _logger.LogInformation("Add standard: Regulated standard confirmed for ukprn:{Ukprn} larscode:{LarsCode}", Ukprn, sessionModel.LarsCode);

        return RedirectToRouteWithUkprn(RouteNames.GetAddStandardAddContactDetails);
    }

    [HttpGet("needs-approval", Name = RouteNames.GetNeedApprovalToDeliverRegulatedStandard)]
    public ActionResult NeedConfirmationOfRegulatedStandard()
    {
        var model = new NeedApprovalForRegulatedStandardViewModel
        {
            SelectAStandardLink = GetUrlWithUkprn(RouteNames.GetAddStandardSelectStandard)
        };

        return View("~/Views/AddAStandard/NeedApprovalForRegulatedStandard.cshtml", model);
    }

    private async Task<ConfirmNewRegulatedStandardViewModel> GetViewModel(string larsCode)
    {
        var standardInfo = await _mediator.Send(new GetStandardInformationQuery(larsCode));
        var model = new ConfirmNewRegulatedStandardViewModel()
        {
            StandardInformation = standardInfo,
            ContinueLink = GetUrlWithUkprn(RouteNames.ViewStandards)
        };
        return model;
    }
}
