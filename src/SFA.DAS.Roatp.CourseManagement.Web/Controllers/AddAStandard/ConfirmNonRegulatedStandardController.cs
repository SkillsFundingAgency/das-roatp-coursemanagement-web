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
[Route("{ukprn}/standards/add/confirm-standard")]
public class ConfirmNonRegulatedStandardController : AddAStandardControllerBase
{
    public const string ViewPath = "~/Views/AddAStandard/ConfirmNonRegulatedStandard.cshtml";

    private readonly IMediator _mediator;
    private readonly ILogger<ConfirmNonRegulatedStandardController> _logger;
    private readonly IValidator<ConfirmNonRegulatedStandardSubmitModel> _validator;

    public ConfirmNonRegulatedStandardController(ISessionService sessionService, IMediator mediator, ILogger<ConfirmNonRegulatedStandardController> logger, IValidator<ConfirmNonRegulatedStandardSubmitModel> validator) : base(sessionService)
    {
        _mediator = mediator;
        _logger = logger;
        _validator = validator;
    }

    [HttpGet(Name = RouteNames.GetAddStandardConfirmNonRegulatedStandard)]
    public async Task<IActionResult> GetConfirmationOfStandard()
    {
        var (sessionModel, redirectResult) = GetSessionModelWithEscapeRoute(_logger);
        if (sessionModel == null) return redirectResult;

        var model = await GetViewModel(sessionModel.LarsCode);

        return View(ViewPath, model);
    }

    [HttpPost(Name = RouteNames.PostAddStandardConfirmNonRegulatedStandard)]
    public async Task<IActionResult> SubmitConfirmationOfStandard(int ukprn, ConfirmNonRegulatedStandardSubmitModel submitModel)
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

        if (submitModel.IsCorrectStandard == false)
        {
            return RedirectToRouteWithUkprn(RouteNames.GetAddStandardSelectStandard);
        }

        sessionModel.IsConfirmed = true;
        sessionModel.StandardInformation = await _mediator.Send(new GetStandardInformationQuery(sessionModel.LarsCode));

        _sessionService.Set(sessionModel);

        if (sessionModel.LatestProviderContactModel != null)
        {
            return RedirectToRouteWithUkprn(RouteNames.GetAddStandardUseSavedContactDetails);
        }

        _logger.LogInformation("Add standard: Non-regulated standard confirmed for ukprn:{Ukprn} larscode:{LarsCode}", Ukprn, sessionModel.LarsCode);

        return RedirectToRouteWithUkprn(RouteNames.GetAddStandardAddContactDetails);
    }

    private async Task<ConfirmNonRegulatedStandardViewModel> GetViewModel(string larsCode)
    {
        var standardInfo = await _mediator.Send(new GetStandardInformationQuery(larsCode));
        var model = new ConfirmNonRegulatedStandardViewModel()
        {
            StandardInformation = standardInfo
        };
        return model;
    }
}
