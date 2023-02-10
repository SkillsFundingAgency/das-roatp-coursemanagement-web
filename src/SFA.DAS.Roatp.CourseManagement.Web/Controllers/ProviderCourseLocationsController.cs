using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderCourseLocations;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.Validators;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    [DasAuthorize( Policy = nameof(PolicyNames.HasProviderAccount))]
    public class ProviderCourseLocationsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProviderCourseLocationsController> _logger;
        private readonly ISessionService _sessionService;

        public ProviderCourseLocationsController(IMediator mediator, ILogger<ProviderCourseLocationsController> logger, ISessionService sessionService)
        {
            _mediator = mediator;
            _logger = logger;
            _sessionService = sessionService;
        }

        [Route("{ukprn}/standards/{larsCode}/providerlocations", Name = RouteNames.GetProviderCourseLocations)]
        [HttpGet]
        public async Task<IActionResult> GetProviderCourseLocations([FromRoute] int larsCode)
        {
            _logger.LogInformation("Getting Provider Course Locations for ukprn {ukprn} ", Ukprn);

            ProviderCourseLocationListViewModel model = await BuildViewModel(larsCode);

            return View("~/Views/ProviderCourseLocations/EditTrainingLocations.cshtml", model);
        }

        private async Task<ProviderCourseLocationListViewModel> BuildViewModel(int larsCode)
        {
            var result = await _mediator.Send(new GetProviderCourseLocationsQuery(Ukprn, larsCode));

            if (result == null)
            {
                var message = $"Provider Course Locations not found for ukprn {Ukprn} and larscode {larsCode}";
                _logger.LogError(message);
                result = new GetProviderCourseLocationsQueryResult();
            }

            var model = new ProviderCourseLocationListViewModel
            {
                ProviderCourseLocations = result.ProviderCourseLocations.Select(x => (ProviderCourseLocationViewModel)x).ToList(),
                LarsCode = larsCode
            };
            foreach (var location in model.ProviderCourseLocations)
            {
                location.RemoveUrl = Url.RouteUrl(RouteNames.GetRemoveProviderCourseLocation, new { ukprn = Ukprn, larsCode, id = location.Id });
            }
            if (Request.GetTypedHeaders().Referer == null)
            {
                model.BackUrl = "#";
            }
            else
            {
                model.BackUrl = Request.GetTypedHeaders().Referer.ToString();
                var sessionValue = _sessionService.Get(SessionKeys.SelectedLocationOption);
                if ((!string.IsNullOrEmpty(sessionValue) &&
                    (Enum.TryParse<LocationOption>(sessionValue, out var locationOption)
                        && (locationOption == LocationOption.Both || locationOption == LocationOption.ProviderLocation))))
                {
                    model.BackUrl = Url.RouteUrl(RouteNames.GetLocationOption, new { ukprn = Ukprn, larsCode = model.LarsCode });
                }
                else
                {
                    model.BackUrl = GetStandardDetailsUrl(model.LarsCode);
                }
            }
            model.CancelUrl = GetStandardDetailsUrl(model.LarsCode);
            model.AddTrainingLocationUrl = Url.RouteUrl(RouteNames.GetAddProviderCourseLocation, new { ukprn = Ukprn, larsCode = model.LarsCode });
            return model;
        }

        [Route("{ukprn}/standards/{larsCode}/providerlocations", Name = RouteNames.PostProviderCourseLocations)]
        [HttpPost]
        public async Task <IActionResult> ConfirmedProviderCourseLocations(ProviderCourseLocationListViewModel model)
        {
            model = await BuildViewModel(model.LarsCode);
            var validator = new ProviderCourseLocationListViewModelValidator();
            var validatorResult = validator.Validate(model);
            if (!validatorResult.IsValid)
            {
                return View("~/Views/ProviderCourseLocations/EditTrainingLocations.cshtml", model);
            }

            var sessionValue = _sessionService.Get(SessionKeys.SelectedLocationOption);
            if ((!string.IsNullOrEmpty(sessionValue) &&
                (Enum.TryParse<LocationOption>(sessionValue, out var locationOption)
                    && (locationOption == LocationOption.Both))))
            {
                return RedirectToRoute(RouteNames.GetNationalDeliveryOption, new { Ukprn, model.LarsCode });
            }
            else
            {
                return RedirectToRoute(RouteNames.GetStandardDetails, new { Ukprn, model.LarsCode });
            }
        }
    }
}
