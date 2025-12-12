using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAStandard
{
    [TestFixture]
    public class AddAStandardControllerBaseTests
    {
        [Test]
        public void GetSessionModelWithEscapeRoute_SessionModelIsMissing_ReturnsEscapeRoute()
        {
            var sessionServiceMock = new Mock<ISessionService>();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns((StandardSessionModel)null);
            var ctrl = new TestController(sessionServiceMock.Object, Mock.Of<ILogger<TestController>>());
            ctrl.AddDefaultContextWithUser();

            var result = ctrl.TestAction();

            result.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.ReviewYourDetails);
        }

        [Test]
        public void GetSessionModelWithEscapeRoute_LarsCodeNotSetInSessionModel_ReturnsEscapeRoute()
        {
            var sessionServiceMock = new Mock<ISessionService>();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(new StandardSessionModel());
            var ctrl = new TestController(sessionServiceMock.Object, Mock.Of<ILogger<TestController>>());
            ctrl.AddDefaultContextWithUser();

            var result = ctrl.TestAction();

            result.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.ReviewYourDetails);
        }

        [Test]
        public void GetSessionModelWithEscapeRoute_LarsCodeSetInSessionModel_ReturnsSessionModel()
        {
            var larsCode = "1";
            var sessionServiceMock = new Mock<ISessionService>();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(new StandardSessionModel() { LarsCode = larsCode });
            var ctrl = new TestController(sessionServiceMock.Object, Mock.Of<ILogger<TestController>>());
            ctrl.AddDefaultContextWithUser();

            var result = ctrl.TestAction();

            result.As<OkObjectResult>().Value.As<StandardSessionModel>().LarsCode.Should().Be(larsCode);
        }
    }

    public class TestController : AddAStandardControllerBase
    {
        private readonly ILogger _logger;
        public TestController(ISessionService sessionService, ILogger<TestController> logger) : base(sessionService)
        {
            _logger = logger;
        }

        public IActionResult TestAction()
        {
            var (sessionModel, redirectResult) = GetSessionModelWithEscapeRoute(_logger);
            if (sessionModel == null) return redirectResult;
            return Ok(sessionModel);
        }
    }
}
