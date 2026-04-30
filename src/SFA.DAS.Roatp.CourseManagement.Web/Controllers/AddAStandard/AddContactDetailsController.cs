using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Extensions;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAStandard;

[AuthorizeCourseType(CourseType.Apprenticeship)]
[Route("{ukprn}/standards/add/contact-details")]
public class AddContactDetailsController : AddAStandardControllerBase
{
    public const string ViewPath = "~/Views/AddAStandard/AddStandardContactDetails.cshtml";
    private readonly ILogger<AddContactDetailsController> _logger;
    private readonly IValidator<CourseContactDetailsSubmitModel> _validator;

    public AddContactDetailsController(ILogger<AddContactDetailsController> logger, ISessionService sessionService, IValidator<CourseContactDetailsSubmitModel> validator) : base(sessionService)
    {
        _logger = logger;
        _validator = validator;
    }

    [HttpGet(Name = RouteNames.GetAddStandardAddContactDetails)]
    public IActionResult GetContactDetails()
    {
        var (sessionModel, redirectResult) = GetSessionModelWithEscapeRoute(_logger);
        if (sessionModel == null) return redirectResult;

        var model = new AddStandardContactDetailsViewModel();
        if (sessionModel.ContactInformation != null)
        {
            model.ContactUsEmail = sessionModel.ContactInformation.ContactUsEmail;
            model.ContactUsPhoneNumber = sessionModel.ContactInformation.ContactUsPhoneNumber;
            model.StandardInfoUrl = sessionModel.ContactInformation.StandardInfoUrl;
        }

        if (IsUsingSavedContactDetails(sessionModel))
        {
            model.ContactUsEmail = sessionModel.LatestProviderContactModel.EmailAddress;
            model.ContactUsPhoneNumber = sessionModel.LatestProviderContactModel.PhoneNumber;
            model.StandardInfoUrl = null;
            model.ShowSavedContactDetailsText = true;
        }

        return View(ViewPath, model);
    }

    [HttpPost(Name = RouteNames.PostAddStandardAddContactDetails)]
    public IActionResult SubmitContactDetails(CourseContactDetailsSubmitModel submitModel)
    {
        var (sessionModel, redirectResult) = GetSessionModelWithEscapeRoute(_logger);
        if (sessionModel == null) return redirectResult;

        var validatedResult = _validator.Validate(submitModel);

        if (!validatedResult.IsValid)
        {
            var model = new AddStandardContactDetailsViewModel()
            {
                ContactUsEmail = submitModel.ContactUsEmail,
                ContactUsPhoneNumber = submitModel.ContactUsPhoneNumber,
                StandardInfoUrl = submitModel.StandardInfoUrl,
                ShowSavedContactDetailsText = IsUsingSavedContactDetails(sessionModel)
            };

            ModelState.AddValidationErrors(validatedResult.Errors);

            return View(ViewPath, model);
        }

        sessionModel.ContactInformation.ContactUsEmail = submitModel.ContactUsEmail;
        sessionModel.ContactInformation.ContactUsPhoneNumber = submitModel.ContactUsPhoneNumber;
        sessionModel.ContactInformation.StandardInfoUrl = submitModel.StandardInfoUrl;

        _sessionService.Set(sessionModel);

        _logger.LogInformation("Add standard: Contact details added for ukprn:{Ukprn} larscode:{LarsCode}", Ukprn, sessionModel.LarsCode);

        return RedirectToRouteWithUkprn(RouteNames.GetAddStandardSelectLocationOption);
    }


    private static bool IsUsingSavedContactDetails(StandardSessionModel sessionModel)
    {
        return sessionModel.LatestProviderContactModel != null && sessionModel.IsUsingSavedContactDetails is true;
    }
}
