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
    public class ProviderCourseLocationsControllerPostTests
    {
        private const string Ukprn = "10012002";
        private Mock<IMediator> _mediatorMock;
        private ProviderCourseLocationsController _sut;
        readonly string verifyUrl = "http://test";
        protected Mock<ISessionService> _sessionServiceMock;

        [SetUp]
        public void Before_Each_Test()
        {
            _mediatorMock = new Mock<IMediator>();
            _sessionServiceMock = new Mock<ISessionService>();

            _sut = new ProviderCourseLocationsController(_mediatorMock.Object, Mock.Of<ILogger<ProviderCourseLocationsController>>(), _sessionServiceMock.Object);

            _sut.AddDefaultContextWithUser()
               .AddUrlHelperMock()
               .AddUrlForRoute(RouteNames.GetStandardDetails, verifyUrl);

        }

        [Test, AutoData]
        public async Task Post_ValidModel_RedirectToRoute(ProviderCourseLocationListViewModel model, GetProviderCourseLocationsQueryResult queryResult)
        {
            _mediatorMock
               .Setup(m => m.Send(It.Is<GetProviderCourseLocationsQuery>(q => q.Ukprn == int.Parse(Ukprn) && q.LarsCode == model.LarsCode), It.IsAny<CancellationToken>()))
               .ReturnsAsync(queryResult);

            var result = await _sut.ConfirmedProviderCourseLocations(model);
            var redirectResult = result as RedirectToRouteResult;
            redirectResult.Should().NotBeNull();
            redirectResult.RouteName.Should().Be(RouteNames.GetStandardDetails);
            redirectResult.RouteValues["ukprn"].Should().Be(int.Parse(Ukprn));
        }

        [Test, AutoData]
        public async Task Post_InValidModel_ReturnsSameView(ProviderCourseLocationListViewModel model, GetProviderCourseLocationsQueryResult queryResult)
        {
            var refererUrl = "http://test-referer-url/";
            _sut.HttpContext.Request.Headers.Referer = refererUrl;

            queryResult.ProviderCourseLocations = new System.Collections.Generic.List<Domain.ApiModels.ProviderCourseLocation>();

            _mediatorMock
           .Setup(m => m.Send(It.Is<GetProviderCourseLocationsQuery>(q => q.Ukprn == int.Parse(Ukprn) && q.LarsCode == model.LarsCode), It.IsAny<CancellationToken>()))
           .ReturnsAsync(queryResult);


            var result = await _sut.ConfirmedProviderCourseLocations(model);

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.ViewName.Should().Contain("EditTrainingLocations.cshtml");
            viewResult.Model.Should().NotBeNull();
            var modelResult = viewResult.Model as ProviderCourseLocationListViewModel;
            modelResult.Should().NotBeNull();
            modelResult.BackUrl.Should().Be(verifyUrl);
            modelResult.CancelUrl.Should().Be(verifyUrl);
        }

        [Test, AutoData]
        public async Task Post_ValidModel_RedirectToNationalDeliveryOption(ProviderCourseLocationListViewModel model, GetProviderCourseLocationsQueryResult queryResult)
        {
            var refererUrl = "http://test-referer-url/";
            _sut.HttpContext.Request.Headers.Referer = refererUrl;

            _mediatorMock
           .Setup(m => m.Send(It.Is<GetProviderCourseLocationsQuery>(q => q.Ukprn == int.Parse(Ukprn) && q.LarsCode == model.LarsCode), It.IsAny<CancellationToken>()))
           .ReturnsAsync(queryResult);

            _sessionServiceMock.Setup(s => s.Get(SessionKeys.SelectedLocationOption)).Returns(LocationOption.Both.ToString());
            var result = await _sut.ConfirmedProviderCourseLocations(model);

            var redirectResult = result as RedirectToRouteResult;

            redirectResult.Should().NotBeNull();
            redirectResult.RouteName.Should().Be(RouteNames.GetNationalDeliveryOption);
            redirectResult.RouteValues["ukprn"].Should().Be(int.Parse(Ukprn));
        }
    }
}
