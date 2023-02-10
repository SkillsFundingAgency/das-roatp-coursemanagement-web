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
    [DasAuthorize( Policy = nameof(PolicyNames.HasProviderAccount))]
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

            if (sessionModel.LocationOption == LocationOption.ProviderLocation)
            {
                _logger.LogWarning($"User: {UserId} unexpectedly landed on national delivery confirmation page when location option is set to providers.");
                return RedirectToRouteWithUkprn(RouteNames.ViewStandards);
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
            _sessionService.Set(sessionModel);

            _logger.LogInformation("Add standard: national delivery option set to {nationaldeliveryoption} for ukprn:{ukprn} larscode:{larscode}", submitModel.HasNationalDeliveryOption, Ukprn, sessionModel.LarsCode);

            if (submitModel.HasNationalDeliveryOption.GetValueOrDefault())
            {
                _logger.LogInformation("National option available for standard:{larscode} Ukprn:{ukprn}", sessionModel.LarsCode, Ukprn);
                return RedirectToRouteWithUkprn(RouteNames.GetAddStandardReviewStandard);
            }
            else
            {
                _logger.LogInformation("National option NOT available for standard:{larscode} Ukprn:{ukprn}", sessionModel.LarsCode, Ukprn);
                return RedirectToRouteWithUkprn(RouteNames.GetAddStandardAddRegions);
            }
        }

        private ConfirmNationalProviderViewModel GetModel() => new ConfirmNationalProviderViewModel
        {
            CancelLink = GetUrlWithUkprn(RouteNames.ViewStandards)
        };
    }
}
