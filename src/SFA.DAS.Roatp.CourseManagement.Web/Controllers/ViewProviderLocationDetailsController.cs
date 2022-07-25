using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    [DasAuthorize(new[] { "ProviderFeature.CourseManagement" }, Policy = nameof(PolicyNames.HasProviderAccount) )]
    public class ViewProviderLocationDetailsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ViewProviderLocationDetailsController> _logger;
        public ViewProviderLocationDetailsController(IMediator mediator, ILogger<ViewProviderLocationDetailsController> logger)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [Route("/providers/{ukprn}/locations/{Id}", Name = RouteNames.GetProviderLocationDetails)]
        [HttpGet]
        public void GetProviderLocationDetails([FromRoute] int Id)
        {
            _logger.LogInformation("Getting Provider Location for {ukprn}, {Id}", Ukprn, Id);
        }
    }
}
