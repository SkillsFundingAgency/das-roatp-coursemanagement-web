using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAllProviderLocations;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAvailableProviderLocations;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.AddProviderCourseLocation;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAStandard
{
    [DasAuthorize(new[] { "ProviderFeature.CourseManagement" }, Policy = nameof(PolicyNames.HasProviderAccount))]
    public class LocationAddController : ControllerBase
    {
        public const string ViewPath = "~/Views/AddAStandard/AddCourseLocation.cshtml";
        private readonly ILogger<LocationAddController> _logger;
        private readonly IMediator _mediator;
        private readonly ISessionService _sessionService;

        public LocationAddController(IMediator mediator, ISessionService sessionService, ILogger<LocationAddController> logger)
        {

            _mediator = mediator;
            _sessionService = sessionService;
            _logger = logger;
        }

        [Route("{ukprn}/standards/{larsCode}/locations/add-new", Name = RouteNames.GetNewStandardAddProviderCourseLocation)]
        [HttpGet]
        public async Task<IActionResult> SelectAProviderlocation(int larsCode)
        {
            var model = await GetModel(larsCode);
            return View(ViewPath, model);
        }

        [Route("{ukprn}/standards/{larscode}/locations/add-new", Name = RouteNames.PostNewStandardAddProviderCourseLocation)]
        [HttpPost]
        public async Task<IActionResult> SubmitAProviderlocation(CourseLocationAddSubmitModel submitModel, int larsCode)
        {
            var model = await GetModel(larsCode);
            if (!ModelState.IsValid)
            {
        
                //MFCMFC if model is null???

                return View(ViewPath, model);
            }
            // var addLocationModel = new CourseLocationAddSubmitModel()
            // {
            //     //Ukprn = base.Ukprn,
            //     // UserId = base.UserId,
            //     // LarsCode = larsCode,
            //     TrainingVenueNavigationId = submitModel.TrainingVenueNavigationId,
            //     HasDayReleaseDeliveryOption = submitModel.HasDayReleaseDeliveryOption,
            //     HasBlockReleaseDeliveryOption = submitModel.HasBlockReleaseDeliveryOption
            // };

            //await _mediator.Send(command);

            // MFCMFC
            // add choice to session object
            var sessionModel = _sessionService.Get<StandardSessionModel>(Ukprn.ToString());
            if (sessionModel.CourseLocations == null)
                sessionModel.CourseLocations = new List<CourseLocationModel>() ;

           sessionModel.CourseLocations.Add(new CourseLocationModel
           {
               LocationType = LocationType.Provider,
               ProviderLocationId = Guid.Parse(submitModel.TrainingVenueNavigationId),
               LocationName = model.TrainingVenues.First(x=>x.Value==submitModel.TrainingVenueNavigationId).Text,
               DeliveryMethod = new DeliveryMethodModel
               {
                   HasBlockReleaseDeliveryOption = submitModel.HasBlockReleaseDeliveryOption,
                   HasDayReleaseDeliveryOption = submitModel.HasDayReleaseDeliveryOption
               }
           });

           _sessionService.Set(sessionModel, Ukprn.ToString());

            return RedirectToRoute(RouteNames.GetNewStandardViewTrainingLocationOptions, new { ukprn = Ukprn });
        }

        private async Task<CourseLocationAddViewModel> GetModel(int larsCode)
        {
            _logger.LogInformation("Getting provider course locations for ukprn {ukprn} for larsCode {larsCode}", Ukprn, larsCode);


            var result = await _mediator.Send(new GetAllProviderLocationsQuery(Ukprn));
            var allProviderLocations = result.ProviderLocations;

            var availableProviderLocations = new List<ProviderLocation>();

            var sessionModel = _sessionService.Get<StandardSessionModel>(Ukprn.ToString());
            if (sessionModel == null || sessionModel.LarsCode <= 0) return null;

            foreach (var location in allProviderLocations.Where(x => x.LocationType == LocationType.Provider))
            {
                if (!sessionModel.ProviderLocations.Any(x => x.LocationType == LocationType.Provider && x.LocationName == location.LocationName))
                    availableProviderLocations.Add(location);
            }

            var model = new CourseLocationAddViewModel
            {
                TrainingVenues = availableProviderLocations.OrderBy(c => c.LocationName).Select(s => new SelectListItem($"{s.LocationName}", s.NavigationId.ToString())),
                LarsCode = larsCode
            };
            // MFCMFC  this back link needs to change
            model.BackLink = model.CancelLink = Url.RouteUrl(RouteNames.GetNewStandardViewTrainingLocationOptions, new { ukprn = Ukprn });
            return model;
        }
    }
}