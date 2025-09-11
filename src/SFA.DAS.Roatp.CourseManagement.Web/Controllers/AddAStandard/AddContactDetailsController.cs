using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAStandard
{
    [Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
    public class AddContactDetailsController : AddAStandardControllerBase
    {
        public const string ViewPath = "~/Views/AddAStandard/AddStandardContactDetails.cshtml";
        private readonly ILogger<AddContactDetailsController> _logger;

        public AddContactDetailsController(ILogger<AddContactDetailsController> logger, ISessionService sessionService) : base(sessionService)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("{ukprn}/standards/add/contact-details", Name = RouteNames.GetAddStandardAddContactDetails)]
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

            if (sessionModel.LatestProviderContactModel != null && sessionModel.IsUsingSavedContactDetails is true)
            {
                model.ContactUsEmail = sessionModel.LatestProviderContactModel.EmailAddress;
                model.ContactUsPhoneNumber = sessionModel.LatestProviderContactModel.PhoneNumber;
                model.StandardInfoUrl = null;
                model.ShowSavedContactDetailsText = true;
            }

            model.CancelLink = GetUrlWithUkprn(RouteNames.ViewStandards);
            return View(ViewPath, model);
        }

        [HttpPost]
        [Route("{ukprn}/standards/add/contact-details", Name = RouteNames.PostAddStandardAddContactDetails)]
        public IActionResult SubmitContactDetails(CourseContactDetailsSubmitModel submitModel)
        {
            var (sessionModel, redirectResult) = GetSessionModelWithEscapeRoute(_logger);
            if (sessionModel == null) return redirectResult;

            if (!ModelState.IsValid)
            {
                var model = new AddStandardContactDetailsViewModel()
                {
                    ContactUsEmail = submitModel.ContactUsEmail,
                    ContactUsPhoneNumber = submitModel.ContactUsPhoneNumber,
                    StandardInfoUrl = submitModel.StandardInfoUrl
                };
                model.CancelLink = GetUrlWithUkprn(RouteNames.ViewStandards);
                return View(ViewPath, model);
            }

            sessionModel.ContactInformation.ContactUsEmail = submitModel.ContactUsEmail;
            sessionModel.ContactInformation.ContactUsPhoneNumber = submitModel.ContactUsPhoneNumber;
            sessionModel.ContactInformation.StandardInfoUrl = submitModel.StandardInfoUrl;

            _sessionService.Set(sessionModel);

            _logger.LogInformation("Add standard: Contact details added for ukprn:{ukprn} larscode:{larscode}", Ukprn, sessionModel.LarsCode);

            return RedirectToRouteWithUkprn(RouteNames.GetAddStandardSelectLocationOption);
        }
    }
}
