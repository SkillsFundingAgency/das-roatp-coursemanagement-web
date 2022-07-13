using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderCourseLocations;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using SFA.DAS.Roatp.CourseManagement.Web.Validators;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    [DasAuthorize(new[] { "ProviderFeature.CourseManagement" }, Policy = nameof(PolicyNames.HasProviderAccount))]
    public class ProviderCourseLocationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProviderCourseLocationController> _logger;

        public ProviderCourseLocationController(IMediator mediator, ILogger<ProviderCourseLocationController> logger)
        {
            _mediator = mediator;
            _logger = logger;
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
                throw new InvalidOperationException(message);
            }

            var model = new ProviderCourseLocationListViewModel
            {
                ProviderCourseLocations = result.ProviderCourseLocations.Select(x => (ProviderCourseLocationViewModel)x).ToList(),
                LarsCode = larsCode
            };
            model.BackUrl = model.CancelUrl = GetStandardDetailsUrl(model.LarsCode);
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

            return RedirectToRoute(RouteNames.ViewStandardDetails, new { Ukprn, model.LarsCode });
        }
    }
}
