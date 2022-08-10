using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderCourseLocations;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ProviderCourseLocationsControllerTests
{
    [TestFixture]
    public class ProviderCourseLocationsControllerGetTests
    {
        private const string Ukprn = "10012002";
        private Mock<ILogger<ProviderCourseLocationsController>> _loggerMock;
        private Mock<IMediator> _mediatorMock;
        private ProviderCourseLocationsController _sut;
        private Mock<IUrlHelper> urlHelper;
        string verifyUrlGetStandardDetails = "http://test-GetStandardDetails";

        string verifyUrlGetLocationOption = "http://test-GetLocationOption";

        string verifyRemoveProviderCourseLocationUrlGet = "http://test-RemoveProviderCourseLocation";

        protected Mock<ISessionService> _sessionServiceMock;

        [SetUp]
        public void Before_Each_Test()
        {
            _loggerMock = new Mock<ILogger<ProviderCourseLocationsController>>();
            _mediatorMock = new Mock<IMediator>();
            _sessionServiceMock = new Mock<ISessionService>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ProviderClaims.ProviderUkprn, Ukprn), }, "mock"));
            var httpContext = new DefaultHttpContext() { User = user };
            _sut = new ProviderCourseLocationsController(_mediatorMock.Object, _loggerMock.Object, _sessionServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };

            urlHelper = new Mock<IUrlHelper>();

            UrlRouteContext verifyRouteValues = null;
            urlHelper
               .Setup(m => m.RouteUrl(It.Is<UrlRouteContext>(c =>
                   c.RouteName.Equals(RouteNames.GetStandardDetails)
               )))
               .Returns(verifyUrlGetStandardDetails)
               .Callback<UrlRouteContext>(c =>
               {
                   verifyRouteValues = c;
               });

            urlHelper
               .Setup(m => m.RouteUrl(It.Is<UrlRouteContext>(c =>
                   c.RouteName.Equals(RouteNames.GetLocationOption)
               )))
               .Returns(verifyUrlGetLocationOption)
               .Callback<UrlRouteContext>(c =>
               {
                   verifyRouteValues = c;
               });

            urlHelper
                .Setup(m => m.RouteUrl(It.Is<UrlRouteContext>(c =>
                    c.RouteName.Equals(RouteNames.GetRemoveProviderCourseLocation)
                )))
                .Returns(verifyRemoveProviderCourseLocationUrlGet)
                .Callback<UrlRouteContext>(c =>
                {
                    verifyRouteValues = c;
                });

            _sut.Url = urlHelper.Object;
        }

        [Test, AutoData]
        public async Task GetProviderCourseLocations_ValidRequest_ReturnsView(
            GetProviderCourseLocationsQueryResult queryResult,
            int larsCode)
        {
            _mediatorMock
                .Setup(m => m.Send(It.Is<GetProviderCourseLocationsQuery>(q => q.Ukprn == int.Parse(Ukprn) && q.LarsCode == larsCode), It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResult);

            var result = await _sut.GetProviderCourseLocations(larsCode);

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as ProviderCourseLocationListViewModel;
            model.Should().NotBeNull();
            model.ProviderCourseLocations.Should().NotBeEmpty();
            model.BackUrl.Should().NotBeNull();
            model.CancelUrl.Should().NotBeNull();
        }

          [Test, AutoData]
        public async Task GetProviderCourseLocations_InvalidRequest_ReturnsEmptyResponse(int larsCode)
        {
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetProviderCourseLocationsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetProviderCourseLocationsQueryResult)null);

            var result = await _sut.GetProviderCourseLocations(larsCode);

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as ProviderCourseLocationListViewModel;
            model.Should().NotBeNull();
            model.ProviderCourseLocations.Should().BeEmpty();
            model.BackUrl.Should().NotBeNull();
            model.CancelUrl.Should().NotBeNull();
        }

        [Test, AutoData]
        public async Task GetProviderCourseLocations_validRequestLocationOptionBoth_ReturnsBackUrlGetLocationOption(int larsCode, ProviderCourseLocationListViewModel model, GetProviderCourseLocationsQueryResult queryResult)
        {
            var refererUrl = "http://test-referer-url/";
            _sut.HttpContext.Request.Headers.Add("Referer", refererUrl);


            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetProviderCourseLocationsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResult);

            _sessionServiceMock.Setup(s => s.Get(SessionKeys.SelectedLocationOption, larsCode.ToString())).Returns(LocationOption.Both.ToString());

            var result = await _sut.GetProviderCourseLocations(larsCode);

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var modelResult = viewResult.Model as ProviderCourseLocationListViewModel;
            modelResult.Should().NotBeNull();
            modelResult.ProviderCourseLocations.Should().NotBeEmpty();
            modelResult.BackUrl.Should().Be(verifyUrlGetLocationOption);
        }

        [Test, AutoData]
        public async Task GetProviderCourseLocations_validRequestLocationOptionProviderLocation_ReturnsBackUrlGetLocationOption(int larsCode, ProviderCourseLocationListViewModel model, GetProviderCourseLocationsQueryResult queryResult)
        {
            var refererUrl = "http://test-referer-url/";
            _sut.HttpContext.Request.Headers.Add("Referer", refererUrl);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetProviderCourseLocationsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResult);

            _sessionServiceMock.Setup(s => s.Get(SessionKeys.SelectedLocationOption, larsCode.ToString())).Returns(LocationOption.ProviderLocation.ToString());

            var result = await _sut.GetProviderCourseLocations(larsCode);

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var modelResult = viewResult.Model as ProviderCourseLocationListViewModel;
            modelResult.Should().NotBeNull();
            modelResult.ProviderCourseLocations.Should().NotBeEmpty();
            modelResult.BackUrl.Should().Be(verifyUrlGetLocationOption);
        }

        [Test, AutoData]
        public async Task GetProviderCourseLocations_validRequest_ReturnsBackUrlGetLocationOption(int larsCode, GetProviderCourseLocationsQueryResult queryResult)
        {
            var refererUrl = "http://test-referer-url/";
            _sut.HttpContext.Request.Headers.Add("Referer", refererUrl);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetProviderCourseLocationsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResult);

            var result = await _sut.GetProviderCourseLocations(larsCode);

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var modelResult = viewResult.Model as ProviderCourseLocationListViewModel;
            modelResult.Should().NotBeNull();
            modelResult.ProviderCourseLocations.Should().NotBeEmpty();
            modelResult.BackUrl.Should().Be(verifyUrlGetStandardDetails);
        }

        [Test, AutoData]
        public async Task GetProviderCourseLocations_validRequest_ReturnLocationHaveRemoveUrl(int larsCode, GetProviderCourseLocationsQueryResult queryResult)
        {
            var refererUrl = "http://test-referer-url/";
            _sut.HttpContext.Request.Headers.Add("Referer", refererUrl);

            queryResult = new GetProviderCourseLocationsQueryResult { ProviderCourseLocations = new System.Collections.Generic.List<Domain.ApiModels.ProviderCourseLocation> { new Domain.ApiModels.ProviderCourseLocation { LocationType = Domain.ApiModels.LocationType.Provider, Id = 1 } } };
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetProviderCourseLocationsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResult);

            var result = await _sut.GetProviderCourseLocations(larsCode);

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var modelResult = viewResult.Model as ProviderCourseLocationListViewModel;
            modelResult.Should().NotBeNull();
            modelResult.ProviderCourseLocations.Should().NotBeEmpty();
            modelResult.ProviderCourseLocations.FirstOrDefault().RemoveUrl.Should().Be(verifyRemoveProviderCourseLocationUrlGet);
        }
    }
}
