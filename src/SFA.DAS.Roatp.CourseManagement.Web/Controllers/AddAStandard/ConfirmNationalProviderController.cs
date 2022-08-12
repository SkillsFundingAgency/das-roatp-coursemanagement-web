using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAStandard
{
    [DasAuthorize(new[] { "ProviderFeature.CourseManagement" }, Policy = nameof(PolicyNames.HasProviderAccount))]
    public class ConfirmNationalProviderController : AddAStandardControllerBase
    {
        public const string ViewPath = "~/Views/AddAStandard/ConfirmNationalProvider.cshtml";
        private readonly ILogger<ConfirmNationalProviderController> _logger;

        public ConfirmNationalProviderController(
            ISessionService sessionService,
            ILogger<ConfirmNationalProviderController> logger) : base(sessionService)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("{ukprn}/standards/add/confirm-national", Name = RouteNames.GetAddStandardConfirmNationalProvider)]
        public IActionResult ConfirmNationalDeliveryOption()
        {
            var (sessionModel, redirectResult) = GetSessionModelWithEscapeRoute(_logger);
            if (sessionModel == null) return redirectResult;

            if (sessionModel.LocationOption != LocationOption.EmployerLocation && sessionModel.LocationOption != LocationOption.Both)
            {
                _logger.LogInformation("Add standard national option: location option {locationOption} in session does not allow this question, navigating back to select standard", sessionModel.LocationOption);
                return RedirectToRouteWithUkprn(RouteNames.GetAddStandardSelectStandard);
            }

            return View(ViewPath, GetModel());
        }

        [HttpPost]
        [Route("{ukprn}/standards/add/confirm-national", Name = RouteNames.PostAddStandardConfirmNationalProvider)]
        public IActionResult SubmitConfirmationOnNationalProvider(ConfirmNationalProviderSubmitModel submitModel)
        {
            var (sessionModel, redirectResult) = GetSessionModelWithEscapeRoute(_logger);
            if (sessionModel == null) return redirectResult;

            if (!ModelState.IsValid)
            {
                return View(ViewPath, GetModel());
            }

            sessionModel.HasNationalDeliveryOption = submitModel.HasNationalDeliveryOption;
            _sessionService.Set(sessionModel, Ukprn.ToString());

            _logger.LogInformation("Add standard: national delivery option set to {nationaldeliveryoption} for ukprn:{ukprn} larscode:{larscode}", submitModel.HasNationalDeliveryOption, Ukprn, sessionModel.LarsCode);

            return Ok();
        }

        private ConfirmNationalProviderViewModel GetModel() => new ConfirmNationalProviderViewModel
        {
            CancelLink = GetUrlWithUkprn(RouteNames.ViewStandards)
        };
    }
}
