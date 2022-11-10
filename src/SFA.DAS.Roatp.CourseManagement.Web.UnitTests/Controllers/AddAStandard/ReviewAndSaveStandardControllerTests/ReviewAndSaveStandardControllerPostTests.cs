using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.AddProviderCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAStandard.ReviewAndSaveStandardControllerTests
{
    [TestFixture]
    public class ReviewAndSaveStandardControllerPostTests
    {
        [Test, MoqAutoData]
        public async Task Post_ModelMissingFromSession_RedirectsToSelectAStandard(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] ReviewAndSaveStandardController sut)
        {
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns((StandardSessionModel)null);

            var result = await sut.SaveStandard();

            result.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.GetAddStandardSelectStandard);
            mediatorMock.Verify(m => m.Send(It.IsAny<AddProviderCourseCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test, MoqAutoData]
        public async Task Post_InvokesCommand(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] ReviewAndSaveStandardController sut,
            StandardSessionModel standardSessionModel)
        {
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(standardSessionModel);

            var result = await sut.SaveStandard();

            mediatorMock.Verify(m => m.Send(It.Is<AddProviderCourseCommand>(c => c.Ukprn.ToString() == TestConstants.DefaultUkprn && c.LarsCode == standardSessionModel.LarsCode && c.UserId == TestConstants.DefaultUserId), It.IsAny<CancellationToken>()));
            result.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.ViewStandards);
        }
    }
}
