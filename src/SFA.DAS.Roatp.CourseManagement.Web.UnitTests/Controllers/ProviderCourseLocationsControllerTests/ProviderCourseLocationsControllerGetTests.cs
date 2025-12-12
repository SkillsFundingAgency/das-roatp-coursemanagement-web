using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderCourseLocations;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ProviderCourseLocationsControllerTests
{
    [TestFixture]
    public class ProviderCourseLocationsControllerGetTests
    {
        private const string Ukprn = "10012002";
        private Mock<IMediator> _mediatorMock;
        private ProviderCourseLocationsController _sut;
        readonly string verifyUrlGetStandardDetails = "http://test-GetStandardDetails";

        readonly string verifyUrlGetLocationOption = "http://test-GetLocationOption";

        readonly string verifyRemoveProviderCourseLocationUrlGet = "http://test-RemoveProviderCourseLocation";

        readonly string verifyAddProviderCourseLocationUrlGet = "http://test-AddProviderCourseLocation";

        protected Mock<ISessionService> _sessionServiceMock;

        [SetUp]
        public void Before_Each_Test()
        {
            _mediatorMock = new Mock<IMediator>();
            _sessionServiceMock = new Mock<ISessionService>();

            _sut = new ProviderCourseLocationsController(_mediatorMock.Object, Mock.Of<ILogger<ProviderCourseLocationsController>>(), _sessionServiceMock.Object);

            _sut.AddDefaultContextWithUser()
               .AddUrlHelperMock()
               .AddUrlForRoute(RouteNames.GetStandardDetails, verifyUrlGetStandardDetails)
               .AddUrlForRoute(RouteNames.GetLocationOption, verifyUrlGetLocationOption)
               .AddUrlForRoute(RouteNames.GetRemoveProviderCourseLocation, verifyRemoveProviderCourseLocationUrlGet)
               .AddUrlForRoute(RouteNames.GetAddProviderCourseLocation, verifyAddProviderCourseLocationUrlGet);
        }

        [Test, AutoData]
        public async Task GetProviderCourseLocations_ValidRequest_ReturnsView(
            GetProviderCourseLocationsQueryResult queryResult,
            string larsCode)
        {
            _mediatorMock
                .Setup(m => m.Send(It.Is<GetProviderCourseLocationsQuery>(q => q.Ukprn == int.Parse(Ukprn) && q.LarsCode == larsCode), It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResult);

            var result = await _sut.GetProviderCourseLocations(larsCode);

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as ProviderCourseLocationListViewModel;
            model.Should().NotBeNull();
            model!.ProviderCourseLocations.Should().NotBeEmpty();
            model.AddTrainingLocationUrl.Should().Be(verifyAddProviderCourseLocationUrlGet);
        }

        [Test, AutoData]
        public async Task GetProviderCourseLocations_InvalidRequest_ReturnsEmptyResponse(string larsCode)
        {
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetProviderCourseLocationsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetProviderCourseLocationsQueryResult)null);

            var result = await _sut.GetProviderCourseLocations(larsCode);

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as ProviderCourseLocationListViewModel;
            model.Should().NotBeNull();
            model!.ProviderCourseLocations.Should().BeEmpty();
        }

        [Test, AutoData]
        public async Task GetProviderCourseLocations_validRequestLocationOptionBoth_ReturnGetLocationOption(string larsCode, GetProviderCourseLocationsQueryResult queryResult)
        {
            var refererUrl = "http://test-referer-url/";
            _sut.HttpContext.Request.Headers.Referer = refererUrl;


            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetProviderCourseLocationsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResult);

            _sessionServiceMock.Setup(s => s.Get(SessionKeys.SelectedLocationOption)).Returns(LocationOption.Both.ToString());

            var result = await _sut.GetProviderCourseLocations(larsCode);

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var modelResult = viewResult.Model as ProviderCourseLocationListViewModel;
            modelResult.Should().NotBeNull();
            modelResult!.ProviderCourseLocations.Should().NotBeEmpty();
        }

        [Test, AutoData]
        public async Task GetProviderCourseLocations_validRequestLocationOptionProviderLocation_ReturnsGetLocationOption(string larsCode, GetProviderCourseLocationsQueryResult queryResult)
        {
            var refererUrl = "http://test-referer-url/";
            _sut.HttpContext.Request.Headers.Referer = refererUrl;

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetProviderCourseLocationsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResult);

            _sessionServiceMock.Setup(s => s.Get(SessionKeys.SelectedLocationOption)).Returns(LocationOption.ProviderLocation.ToString());

            var result = await _sut.GetProviderCourseLocations(larsCode);

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var modelResult = viewResult!.Model as ProviderCourseLocationListViewModel;
            modelResult.Should().NotBeNull();
            modelResult!.ProviderCourseLocations.Should().NotBeEmpty();
        }

        [Test, AutoData]
        public async Task GetProviderCourseLocations_validRequest_ReturnsGetLocationOption(string larsCode, GetProviderCourseLocationsQueryResult queryResult)
        {
            var refererUrl = "http://test-referer-url/";
            _sut.HttpContext.Request.Headers.Referer = refererUrl;

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetProviderCourseLocationsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResult);

            var result = await _sut.GetProviderCourseLocations(larsCode);

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var modelResult = viewResult.Model as ProviderCourseLocationListViewModel;
            modelResult.Should().NotBeNull();
            modelResult!.ProviderCourseLocations.Should().NotBeEmpty();
        }

        [Test, AutoData]
        public async Task GetProviderCourseLocations_validRequest_ReturnLocationHaveRemoveUrl(string larsCode, GetProviderCourseLocationsQueryResult queryResult)
        {
            var refererUrl = "http://test-referer-url/";
            _sut.HttpContext.Request.Headers.Referer = refererUrl;

            queryResult = new GetProviderCourseLocationsQueryResult { ProviderCourseLocations = new System.Collections.Generic.List<Domain.ApiModels.ProviderCourseLocation> { new Domain.ApiModels.ProviderCourseLocation { LocationType = Domain.ApiModels.LocationType.Provider, Id = Guid.NewGuid() } } };
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetProviderCourseLocationsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResult);

            var result = await _sut.GetProviderCourseLocations(larsCode);

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var modelResult = viewResult!.Model as ProviderCourseLocationListViewModel;
            modelResult.Should().NotBeNull();
            modelResult!.ProviderCourseLocations.Should().NotBeEmpty();
            modelResult.ProviderCourseLocations.FirstOrDefault()!.RemoveUrl.Should().Be(verifyRemoveProviderCourseLocationUrlGet);
        }
    }
}
