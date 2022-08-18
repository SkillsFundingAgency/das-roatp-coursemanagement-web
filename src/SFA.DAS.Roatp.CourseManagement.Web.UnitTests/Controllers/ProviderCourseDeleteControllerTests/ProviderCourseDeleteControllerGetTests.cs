using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Queries.GetStandardInformation;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using System;
using System.Threading;
using System.Threading.Tasks;

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
            int larsCode)
        {
            _mediatorMock
                .Setup(m => m.Send(It.Is<GetStandardInformationQuery>(q => q.LarsCode == larsCode), It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResult);

            var result = await _sut.GetProviderCourse(larsCode);

            _mediatorMock.Verify(m => m.Send(It.IsAny<GetStandardInformationQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as ConfirmDeleteStandardViewModel;
            model.Should().NotBeNull();
            model.BackUrl.Should().Be(verifyUrl);
            model.CancelUrl.Should().Be(verifyUrl);
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
