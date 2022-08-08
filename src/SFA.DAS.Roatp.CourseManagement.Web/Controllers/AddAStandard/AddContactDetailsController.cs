using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAStandard
{
    [DasAuthorize(new[] { "ProviderFeature.CourseManagement" }, Policy = nameof(PolicyNames.HasProviderAccount))]
    public class AddContactDetailsController : AddAStandardControllerBase
    {
        public const string ViewPath = "~/Views/AddAStandard/AddStandardContactDetails.cshtml";
        private readonly ILogger<AddContactDetailsController> _logger;

        public AddContactDetailsController(ILogger<AddContactDetailsController> logger, ISessionService sessionService) : base (sessionService)
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
            return View(ViewPath, model);
        }

        [HttpPost]
        [Route("{ukprn}/standards/add/contact-details", Name = RouteNames.PostAddStandardAddContactDetails)]
        public IActionResult SubmitContactDetails(EditCourseContactDetailsSubmitModel submitModel)
        {
            var (sessionModel, redirectResult) = GetSessionModelWithEscapeRoute(_logger);
            if (sessionModel == null) return redirectResult;

            if (!ModelState.IsValid)
            {
                var model = new AddStandardContactDetailsViewModel() 
                {
                    ContactUsEmail = submitModel.ContactUsEmail,
                    ContactUsPhoneNumber = submitModel.ContactUsPhoneNumber,
                    ContactUsPageUrl = submitModel.ContactUsPageUrl,
                    StandardInfoUrl = submitModel.StandardInfoUrl
                };
                return View(ViewPath, model);
            }

            sessionModel.ContactUsEmail = submitModel.ContactUsEmail;
            sessionModel.ContactUsPhoneNumber = submitModel.ContactUsPhoneNumber;
            sessionModel.ContactUsPageUrl = submitModel.ContactUsPageUrl;
            sessionModel.StandardInfoUrl = submitModel.StandardInfoUrl;

            _sessionService.Set(sessionModel, Ukprn.ToString());

            return Ok();
        }
    }
}
