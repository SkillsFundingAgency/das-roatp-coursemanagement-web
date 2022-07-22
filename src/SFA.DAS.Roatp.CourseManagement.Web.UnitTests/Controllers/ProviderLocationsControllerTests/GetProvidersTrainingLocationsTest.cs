using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAllProviderLocations;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderLocations;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers
{
    [TestFixture]
    public class GetProvidersTrainingLocationsTest
    {
        private ProviderLocationsController _controller;
        private Mock<ILogger<ProviderLocationsController>> _logger;
        private Mock<IMediator> _mediator;
        private Mock<IUrlHelper> urlHelper;
        string verifyUrl = "http://test";

        [SetUp]
        public void Before_each_test()
        {
            _logger = new Mock<ILogger<ProviderLocationsController>>();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ProviderClaims.ProviderUkprn, "111"), }, "mock"));

            var response = new System.Collections.Generic.List<ProviderLocation>();

            var ProviderLocation1 = new ProviderLocation
            {
                ProviderLocationId = 1,
                LocationName = "test1",
                Postcode = "IG117WQ",
                Email = "test1@test.com",
                Phone="1234567891"
            };
            var ProviderLocation2 = new ProviderLocation
            {
                ProviderLocationId = 2,
                LocationName = "test2",
                Postcode = "IG117XR",
                Email = "test2@test.com",
                Phone = "1234567892"
            };
            response.Add(ProviderLocation1);
            response.Add(ProviderLocation2);

            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.Send(It.IsAny<GetAllProviderLocationsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new GetAllProviderLocationsQueryResult
                {
                    ProviderLocations = response
                });

            _controller = new ProviderLocationsController(_mediator.Object,  _logger.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext() { User = user },
                },
                TempData = Mock.Of<ITempDataDictionary>()
            };

            urlHelper = new Mock<IUrlHelper>();
            
            UrlRouteContext verifyRouteValues = null;
            urlHelper
               .Setup(m => m.RouteUrl(It.Is<UrlRouteContext>(c =>
                   c.RouteName.Equals(RouteNames.ReviewYourDetails)
               )))
               .Returns(verifyUrl)
               .Callback<UrlRouteContext>(c =>
               {
                   verifyRouteValues = c;
               });
            _controller.Url = urlHelper.Object;
        }

        [Test]
        public async Task GetProvidersTrainingLocation_ReturnsValidResponse()
        {
            var result = await _controller.GetProvidersTrainingLocation();

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.ViewName.Should().Contain("ViewProviderLocations.cshtml");
            var model = viewResult.Model as ProviderLocationListViewModel;
            model.Should().NotBeNull();
            model.ProviderLocations.Should().NotBeNull();
            model.BackUrl.Should().Be(verifyUrl);
        }

        [Test]
        public async Task GetProvidersTrainingLocation_ReturnsNoProviderLocationData()
        {
            _mediator.Setup(x => x.Send(It.IsAny<GetAllProviderLocationsQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(() => null);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]{ new Claim(ProviderClaims.ProviderUkprn,"111"),}, "mock"));
            
            _controller = new ProviderLocationsController(_mediator.Object, _logger.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext() { User = user },
                },
                TempData = Mock.Of<ITempDataDictionary>()
            };
            _controller.Url = urlHelper.Object;
            var result = await _controller.GetProvidersTrainingLocation();

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.ViewName.Should().Contain("ViewProviderLocations.cshtml");
            var model = viewResult.Model as ProviderLocationListViewModel;
            model.Should().NotBeNull();
            model.ProviderLocations.Should().BeEmpty();
            model.BackUrl.Should().Be(verifyUrl);
        }
    }
}
