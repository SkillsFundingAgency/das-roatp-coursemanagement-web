using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.Validators.AddAStandard;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAStandard;

[Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
[Route("{ukprn}/standards/add/use-provider-contact", Name = RouteNames.GetAddStandardUseSavedContactDetails)]
public class UseSavedContactDetailsController : AddAStandardControllerBase
{
    public const string ViewPath = "~/Views/AddAStandard/UseSavedContactDetails.cshtml";
    private readonly ILogger<UseSavedContactDetailsController> _logger;

    public UseSavedContactDetailsController(
        ISessionService sessionService,
        ILogger<UseSavedContactDetailsController> logger) : base(sessionService)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult UseSavedContactDetails(int ukprn)
    {
        var (sessionModel, redirectResult) = GetSessionModelWithEscapeRoute(_logger);
        if (sessionModel?.LatestProviderContactModel == null) return redirectResult;

        var model = GetViewModel(sessionModel, ukprn);

        return View(ViewPath, model);
    }

    [HttpPost]
    public IActionResult SubmitTrainingLocations(UseSavedContactDetailsViewModel model)
    {
        var (sessionModel, redirectResult) = GetSessionModelWithEscapeRoute(_logger);
        if (sessionModel == null) return redirectResult;

        var validator = new UseSavedContactDetailsViewModelValidator();
        var validatorResult = validator.Validate(model);

        if (!validatorResult.IsValid)
        {
            return View(ViewPath, model);
        }

        sessionModel.IsUsingSavedContactDetails = model.IsUsingSavedContactDetails;
        _sessionService.Set(sessionModel);

        return RedirectToRouteWithUkprn(RouteNames.GetAddStandardAddContactDetails);

    }

    private static UseSavedContactDetailsViewModel GetViewModel(StandardSessionModel sessionModel, int ukprn)
    {
        return new UseSavedContactDetailsViewModel
        {
            Ukprn = ukprn,
            EmailAddress = sessionModel.LatestProviderContactModel.EmailAddress,
            PhoneNumber = sessionModel.LatestProviderContactModel.PhoneNumber,
            ShowEmail = !string.IsNullOrWhiteSpace(sessionModel.LatestProviderContactModel.EmailAddress),
            ShowPhone = !string.IsNullOrWhiteSpace(sessionModel.LatestProviderContactModel.PhoneNumber),
            IsUsingSavedContactDetails = sessionModel.IsUsingSavedContactDetails
        };
    }
}
