using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAllProviderStandards;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAStandard
{
    [DasAuthorize(new[] { "ProviderFeature.CourseManagement" }, Policy = nameof(PolicyNames.HasProviderAccount))]

    public class ViewStandardController:ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ViewStandardController> _logger;
        public ViewStandardController(IMediator mediator, ILogger<ViewStandardController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [Route("{ukprn}/standards/{larsCode}/view-details-to-add", Name = RouteNames.GetAddedStandardDetails)]
        [HttpGet]
        public async Task<IActionResult> ViewStandardToAdd(int ukprn, int larsCode)
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
                standard.StandardUrl = Url.RouteUrl(RouteNames.GetStandardDetails, new { Ukprn, larsCode = standard.LarsCode });
                standard.ConfirmRegulatedStandardUrl = standard.IsApprovalPending ? Url.RouteUrl(RouteNames.GetConfirmRegulatedStandard, new { Ukprn, standard.LarsCode }) : string.Empty;
            }

            return View("~/Views/AddAStandard/ViewAddedStandardDetails.cshtml", model);
        }
    }
}
