using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderCourseLocations;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.Validators.AddAStandard;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAStandard
{
    [DasAuthorize(new[] { "ProviderFeature.CourseManagement" }, Policy = nameof(PolicyNames.HasProviderAccount))]

    public class StandardTrainingLocationsController : AddAStandardControllerBase
    {
        public const string ViewPath = "~/Views/AddAStandard/StandardTrainingLocations.cshtml";
        private readonly ILogger<StandardTrainingLocationsController> _logger;

    
        public StandardTrainingLocationsController(
            ISessionService sessionService,
            ILogger<StandardTrainingLocationsController> logger) : base(sessionService)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("{ukprn}/standards/add/locations", Name = RouteNames.GetNewStandardViewTrainingLocationOptions)]
        public IActionResult ViewTrainingLocations()
        {
            var (sessionModel, redirectResult) = GetSessionModelWithEscapeRoute(_logger);
            if (sessionModel == null) return redirectResult;

            var model = GetModel();
            model.ProviderCourseLocations = MapProviderLocationsToProviderCourseLocations(sessionModel.ProviderLocations);
            
            return View(ViewPath, model);
        }

        [HttpPost]
        [Route("{ukprn}/standards/add/locations", Name = RouteNames.PostNewStandardConfirmTrainingLocationOptions)]
        public IActionResult SubmitTrainingLocations(TrainingLocationListViewModel model)
        {
            var (sessionModel, redirectResult) = GetSessionModelWithEscapeRoute(_logger);
            if (sessionModel == null) return redirectResult;
            model.ProviderCourseLocations = MapProviderLocationsToProviderCourseLocations(sessionModel.ProviderLocations);
            var validator = new TrainingLocationListViewModelValidator();
            var validatorResult = validator.Validate(model);
            if (!validatorResult.IsValid)
            {
                return View(ViewPath, model);
            }
            
            return RedirectToRouteWithUkprn(RouteNames.GetAddStandardReviewStandard);
        }

        private TrainingLocationListViewModel GetModel() => new TrainingLocationListViewModel
        {
            BackLink = GetUrlWithUkprn(RouteNames.GetAddStandardSelectLocationOption),
            CancelLink = GetUrlWithUkprn(RouteNames.GetAddStandardSelectLocationOption),
            AddTrainingLocationUrl = Url.RouteUrl(RouteNames.GetAddStandardTrainingLocation, new { Ukprn })
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