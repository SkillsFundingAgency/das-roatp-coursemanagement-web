using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAllProviderLocations;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAvailableProviderLocations;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.AddProviderCourseLocation;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderCourseLocations;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ProviderCourseLocationAddControllerTests
{
    [TestFixture]
    public class ProviderCourseLocationAddPostControllerTests
    {
        private const string Ukprn = "10012002";
        private Mock<IMediator> _mediatorMock;
        private ProviderCourseLocationAddController _sut;
        string verifyUrl = "http://test";

        [SetUp]
        public void Before_Each_Test()
        {
            _mediatorMock = new Mock<IMediator>();
            _sut = new ProviderCourseLocationAddController(Mock.Of<ILogger<ProviderCourseLocationAddController>>(), _mediatorMock.Object);
            _sut.AddDefaultContextWithUser()
                .AddUrlHelperMock()
                .AddUrlForRoute(RouteNames.GetProviderCourseLocations, verifyUrl);
        }

        [Test, AutoData]
        public async Task Post_ValidModel_RedirectToRoute(GetAllProviderLocationsQueryResult resultAllProviderLocations,
            GetProviderCourseLocationsQueryResult resultProviderCourseLocations, int larsCode, ProviderCourseLocationAddSubmitModel model)
        {
            resultAllProviderLocations.ProviderLocations.First().NavigationId = Guid.NewGuid();
            _mediatorMock
               .Setup(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn == int.Parse(Ukprn)), It.IsAny<CancellationToken>()))
               .ReturnsAsync(resultAllProviderLocations);

            _mediatorMock
                .Setup(m => m.Send(It.Is<GetProviderCourseLocationsQuery>(q => q.Ukprn == int.Parse(Ukprn) && q.LarsCode == larsCode), It.IsAny<CancellationToken>()))
                .ReturnsAsync(resultProviderCourseLocations);

            model.TrainingVenueNavigationId = resultAllProviderLocations.ProviderLocations.First().NavigationId.ToString();
            var result =  await _sut.SubmitAProviderlocation(larsCode, model);

            _mediatorMock.Verify(m => m.Send(It.IsAny<AddProviderCourseLocationCommand>(), It.IsAny<CancellationToken>()), Times.Once);

            var actual = (RedirectToRouteResult)result;
            Assert.NotNull(actual);
            actual.RouteName.Should().Be(RouteNames.GetProviderCourseLocations);
        }

        [Test, AutoData]
        public async Task Post_InValidModel_ReturnsSameView(GetAvailableProviderLocationsQueryResult availableProviderLocationsQueryResult, int larsCode, ProviderCourseLocationAddSubmitModel model)
        {
            _mediatorMock
            .Setup(m => m.Send(It.Is<GetAvailableProviderLocationsQuery>(q => q.Ukprn == int.Parse(Ukprn) && q.LarsCode == larsCode), It.IsAny<CancellationToken>()))
            .ReturnsAsync(availableProviderLocationsQueryResult);

            _sut.ModelState.AddModelError("key", "error");
            var result = await _sut.SubmitAProviderlocation(larsCode, model);

            _mediatorMock.Verify(m => m.Send(It.IsAny<AddProviderCourseLocationCommand>(), It.IsAny<CancellationToken>()), Times.Never);

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.ViewName.Should().Contain(ProviderCourseLocationAddController.ViewPath);
            var viewmodel = viewResult.Model as ProviderCourseLocationAddViewModel;
            viewmodel.Should().NotBeNull();
            viewmodel.BackLink.Should().Be(verifyUrl);
            viewmodel.CancelLink.Should().Be(verifyUrl);
        }
    }
}
