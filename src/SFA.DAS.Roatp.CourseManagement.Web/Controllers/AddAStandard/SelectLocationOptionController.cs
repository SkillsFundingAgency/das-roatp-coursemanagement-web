using System.Collections.Generic;
using System.Linq;
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
    public class SelectLocationOptionController : AddAStandardControllerBase
    {
        public const string ViewPath = "~/Views/AddAStandard/SelectLocationOption.cshtml";
        private readonly ILogger<SelectLocationOptionController> _logger;
        public SelectLocationOptionController(ISessionService sessionService, ILogger<SelectLocationOptionController> logger) : base(sessionService)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("{ukprn}/standards/add/select-location-option", Name = RouteNames.GetAddStandardSelectLocationOption)]
        public IActionResult SelectLocationOption()
        {
            var (sessionModel, redirectResult) = GetSessionModelWithEscapeRoute(_logger);
            if (sessionModel == null) return redirectResult;
           
            var model = GetModel();
            
            model.LocationOption = sessionModel.LocationOption;
            if (sessionModel.CourseLocations != null && sessionModel.CourseLocations.Any())
            {
                sessionModel.CourseLocations = new List<CourseLocationModel>();
                _sessionService.Set(sessionModel, Ukprn.ToString());
            }
            return View(ViewPath, model);
        }

        [HttpPost]
        [Route("{ukprn}/standards/add/select-location-option", Name = RouteNames.PostAddStandardSelectLocationOption)]
        public IActionResult SubmitLocationOption(LocationOptionSubmitModel submitModel)
        {
            var (sessionModel, redirectResult) = GetSessionModelWithEscapeRoute(_logger);
            if (sessionModel == null) return redirectResult;

            if (!ModelState.IsValid)
            {
                return View(ViewPath, GetModel());
            }

            _logger.LogInformation("Add standard: Location option added to {locationOption} for ukprn:{ukprn} larscode:{larscode}", submitModel.LocationOption, Ukprn, sessionModel.LarsCode);

            if (submitModel.LocationOption == LocationOption.EmployerLocation)
            {
                return RedirectToRouteWithUkprn(RouteNames.GetAddStandardConfirmNationalProvider);
            }
            // If the location option is provider or both then we need to navigate to provider location page which is covered in a follow up story
            return RedirectToRouteWithUkprn(RouteNames.GetNewStandardViewTrainingLocationOptions);
        }

        private SelectLocationOptionViewModel GetModel() => new SelectLocationOptionViewModel
        {
            CancelLink = GetUrlWithUkprn(RouteNames.ViewStandards)
        };
    }
}
