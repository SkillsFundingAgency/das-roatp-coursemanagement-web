using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using System;
using System.Security.Claims;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.EditLocationOptionControllerTests
{
    public static class TestConstants
    {
        public const string DefaultUkprn = "10012002";
        public static string DefaultUserId = Guid.NewGuid().ToString();
        public static string DefaultUrl = Guid.NewGuid().ToString();
    }
    public static class ControllerExtensions
    {
        public static ControllerContext GetDefaultHttpContextWithUser(string ukprn, string userId)
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

        public static Controller AddDefaultContextWithUser(this Controller controller)
        {
            controller.ControllerContext = GetDefaultHttpContextWithUser(TestConstants.DefaultUkprn, TestConstants.DefaultUserId);
            return controller;
        }

        public static Mock<IUrlHelper> AddDefaultUrlMock(this Controller controller, string routeName)
        {
            var urlHelperMock = new Mock<IUrlHelper>();
            urlHelperMock
               .Setup(m => m.RouteUrl(It.Is<UrlRouteContext>(c => c.RouteName.Equals(routeName))))
               .Returns(TestConstants.DefaultUrl);
            controller.Url = urlHelperMock.Object;
            return urlHelperMock;
        }
    }
}
