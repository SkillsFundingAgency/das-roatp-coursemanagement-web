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
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Queries.GetStandardInformation;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ProviderCourseDeleteControllerTests
{
    [TestFixture]
    public class ProviderCourseDeleteControllerGetTests
    {
        private Mock<IMediator> _mediatorMock;
        private ProviderCourseDeleteController _sut;
        string verifyUrl = "http://test";

        [SetUp]
        public void Before_Each_Test()
        {
            _mediatorMock = new Mock<IMediator>();

            _sut = new ProviderCourseDeleteController(_mediatorMock.Object, Mock.Of<ILogger<ProviderCourseDeleteController>>());
            _sut.AddDefaultContextWithUser()
               .AddUrlHelperMock()
               .AddUrlForRoute(RouteNames.GetStandardDetails, verifyUrl);
        }

        [Test, AutoData]
        public async Task Get_ValidRequest_ReturnsView(
            GetStandardInformationQueryResult queryResult,
            GetStandardDetailsQueryResult getStandardDetailsQueryResult,
            int larsCode)
        {
            _mediatorMock
                .Setup(m => m.Send(It.Is<GetStandardInformationQuery>(q => q.LarsCode == larsCode), It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResult);

            _mediatorMock
                .Setup(m => m.Send(It.Is<GetStandardDetailsQuery>(q => q.LarsCode == larsCode), It.IsAny<CancellationToken>()))
                .ReturnsAsync(getStandardDetailsQueryResult);

            var result = await _sut.GetProviderCourse(larsCode);

            _mediatorMock.Verify(m => m.Send(It.IsAny<GetStandardInformationQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetStandardDetailsQuery>(), It.IsAny<CancellationToken>()), Times.Once);

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult!.Model as ConfirmDeleteStandardViewModel;
            model.Should().NotBeNull();
        }

        [Test, AutoData]
        public async Task Get_StandardNotPresentAgainstProvider_ReturnsToReviewYourDetails(
            GetStandardInformationQueryResult queryResult,
            GetStandardDetailsQueryResult getStandardDetailsQueryResult,
            int larsCode)
        {
            _mediatorMock
                .Setup(m => m.Send(It.Is<GetStandardInformationQuery>(q => q.LarsCode == larsCode), It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResult);

            _mediatorMock
                .Setup(m => m.Send(It.Is<GetStandardDetailsQuery>(q => q.LarsCode == larsCode), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetStandardDetailsQueryResult)null);

            var result = await _sut.GetProviderCourse(larsCode);

            _mediatorMock.Verify(m => m.Send(It.IsAny<GetStandardInformationQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetStandardDetailsQuery>(), It.IsAny<CancellationToken>()), Times.Once);

            var viewResult = result as RedirectToRouteResult;
            viewResult.Should().NotBeNull();
            viewResult!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
        }

        [Test, AutoData]
        public async Task Get_InvalidRequest_ThrowsInvalidOperationException(int larsCode)
        {
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetStandardInformationQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetStandardInformationQueryResult)null);

            Func<Task> action = () => _sut.GetProviderCourse(larsCode);

            await action.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}
