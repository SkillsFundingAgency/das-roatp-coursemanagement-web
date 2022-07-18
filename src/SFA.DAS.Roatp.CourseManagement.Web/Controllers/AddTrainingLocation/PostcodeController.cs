using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddTrainingLocation
{
    [DasAuthorize(new[] { "ProviderFeature.CourseManagement" }, Policy = nameof(PolicyNames.HasProviderAccount))]
    public class PostcodeController : ControllerBase
    {
        public const string ViewPath = "~/Views/AddTrainingLocation/Postcode.cshtml";
        [Route("{ukprn}/add-training-location/postcode", Name = RouteNames.GetTrainingLocationPostcode)]
        [HttpGet]
        public IActionResult GetPostcode()
        {
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

            return RedirectToRoute(RouteNames.GetTrainingLocationAddress, new { Ukprn, model.Postcode });
        }
    }
}
