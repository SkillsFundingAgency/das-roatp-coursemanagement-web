using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderCourseLocations;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAStandard
{
    [DasAuthorize(new[] { "ProviderFeature.CourseManagement" }, Policy = nameof(PolicyNames.HasProviderAccount))]

    public class ViewTrainingLocationsController : AddAStandardControllerBase
    {
        public const string ViewPath = "~/Views/AddAStandard/ViewTrainingLocations.cshtml";
        private readonly ILogger<ViewTrainingLocationsController> _logger;

    
        public ViewTrainingLocationsController(
            ISessionService sessionService,
            ILogger<ViewTrainingLocationsController> logger) : base(sessionService)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("{ukprn}/standards/view-training-locations", Name = RouteNames.GetNewStandardViewTrainingLocationOptions)]
        public IActionResult ViewTrainingLocations()
        {
            var (sessionModel, redirectResult) = GetSessionModelWithEscapeRoute(_logger);
            if (sessionModel == null) return redirectResult;

            var model = GetModel(sessionModel.LarsCode);
            model.ProviderCourseLocations = MapProviderLocationsToProviderCourseLocations(sessionModel.ProviderLocations);
            model.FirstLocation = model.ProviderCourseLocations.FirstOrDefault()?.LocationName;

            return View(ViewPath, model);
        }

        [HttpPost]
        [Route("{ukprn}/standards/view-training-locations", Name = RouteNames.PostNewStandardConfirmTrainingLocationOptions)]
        public IActionResult SubmitTrainingLocations(TrainingLocationListViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(ViewPath, GetModel(model.LarsCode));
            }

            var (sessionModel, redirectResult) = GetSessionModelWithEscapeRoute(_logger);
            if (sessionModel == null) return redirectResult;

            return RedirectToRouteWithUkprn(RouteNames.GetAddStandardReviewStandard);
        }

        private TrainingLocationListViewModel GetModel(int larsCode) => new TrainingLocationListViewModel
        {
            BackLink = GetUrlWithUkprn(RouteNames.GetAddStandardSelectLocationOption),
            CancelLink = GetUrlWithUkprn(RouteNames.GetAddStandardSelectLocationOption),
            AddTrainingLocationUrl = Url.RouteUrl(RouteNames.GetNewStandardAddProviderCourseLocation, new { Ukprn, larsCode })
        };
        private List<ProviderCourseLocationViewModel> MapProviderLocationsToProviderCourseLocations(IEnumerable<CourseLocationModel> sessionModelProviderLocations)
        {
            var providerCourseLocations = new List<ProviderCourseLocationViewModel>();
            foreach (var location in sessionModelProviderLocations)
            {
                providerCourseLocations.Add(new ProviderCourseLocationViewModel
                {
                    DeliveryMethod = location.DeliveryMethod,
                    LocationName = location.LocationName,
                    LocationType = location.LocationType
                });
            }

            return providerCourseLocations;
        }
    }
}