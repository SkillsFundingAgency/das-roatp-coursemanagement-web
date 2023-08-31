using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using System.Security.Claims;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers
{
    public static class ControllerExtensions
    {
        public static Controller AddDefaultContextWithUser(this Controller controller)
        {
            controller.ControllerContext = GetDefaultHttpContextWithUser(TestConstants.DefaultUkprn, TestConstants.DefaultUserId);
            controller.Url = Mock.Of<IUrlHelper>();
            return controller;
        }
        
        public static Controller AddDefaultContextWithDfEUser(this Controller controller)
        {
            controller.ControllerContext = GetDefaultHttpContextWithDfEUser(TestConstants.DefaultUkprn, TestConstants.DefaultDfEUserId);
            controller.Url = Mock.Of<IUrlHelper>();
            return controller;
        }

        public static Mock<IUrlHelper> AddUrlHelperMock(this Controller controller)
        {
            var urlHelperMock = new Mock<IUrlHelper>();
            controller.Url = urlHelperMock.Object;
            return urlHelperMock;
        }

        public static Mock<IUrlHelper> AddUrlForRoute(this Mock<IUrlHelper> urlHelperMock, string routeName, string url = TestConstants.DefaultUrl)
        {
            urlHelperMock
               .Setup(m => m.RouteUrl(It.Is<UrlRouteContext>(c => c.RouteName.Equals(routeName))))
               .Returns(url);
            return urlHelperMock;
        }

        private static ControllerContext GetDefaultHttpContextWithUser(string ukprn, string userId)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[]
                {
                    new Claim(ProviderClaims.ProviderUkprn, ukprn),
                    new Claim(ProviderClaims.UserId, userId)
                },
                "mock"));

            return new ControllerContext { HttpContext = new DefaultHttpContext() { User = user } };
        }
        
        private static ControllerContext GetDefaultHttpContextWithDfEUser(string ukprn, string userId)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[]
                {
                    new Claim(ProviderClaims.ProviderUkprn, ukprn),
                    new Claim(ProviderClaims.DfEUserId, userId)
                },
                "mock"));

            return new ControllerContext { HttpContext = new DefaultHttpContext() { User = user } };
        }
    }
}
