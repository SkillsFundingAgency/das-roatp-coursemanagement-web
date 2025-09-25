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
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.UpdateContactDetails;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.EditCourseContactDetailsControllerTests
{
    [TestFixture]
    public class EditCourseContactDetailsControllerPostTests
    {
        private static string DetailsUrl = Guid.NewGuid().ToString();
        private Mock<IMediator> _mediatorMock;
        private EditCourseContactDetailsController _sut;

        [SetUp]
        public void Before_Each_Test()
        {
            _mediatorMock = new Mock<IMediator>();

            _sut = new EditCourseContactDetailsController(_mediatorMock.Object, Mock.Of<ILogger<EditCourseContactDetailsController>>());
            _sut
                .AddDefaultContextWithUser();
        }

        [Test, AutoData]
        public async Task Post_ValidModel_SendsUpdateCommand(CourseContactDetailsSubmitModel model, int larsCode)
        {
            var result = await _sut.Index(larsCode, model);

            _mediatorMock.Verify(m => m.Send(It.Is<UpdateProviderCourseContactDetailsCommand>(c => c.Ukprn == int.Parse(TestConstants.DefaultUkprn) && c.UserId == TestConstants.DefaultUserId && c.LarsCode == larsCode), It.IsAny<CancellationToken>()));
            var routeResult = result as RedirectToRouteResult;
            routeResult.Should().NotBeNull();
            routeResult.RouteName.Should().Be(RouteNames.GetStandardDetails);
            routeResult.RouteValues.Should().NotBeEmpty().And.HaveCount(2);
            routeResult.RouteValues.Should().ContainKey("ukprn").WhoseValue.Should().Be(int.Parse(TestConstants.DefaultUkprn));
            routeResult.RouteValues.Should().ContainKey("larsCode").WhoseValue.Should().Be(larsCode);
        }

        [Test, AutoData]
        public async Task Post_InvalidModel_ReturnsView(CourseContactDetailsSubmitModel model, GetStandardDetailsQueryResult queryResult, int larsCode)
        {
            _mediatorMock
                .Setup(m => m.Send(It.Is<GetStandardDetailsQuery>(q => q.Ukprn == int.Parse(TestConstants.DefaultUkprn) && q.LarsCode == larsCode), It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResult);
            _sut.ModelState.AddModelError("key", "error");

            var result = await _sut.Index(larsCode, model);

            _mediatorMock.Verify(m => m.Send(It.IsAny<UpdateProviderCourseContactDetailsCommand>(), It.IsAny<CancellationToken>()), Times.Never);

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
        }

        [Test, AutoData]
        public async Task Post_InvalidModelAndCourseDetailsNotFound_ThrowsException(CourseContactDetailsSubmitModel submitModel, int larsCode)
        {
            _mediatorMock
                .Setup(m => m.Send(It.Is<GetStandardDetailsQuery>(q => q.Ukprn == int.Parse(TestConstants.DefaultUkprn) && q.LarsCode == larsCode), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetStandardDetailsQueryResult)null);
            _sut.ModelState.AddModelError("key", "error");

            Func<Task> action = () => _sut.Index(larsCode, submitModel);

            await action.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}
