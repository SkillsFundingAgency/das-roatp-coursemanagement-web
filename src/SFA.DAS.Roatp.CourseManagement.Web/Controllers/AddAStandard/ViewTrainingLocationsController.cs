using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAStandard
{
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

            // if (sessionModel.LocationOption != LocationOption.EmployerLocation && sessionModel.LocationOption != LocationOption.Both)
            // {
            //     _logger.LogInformation("Add standard national option: location option {locationOption} in session does not allow this question, navigating back to select standard", sessionModel.LocationOption);
            //     return RedirectToRouteWithUkprn(RouteNames.GetAddStandardSelectStandard);
            // }
            var model = GetModel(sessionModel.LarsCode);
            var xn = GetUrlWithUkprn(RouteNames.GetNewStandardAddProviderCourseLocation);
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

            // if (sessionModel.LocationOption != LocationOption.EmployerLocation && sessionModel.LocationOption != LocationOption.Both)
            // {
            //     _logger.LogInformation("Add standard national option: location option {locationOption} in session does not allow this question, navigating back to select standard", sessionModel.LocationOption);
            //     return RedirectToRouteWithUkprn(RouteNames.GetAddStandardSelectStandard);
            // }

            return View(ViewPath, GetModel(model.LarsCode));
        }




        private TrainingLocationListViewModel GetModel(int larsCode) => new TrainingLocationListViewModel
        {
            BackUrl = GetUrlWithUkprn(RouteNames.GetAddStandardSelectLocationOption),
            CancelUrl = GetUrlWithUkprn(RouteNames.GetAddStandardSelectLocationOption),
            AddTrainingLocationUrl = Url.RouteUrl(RouteNames.GetNewStandardAddProviderCourseLocation, new { Ukprn, larsCode })
        };
    }
}