using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Extensions;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAStandard;

[AuthorizeCourseType(CourseType.Apprenticeship)]
[Route("{ukprn}/standards/add/use-provider-contact", Name = RouteNames.GetAddStandardUseSavedContactDetails)]
public class UseSavedContactDetailsController : AddAStandardControllerBase
{
    public const string ViewPath = "~/Views/AddAStandard/UseSavedContactDetails.cshtml";
    private readonly ILogger<UseSavedContactDetailsController> _logger;
    private readonly IValidator<UseSavedContactDetailsSubmitViewModel> _validator;

    public UseSavedContactDetailsController(
        ISessionService sessionService,
        ILogger<UseSavedContactDetailsController> logger,
        IValidator<UseSavedContactDetailsSubmitViewModel> validator) : base(sessionService)
    {
        _logger = logger;
        _validator = validator;
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
    public IActionResult PostUseSavedContactDetails(UseSavedContactDetailsSubmitViewModel model)
    {
        var sessionModel = _sessionService.Get<StandardSessionModel>();
        if (sessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

        var validatedResult = _validator.Validate(model);

        if (!validatedResult.IsValid)
        {
            var viewModel = GetViewModel(sessionModel, Ukprn);

            ModelState.AddValidationErrors(validatedResult.Errors);

            return View(ViewPath, viewModel);
        }

        sessionModel.IsUsingSavedContactDetails = model.IsUsingSavedContactDetails;
        sessionModel.ContactInformation = new();
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
