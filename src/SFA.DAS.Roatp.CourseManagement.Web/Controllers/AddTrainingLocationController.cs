using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderLocations.AddProviderLocation;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    [DasAuthorize(new[] { "ProviderFeature.CourseManagement" }, Policy = nameof(PolicyNames.HasProviderAccount))]
    public class AddTrainingLocationController : ControllerBase
    {
        private readonly ISessionService _sessionService;
        private readonly ILogger<AddTrainingLocationController> _logger;

        public AddTrainingLocationController(ISessionService sessionService, ILogger<AddTrainingLocationController> logger)
        {
            _sessionService = sessionService;
            _logger = logger;
        }

        [Route("{ukprn}/add-training-location/postcode", Name = RouteNames.GetTrainingLocationPostcode)]
        [HttpGet]
        public IActionResult Postcode()
        {
            _sessionService.Delete(SessionKeys.SelectedPostcode, Ukprn.ToString());
            var model = new ProviderLocationPostcodeViewModel();
            model.BackLink = model.CancelLink = Url.RouteUrl(RouteNames.GetProviderLocations, new { Ukprn });
            return View(model);
        }

        [Route("{ukprn}/add-training-location/postcode", Name = RouteNames.PostTrainingLocationPostcode)]
        [HttpPost]
        public IActionResult Postcode(ProviderLocationPostcodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.BackLink = model.CancelLink = Url.RouteUrl(RouteNames.GetProviderLocations, new { Ukprn });
                return View(model);
            }

            _logger.LogInformation("Setting postcode: {postcode} in session for user: {userid}", model.Postcode, UserId);
            _sessionService.Set(model.Postcode, SessionKeys.SelectedPostcode, Ukprn.ToString());
            return RedirectToRoute(RouteNames.GetTrainingLocationAddress);
        }
    }
}
