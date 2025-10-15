using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetProviderLocationDetails;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderLocations;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ViewProviderLocationDetailsControllerTests
{
    [TestFixture]
    public class ViewProviderLocationDetailsControllerTests
    {
        private const string Ukprn = "10012002";
        private Mock<IMediator> _mediatorMock;
        private ViewProviderLocationDetailsController _sut;
        string verifyUrl = "http://test";
        string verifyUpdateProviderLocationDetailsUrl = "http://test-UpdateProviderLocationDetailsUrl";

        [SetUp]
        public void Before_Each_Test()
        {
            _mediatorMock = new Mock<IMediator>();
            _sut = new ViewProviderLocationDetailsController(_mediatorMock.Object, Mock.Of<ILogger<ViewProviderLocationDetailsController>>());
            _sut.AddDefaultContextWithUser()
                .AddUrlHelperMock()
                .AddUrlForRoute(RouteNames.GetProviderLocations, verifyUrl)
                .AddUrlForRoute(RouteNames.GetUpdateProviderLocationDetails, verifyUpdateProviderLocationDetailsUrl);
        }

        [Test, AutoData]
        public async Task GetProviderLocationDetails_ValidRequest_ReturnsView(
            GetProviderLocationDetailsQueryResult queryResult,
            Guid id)
        {
            _mediatorMock
                .Setup(m => m.Send(It.Is<GetProviderLocationDetailsQuery>(q => q.Ukprn == int.Parse(Ukprn) && q.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResult);

            var result = await _sut.GetProviderLocationDetails(id);

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult!.ViewName.Should().Contain("ViewProviderLocationsDetails.cshtml");
            var model = viewResult.Model as ProviderLocationViewModel;
            model.Should().NotBeNull();
            model.UpdateContactDetailsUrl.Should().Be(verifyUpdateProviderLocationDetailsUrl);
        }

        [Test, AutoData]
        public async Task GetProviderLocationDetails_InvalidRequest_ReturnsPageNotFound(Guid id)
        {
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetProviderLocationDetailsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetProviderLocationDetailsQueryResult)null);

            var result = await _sut.GetProviderLocationDetails(id);

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult!.ViewName.Should().Be(ViewsPath.PageNotFoundPath);
        }
    }
}
