using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.UpdateContactDetails;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    [DasAuthorize(new[] { "ProviderFeature.CourseManagement" }, Policy = nameof(PolicyNames.HasProviderAccount) )]
    public class EditCourseContactDetailsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<EditCourseContactDetailsController> _logger;

        public EditCourseContactDetailsController(IMediator mediator, ILogger<EditCourseContactDetailsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [Route("{ukprn}/standards/{larsCode}/edit-contact-details", Name = RouteNames.GetCourseContactDetails)]
        [HttpGet]
        public async Task<IActionResult> Index([FromRoute] int larsCode)
        {
            EditCourseContactDetailsViewModel model = await GetViewModel(larsCode);

            return View(model);
        }

        [Route("{ukprn}/standards/{larscode}/edit-contact-details", Name = RouteNames.PostCourseContactDetails)]
        [HttpPost]
        public async Task<IActionResult> Index([FromRoute] int larsCode, CourseContactDetailsSubmitModel submitModel)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = await GetViewModel(larsCode);
                return View(viewModel);
            }

            var command = (UpdateProviderCourseContactDetailsCommand)submitModel;
            command.LarsCode = larsCode;
            command.Ukprn = Ukprn;
            command.UserId = UserId;

            await _mediator.Send(command);

            return RedirectToRoute(RouteNames.GetStandardDetails, new {Ukprn, larsCode });
        }

        private async Task<EditCourseContactDetailsViewModel> GetViewModel(int larsCode)
        {
            var result = await _mediator.Send(new GetStandardDetailsQuery(Ukprn, larsCode));

            if (result == null)
            {
                _logger.LogError("Standard details not found for ukprn {ukprn} and larscode {larsCode}", Ukprn, larsCode);
                throw new InvalidOperationException($"Standard details not found for ukprn {Ukprn} and larscode {larsCode}");
            }

            var model = (EditCourseContactDetailsViewModel)result;

            model.BackLink = model.CancelLink = GetStandardDetailsUrl(larsCode);
            return model;
        }
    }
}
