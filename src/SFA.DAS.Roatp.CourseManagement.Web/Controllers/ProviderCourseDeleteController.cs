using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Commands.DeleteCourseLocations;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Queries.GetStandardInformation;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;


namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    [Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
    public class ProviderCourseDeleteController : ControllerBase
    {
        public const string ViewPath = "~/Views/Standards/ConfirmDeleteStandard.cshtml";
        private readonly IMediator _mediator;
        private readonly ILogger<ProviderCourseDeleteController> _logger;

        public ProviderCourseDeleteController(IMediator mediator, ILogger<ProviderCourseDeleteController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [Route("{ukprn}/standards/{larsCode}/delete-standard", Name = RouteNames.GetConfirmDeleteStandard)]
        [HttpGet]
        public async Task<IActionResult> GetProviderCourse(string larsCode)
        {
            _logger.LogInformation("Getting Standard information for ukprn {ukprn} LarsCode {larsCode}", Ukprn, larsCode);

            var result = await _mediator.Send(new GetStandardInformationQuery(larsCode));

            if (result == null)
            {
                var message = $"Standard Standard information found for larscode {larsCode}";
                _logger.LogError(message);
                throw new InvalidOperationException(message);
            }

            var standardResult = await _mediator.Send(new GetStandardDetailsQuery(Ukprn, larsCode));

            if (standardResult == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

            var model = (ConfirmDeleteStandardViewModel)result;

            return View(ViewPath, model);
        }

        [Route("{ukprn}/standards/{larsCode}/delete-standard", Name = RouteNames.PostConfirmDeleteStandard)]
        [HttpPost]
        public async Task<IActionResult> DeleteProviderCourse(ConfirmDeleteStandardViewModel model)
        {
            var command = new DeleteProviderCourseCommand(Ukprn, model.StandardInformation.LarsCode, UserId, UserDisplayName);
            await _mediator.Send(command);
            TempData.Add(TempDataKeys.ShowStandardDeletedBannerTempDataKey, true);

            return RedirectToRouteWithUkprn(RouteNames.ViewStandards);
        }
    }
}
