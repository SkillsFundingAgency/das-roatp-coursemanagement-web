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
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetProviderCourseDetails;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.EditCourseContactDetailsControllerTests
{
    [TestFixture]
    public class EditCourseContactDetailsControllerGetTests
    {
        private Mock<ILogger<EditCourseContactDetailsController>> _loggerMock;
        private Mock<IMediator> _mediatorMock;
        private EditCourseContactDetailsController _sut;

        [SetUp]
        public void Before_Each_Test()
        {
            _loggerMock = new Mock<ILogger<EditCourseContactDetailsController>>();
            _mediatorMock = new Mock<IMediator>();

            _sut = new EditCourseContactDetailsController(_mediatorMock.Object, _loggerMock.Object);
            _sut
                .AddDefaultContextWithUser();
        }

        [Test, AutoData]
        public async Task Get_ValidRequest_ReturnsView(
            GetProviderCourseDetailsQueryResult queryResult,
            string larsCode)
        {
            _mediatorMock
                .Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn == int.Parse(TestConstants.DefaultUkprn) && q.LarsCode == larsCode), It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResult);

            var result = await _sut.Index(larsCode);

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as EditCourseContactDetailsViewModel;
            model.Should().NotBeNull();
        }

        [Test, AutoData]
        public async Task Get_InvalidRequest_ThrowsInvalidOperationException(string larsCode)
        {
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetProviderCourseDetailsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetProviderCourseDetailsQueryResult)null);

            Func<Task> action = () => _sut.Index(larsCode);

            await action.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}
