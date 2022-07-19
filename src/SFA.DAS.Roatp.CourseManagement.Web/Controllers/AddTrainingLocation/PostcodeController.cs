using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddTrainingLocation
{
    [DasAuthorize(new[] { "ProviderFeature.CourseManagement" }, Policy = nameof(PolicyNames.HasProviderAccount))]
    public class PostcodeController : ControllerBase
    {
        private readonly ISessionService _sessionService;

        public PostcodeController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public const string ViewPath = "~/Views/AddTrainingLocation/Postcode.cshtml";
        [Route("{ukprn}/add-training-location/postcode", Name = RouteNames.GetTrainingLocationPostcode)]
        [HttpGet]
        public IActionResult GetPostcode()
        {
            _sessionService.Delete(SessionKeys.SelectedPostcode, Ukprn.ToString());
            var model = new PostcodeViewModel();
            model.BackLink = model.CancelLink = Url.RouteUrl(RouteNames.GetProviderLocations, new { Ukprn });
            return View(ViewPath, model);
        }

        [Route("{ukprn}/add-training-location/postcode", Name = RouteNames.PostTrainingLocationPostcode)]
        [HttpPost]
        public IActionResult SubmitPostcode(PostcodeSubmitModel model)
        {
            if (!ModelState.IsValid)
            {
                return GetPostcode();
            }

            _sessionService.Set(model.Postcode.ToUpper(), SessionKeys.SelectedPostcode, Ukprn.ToString());
            return RedirectToRouteWithUkprn(RouteNames.GetTrainingLocationAddress);
        }
    }
}
