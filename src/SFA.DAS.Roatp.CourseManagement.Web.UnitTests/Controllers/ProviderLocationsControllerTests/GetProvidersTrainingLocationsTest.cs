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
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ProviderLocationsControllerTests
{
    [TestFixture]
    public class GetProvidersTrainingLocationsTest
    {
        private ProviderLocationsController _controller;
        private Mock<ILogger<ProviderLocationsController>> _logger;
        private Mock<IMediator> _mediatorMock;
        private Mock<IUrlHelper> _urlHelperMock;
        const string BackUrl = "http://test";
        readonly string _verifyVenueNameUrl = "http://test-VenueNameUrl";
        readonly string _viewStandardsLink = Guid.NewGuid().ToString();
        const string AddTrainingLocationUrl = "www.abc.com";
        private List<ProviderLocationViewModel> AlphabeticallyOrderedList;

        [SetUp]
        public void Before_each_test()
        {
            _logger = new Mock<ILogger<ProviderLocationsController>>();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ProviderClaims.ProviderUkprn, "111"), }, "mock"));

            var response = new List<ProviderLocation>();

            var providerLocation1 = new ProviderLocation
            {
                NavigationId = System.Guid.NewGuid(),
                LocationName = "zz 1",
                Postcode = "IG117WQ"
            };
            var providerLocation2 = new ProviderLocation
            {
                NavigationId = System.Guid.NewGuid(),
                LocationName = "aa 2",
                Postcode = "IG117XR"
            };

            var providerLocation3 = new ProviderLocation
            {
                NavigationId = System.Guid.NewGuid(),
                LocationName = "mm 3",
                Postcode = "IG117XR"
            };

            response.Add(providerLocation1);
            response.Add(providerLocation2);
            response.Add(providerLocation3);


            AlphabeticallyOrderedList = new List<ProviderLocationViewModel> { providerLocation2, providerLocation3, providerLocation1 };

            _mediatorMock = new Mock<IMediator>();
            _mediatorMock.Setup(x => x.Send(It.IsAny<GetAllProviderLocationsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new GetAllProviderLocationsQueryResult
                {
                    ProviderLocations = response
                });

            _controller = new ProviderLocationsController(_mediatorMock.Object, _logger.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext() { User = user },
                },
            };

            _urlHelperMock = new Mock<IUrlHelper>();

            _urlHelperMock
               .Setup(m => m.RouteUrl(It.Is<UrlRouteContext>(c => c.RouteName.Equals(RouteNames.ReviewYourDetails))))
               .Returns(BackUrl);

            _urlHelperMock
               .Setup(m => m.RouteUrl(It.Is<UrlRouteContext>(c => c.RouteName.Equals(RouteNames.GetTrainingLocationPostcode))))
               .Returns(AddTrainingLocationUrl);

            _urlHelperMock
               .Setup(m => m.RouteUrl(It.Is<UrlRouteContext>(c => c.RouteName.Equals(RouteNames.GetProviderLocationDetails))))
              .Returns(_verifyVenueNameUrl);

            _urlHelperMock
                .Setup(m => m.RouteUrl(It.Is<UrlRouteContext>(c => c.RouteName.Equals(RouteNames.ViewStandards))))
                .Returns(_viewStandardsLink);


            _controller.Url = _urlHelperMock.Object;


            var tempDataMock = new Mock<ITempDataDictionary>();
            _controller.TempData = tempDataMock.Object;
        }

        [Test]
        public async Task GetProvidersTrainingLocation_ReturnsValidResponse()
        {
            var result = await _controller.GetProvidersTrainingLocations();

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult!.ViewName.Should().Contain("ViewProviderLocations.cshtml");
            var model = viewResult.Model as ProviderLocationListViewModel;
            model.Should().NotBeNull();
            model!.ProviderLocations.Should().NotBeNull();
            model.ProviderLocations.Count.Should().Be(3);
            model.BackUrl.Should().Be(BackUrl);
            model.AddTrainingLocationLink.Should().Be(AddTrainingLocationUrl);
            model.ShowNotificationBannerAddVenue.Should().Be(false);
            model.ManageYourStandardsUrl.Should().BeNull();
        }

        [Test]
        public async Task GetProvidersTrainingLocation_ReturnsNoProviderLocationData()
        {
            _mediatorMock.Setup(x => x.Send(It.IsAny<GetAllProviderLocationsQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(() => null);

            var result = await _controller.GetProvidersTrainingLocations();

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult!.ViewName.Should().Contain("ViewProviderLocations.cshtml");
            var model = viewResult.Model as ProviderLocationListViewModel;
            model.Should().NotBeNull();
            model!.ProviderLocations.Should().BeEmpty();
            model.BackUrl.Should().Be(BackUrl);
        }

        [Test]
        public async Task GetProvidersTrainingLocation_ReturnsProviderLocationsDataVenueNameUrl()
        {
            var result = await _controller.GetProvidersTrainingLocations();

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult!.Model as ProviderLocationListViewModel;
            model.Should().NotBeNull();
            model!.ProviderLocations.Should().NotBeEmpty();
            foreach (var location in model.ProviderLocations)
            {
                location.VenueNameUrl.Should().Be(_verifyVenueNameUrl);
            }
        }

        [Test]
        public async Task GetProvidersTrainingLocation_ReturnsAlphabeticallyOrderedLocations()
        {
            var result = await _controller.GetProvidersTrainingLocations();

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult!.Model as ProviderLocationListViewModel;
            model.Should().NotBeNull();
            model!.ProviderLocations.Should().NotBeEmpty();
            model.ProviderLocations.Should().BeEquivalentTo(AlphabeticallyOrderedList, option => option
                .Excluding(c => c.VenueNameUrl));
        }


        [Test]
        public async Task GetProvidersTrainingLocation_SetsUpAddBanner()
        {
            var tempDataMock = new Mock<ITempDataDictionary>();
            object isVenueAdded = true;
            tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.ShowVenueAddBannerTempDataKey, out isVenueAdded));


            _controller.TempData = tempDataMock.Object;

            var result = await _controller.GetProvidersTrainingLocations();

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult!.ViewName.Should().Contain("ViewProviderLocations.cshtml");
            var model = viewResult.Model as ProviderLocationListViewModel;
            model.Should().NotBeNull();

            model!.ShowNotificationBannerAddVenue.Should().Be(true);
            model.ManageYourStandardsUrl.Should().Be(_viewStandardsLink);
        }
    }
}
