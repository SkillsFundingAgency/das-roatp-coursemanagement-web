using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderDescription;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    [DasAuthorize(new[] { "ProviderFeature.CourseManagement" }, Policy = nameof(PolicyNames.HasProviderAccount))]
    public class ProviderController : ControllerBase
    {
        private readonly ILogger<ProviderController> _logger;

        public ProviderController(ILogger<ProviderController> logger)
        {
            _logger = logger;
        }

        [Route("{ukprn}/provider-description", Name = RouteNames.ProviderDescription)]
        [HttpGet]
        public IActionResult ViewStandardsAndVenues(int ukprn)
        {

            var productDescription = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Proin imperdiet felis vel nibh vestibulum auctor. Curabitur ut commodo turpis. Duis a magna auctor, fermentum justo vel, euismod ipsum. Curabitur tempor magna eget odio lobortis dapibus. Phasellus eu tortor velit. Integer sit amet turpis volutpat, scelerisque metus ac, commodo ex. Sed suscipit ultricies pulvinar. Nunc ultricies est vitae rutrum volutpat. Sed bibendum ex ac pulvinar suscipit. Duis ultricies a risus dictum posuere. Donec mollis nisi at augue vulputate ullamcorper. Quisque non nibh felis. Aliquam laoreet lacus id mauris accumsan finibus. Nam sed facilisis orci, eu euismod ipsum. Nam luctus, sem vel imperdiet venenatis, justo massa congue mi, sit amet dignissim augue nisl quis purus. Sed est diam, vulputate sed urna molestie, volutpat molestie felis. Nullam vel orci ut ligula tincidunt accumsan ut finibus sem. Quisque et convallis risus. Quisque ultricies nisi ex, sit amet elementum augue ultrices id.Morbi at sodales diam. Sed lacinia ligula ut justo eleifend congue.Sed commodo erat dui, ut lacinia lacus maximus in. Morbi eu vulputate sapien, sit amet pharetra metus.Mauris sit amet dictum dolor.";
            _logger.LogInformation("Showing provider description for {ukprn}", Ukprn);

            var model = new ProviderDescriptionViewModel
            {
                BackUrl = Url.RouteUrl(RouteNames.StandardsAndVenuesDetails, new { ukprn }),
                ProductDescription = productDescription
            };

            return View("~/Views/ProviderDescription/Index.cshtml", model);
        }
    }
}