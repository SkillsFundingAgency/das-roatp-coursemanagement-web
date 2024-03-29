﻿using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.Providers.Queries;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderDescription;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    [Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
    public class ProviderController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProviderController> _logger;

        public ProviderController(IMediator mediator, ILogger<ProviderController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [Route("{ukprn}/provider-description", Name = RouteNames.GetProviderDescription)]
        [HttpGet]
        public async Task<IActionResult> ViewProviderDescription(int ukprn)
        {
            _logger.LogInformation("Provider data gathering for {ukprn}", Ukprn);
            var result = await _mediator.Send(new GetProviderQuery(ukprn));

            var model = new ProviderDescriptionViewModel
            {
                BackUrl = Url.RouteUrl(RouteNames.ReviewYourDetails, new { ukprn }),
                Description = result.Provider.MarketingInfo
            };

            return View("~/Views/ProviderDescription/Index.cshtml", model);
        }
    }
}