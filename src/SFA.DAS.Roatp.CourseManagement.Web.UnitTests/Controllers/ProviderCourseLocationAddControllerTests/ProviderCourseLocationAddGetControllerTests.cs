using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAvailableProviderLocations;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderCourseLocations;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ProviderCourseLocationAddControllerTests
{
    [TestFixture]
    public class ProviderCourseLocationAddGetControllerTests
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
        public async Task SelectAProviderlocation_ValidRequest_ReturnsView(
            GetAvailableProviderLocationsQueryResult availableProviderLocationsQueryResult,
            GetProviderCourseLocationsQueryResult resultProviderCourseLocations,
            int larsCode)
        {
            _mediatorMock
                .Setup(m => m.Send(It.Is<GetAvailableProviderLocationsQuery>(q => q.Ukprn == int.Parse(Ukprn) && q.LarsCode == larsCode), It.IsAny<CancellationToken>()))
                .ReturnsAsync(availableProviderLocationsQueryResult);

            var result = await _sut.SelectAProviderlocation(larsCode);

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult!.ViewName.Should().Contain(ProviderCourseLocationAddController.ViewPath);
            var model = viewResult.Model as ProviderCourseLocationAddViewModel;
            model.Should().NotBeNull();
        }
    }
}
