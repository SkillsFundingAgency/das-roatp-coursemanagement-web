using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAStandard.ReviewAndSaveStandardControllerTests
{
    [TestFixture]
    public class ReviewAndSaveStandardControllerGetTests
    {
        [Test, MoqAutoData]
        public void Get_ModelMissingFromSession_RedirectsToSelectAStandard(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] ReviewAndSaveStandardController sut)
        {
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns((StandardSessionModel)null);

            var result = sut.ReviewStandard();

            result.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.GetAddStandardSelectStandard);
        }

        [Test, MoqAutoData]
        public void Get_ModelInSessionWithLarsCode_ReturnsView(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] ReviewAndSaveStandardController sut,
            StandardSessionModel sessionModel,
            string cancelLink)
        {
            sut.AddDefaultContextWithUser().AddUrlHelperMock().AddUrlForRoute(RouteNames.ViewStandards, cancelLink);
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(sessionModel);

            var result = sut.ReviewStandard();

            result.As<ViewResult>().ViewName.Should().Be(ReviewAndSaveStandardController.ViewPath);
            result.As<ViewResult>().Model.As<StandardSessionModel>().CancelLink.Should().Be(cancelLink);
        }
    }
}
