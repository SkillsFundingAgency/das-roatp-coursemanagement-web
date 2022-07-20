using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;


namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers
{
    [TestFixture]
    public class ControllerBaseTests
    {
        private TestController _testController;

        [SetUp]
        public void Before_Each_Test()
        {
            _testController = new TestController();
            _testController.AddDefaultContextWithUser();
        }

        [Test]
        public void Ukprn_ShouldGetValueFromContext()
        {
            _testController.GetUkprn().Should().Be(int.Parse(TestConstants.DefaultUkprn));
        }

        [Test]
        public void UserId_ShouldGetValueFromContext()
        {
            _testController.GetUserId().Should().Be(TestConstants.DefaultUserId);
        }

        [Test, AutoData]
        public void GetRedirectToResultWithUkprn_ShouldGetRedirectToRouteResultWithUkprn(string routeName)
        {
            var result = _testController.GetRedirectToResultWithUkprn(routeName);
            result.Should().NotBeNull();
            result.RouteValues.Should().HaveCount(1);
            result.RouteValues.ContainsKey("Ukprn");
            result.RouteValues.TryGetValue("Ukprn", out var ukprn);
            ukprn.ToString().Should().Be(TestConstants.DefaultUkprn);
        }
    }

    public class TestController : ControllerBase
    {
        public int GetUkprn() => Ukprn;
        public string GetUserId() => UserId;
        public Microsoft.AspNetCore.Mvc.RedirectToRouteResult GetRedirectToResultWithUkprn(string routeName) => RedirectToRouteWithUkprn(routeName);
    }
}
