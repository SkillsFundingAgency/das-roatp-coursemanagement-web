using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Extensions;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAStandard;

[AuthorizeCourseType(CourseType.Apprenticeship)]
[Route("{ukprn}/standards/add/confirm-national")]
public class ConfirmNationalProviderController : AddAStandardControllerBase
{
    public const string ViewPath = "~/Views/AddAStandard/ConfirmNationalProvider.cshtml";
    private readonly ILogger<ConfirmNationalProviderController> _logger;
    private readonly IValidator<ConfirmNationalProviderSubmitModel> _validator;

    public ConfirmNationalProviderController(
        ISessionService sessionService,
        ILogger<ConfirmNationalProviderController> logger,
        IValidator<ConfirmNationalProviderSubmitModel> validator) : base(sessionService)
    {
        _logger = logger;
        _validator = validator;
    }

    [HttpGet(Name = RouteNames.GetAddStandardConfirmNationalProvider)]
    public IActionResult ConfirmNationalDeliveryOption()
    {
        var (sessionModel, redirectResult) = GetSessionModelWithEscapeRoute(_logger);
        if (sessionModel == null) return redirectResult;

        if (sessionModel.LocationOption == LocationOption.ProviderLocation)
        {
            _logger.LogWarning("User: {UserId} unexpectedly landed on national delivery confirmation page when location option is set to providers.", UserId);
            return RedirectToRouteWithUkprn(RouteNames.ViewStandards);
        }

        return View(ViewPath, new ConfirmNationalProviderViewModel());
    }

    [HttpPost(Name = RouteNames.PostAddStandardConfirmNationalProvider)]
    public IActionResult SubmitConfirmationOnNationalProvider(ConfirmNationalProviderSubmitModel submitModel)
    {
        var (sessionModel, redirectResult) = GetSessionModelWithEscapeRoute(_logger);
        if (sessionModel == null) return redirectResult;

        var validatedResult = _validator.Validate(submitModel);

        if (!validatedResult.IsValid)
        {
            ModelState.AddValidationErrors(validatedResult.Errors);

            return View(ViewPath, new ConfirmNationalProviderViewModel());
        }

        sessionModel.HasNationalDeliveryOption = submitModel.HasNationalDeliveryOption;
        _sessionService.Set(sessionModel);

        _logger.LogInformation("Add standard: national delivery option set to {NationalDeliveryOption} for ukprn:{Ukprn} larscode:{LarsCode}", submitModel.HasNationalDeliveryOption, Ukprn, sessionModel.LarsCode);

        if (submitModel.HasNationalDeliveryOption.GetValueOrDefault())
        {
            _logger.LogInformation("National option available for standard:{LarsCode} Ukprn:{Ukprn}", sessionModel.LarsCode, Ukprn);
            return RedirectToRouteWithUkprn(RouteNames.GetAddStandardReviewStandard);
        }
        else
        {
            _logger.LogInformation("National option NOT available for standard:{LarsCode} Ukprn:{Ukprn}", sessionModel.LarsCode, Ukprn);
            return RedirectToRouteWithUkprn(RouteNames.GetAddStandardAddRegions);
        }
    }
}
