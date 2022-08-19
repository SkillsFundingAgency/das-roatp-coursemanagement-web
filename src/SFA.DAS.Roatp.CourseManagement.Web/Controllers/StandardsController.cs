using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAllProviderStandards;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    [DasAuthorize(new[] { "ProviderFeature.CourseManagement" }, Policy = nameof(PolicyNames.HasProviderAccount) )]
    public class StandardsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<StandardsController> _logger;
        public StandardsController(IMediator mediator, ILogger<StandardsController> logger)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [Route("{ukprn}/standards", Name = RouteNames.ViewStandards)]
        [HttpGet]
        [ClearSession(nameof(StandardSessionModel))]
        public async Task<IActionResult> ViewStandards()
        {
            _logger.LogInformation("Getting standards for {ukprn}", Ukprn);

            var result = await _mediator.Send(new GetAllProviderStandardsQuery(Ukprn));

            var model = new StandardListViewModel
            {
                BackLink = Url.RouteUrl(RouteNames.ReviewYourDetails, new { Ukprn }),
                AddAStandardLink = Url.RouteUrl(RouteNames.GetAddStandardSelectStandard, new { Ukprn })
            };

            if (result == null)
            {
                _logger.LogInformation("Standards data not found for {ukprn}", Ukprn);
                return View("~/Views/Standards/ViewStandards.cshtml", model);
            }

            model.Standards = result.Standards.Select(c => (StandardViewModel)c).ToList();

            foreach (var standard in model.Standards)
            {
                standard.StandardUrl = Url.RouteUrl(RouteNames.GetStandardDetails, new {Ukprn, larsCode = standard.LarsCode});
                standard.ConfirmRegulatedStandardUrl = standard.IsApprovalPending ? Url.RouteUrl(RouteNames.GetConfirmRegulatedStandard, new { Ukprn, standard.LarsCode }) : string.Empty;
            }

            TempData.TryGetValue(TempDataKeys.DeleteProviderCourseDataKey, out var showBanner);
            if (showBanner!=null)
            {
                model.ShowNotificationBanner = true;
            }

            return View("~/Views/Standards/ViewStandards.cshtml", model);
        }

        [Route("{ukprn}/standards/{larsCode}", Name = RouteNames.GetStandardDetails)]
        [HttpGet]
        [ClearSession(SessionKeys.SelectedLocationOption)]
        public async Task<IActionResult> ViewStandard(int larsCode)
        {
            _logger.LogInformation("Getting Course details for ukprn {ukprn} LarsCode {larsCode}", Ukprn, larsCode);

            var result = await _mediator.Send(new GetStandardDetailsQuery(Ukprn, larsCode));

            if (result == null)
            {
                throw new InvalidOperationException();
            }

            var standardDetails = result;

            var model = (StandardDetailsViewModel)standardDetails;
            model.BackUrl = Url.RouteUrl(RouteNames.ViewStandards, new { ukprn = Ukprn });
            model.DeleteStandardUrl = Url.RouteUrl(RouteNames.GetConfirmDeleteStandard, new { ukprn = Ukprn, larsCode });
            
            model.EditContactDetailsUrl = Url.RouteUrl(RouteNames.GetCourseContactDetails, new { Ukprn, larsCode });
            model.EditLocationOptionUrl = Url.RouteUrl(RouteNames.GetLocationOption, new { Ukprn, larsCode });
            model.EditTrainingLocationsUrl = Url.RouteUrl(RouteNames.GetProviderCourseLocations, new { Ukprn, larsCode });

            model.ConfirmRegulatedStandardUrl = model.StandardInformation.IsStandardRegulated ? Url.RouteUrl(RouteNames.GetConfirmRegulatedStandard, new { Ukprn, larsCode }) : string.Empty;

            model.EditProviderCourseRegionsUrl = model.SubRegionCourseLocations.Any() ? Url.RouteUrl(RouteNames.GetStandardSubRegions, new { Ukprn, larsCode }) : string.Empty;

            model.EditTrainingLocationsUrl = Url.RouteUrl(RouteNames.GetProviderCourseLocations, new { Ukprn, larsCode });
            return View("~/Views/Standards/ViewStandardDetails.cshtml", model);
        }
    }
}
