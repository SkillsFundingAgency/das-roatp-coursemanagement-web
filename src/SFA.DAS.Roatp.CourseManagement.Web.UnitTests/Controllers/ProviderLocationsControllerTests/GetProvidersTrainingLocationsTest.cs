using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
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
        private Mock<IMediator> _mediatorMock;
        private Mock<IUrlHelper> _urlHelperMock;
        const string BackUrl = "http://test";
        string verifyVenueNameUrl = "http://test-VenueNameUrl";
        const string AddTrainingLocationUrl = "www.abc.com";

        [SetUp]
        public void Before_each_test()
        {
            _logger = new Mock<ILogger<ProviderLocationsController>>();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ProviderClaims.ProviderUkprn, "111"), }, "mock"));

            var response = new System.Collections.Generic.List<ProviderLocation>();

            var ProviderLocation1 = new ProviderLocation
            {
                NavigationId = System.Guid.NewGuid(),
                LocationName = "test1",
                Postcode = "IG117WQ",
                Email = "test1@test.com",
                Phone="1234567891"
            };
            var ProviderLocation2 = new ProviderLocation
            {
                NavigationId = System.Guid.NewGuid(),
                LocationName = "test2",
                Postcode = "IG117XR",
                Email = "test2@test.com",
                Phone = "1234567892"
            };
            response.Add(ProviderLocation1);
            response.Add(ProviderLocation2);

            _mediatorMock = new Mock<IMediator>();
            _mediatorMock.Setup(x => x.Send(It.IsAny<GetAllProviderLocationsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new GetAllProviderLocationsQueryResult
                {
                    ProviderLocations = response
                });

            _controller = new ProviderLocationsController(_mediatorMock.Object,  _logger.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext() { User = user },
                },
            };

            _urlHelperMock = new Mock<IUrlHelper>();
            
            _urlHelperMock
               .Setup(m => m.RouteUrl(It.Is<UrlRouteContext>(c =>
                   c.RouteName.Equals(RouteNames.ReviewYourDetails)
               )))
               .Returns(BackUrl);

            _urlHelperMock
               .Setup(m => m.RouteUrl(It.Is<UrlRouteContext>(c =>
                   c.RouteName.Equals(RouteNames.GetTrainingLocationPostcode)
               )))
               .Returns(AddTrainingLocationUrl);
               
            _urlHelperMock
               .Setup(m => m.RouteUrl(It.Is<UrlRouteContext>(c =>
                   c.RouteName.Equals(RouteNames.GetProviderLocationDetails)
              )))
              .Returns(verifyVenueNameUrl)
              .Callback<UrlRouteContext>(c =>
              {
                  verifyRouteValues = c;
              });

            _controller.Url = _urlHelperMock.Object;
        }

        [Test]
        public async Task GetProvidersTrainingLocation_ReturnsValidResponse()
        {
            var result = await _controller.GetProvidersTrainingLocations();

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.ViewName.Should().Contain("ViewProviderLocations.cshtml");
            var model = viewResult.Model as ProviderLocationListViewModel;
            model.Should().NotBeNull();
            model.ProviderLocations.Should().NotBeNull();
            model.BackUrl.Should().Be(BackUrl);
            model.AddTrainingLocationLink.Should().Be(AddTrainingLocationUrl);
        }

        [Test]
        public async Task GetProvidersTrainingLocation_ReturnsNoProviderLocationData()
        {
            _mediatorMock.Setup(x => x.Send(It.IsAny<GetAllProviderLocationsQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(() => null);

            var result = await _controller.GetProvidersTrainingLocations();

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.ViewName.Should().Contain("ViewProviderLocations.cshtml");
            var model = viewResult.Model as ProviderLocationListViewModel;
            model.Should().NotBeNull();
            model.ProviderLocations.Should().BeEmpty();
            model.BackUrl.Should().Be(BackUrl);
        }

        [Test]
        public async Task GetProvidersTrainingLocation_ReturnsProviderLocationsDataVenueNameUrl()
        {
            var result = await _controller.GetProvidersTrainingLocation();

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as ProviderLocationListViewModel;
            model.Should().NotBeNull();
            model.ProviderLocations.Should().NotBeEmpty();
            foreach (var location in model.ProviderLocations)
            {
                location.VenueNameUrl.Should().Be(verifyVenueNameUrl);
            }
        }
    }
}
