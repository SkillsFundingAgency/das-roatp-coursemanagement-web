using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;


namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers
{
    [TestFixture]
    public class ControllerBaseTests
    {
        private const string _ukprn = "10012002";
        private static string _userId = Guid.NewGuid().ToString();
        private TestController _testController;
        [SetUp]
        public void Before_Each_Test()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] 
            { 
                new Claim(ProviderClaims.ProviderUkprn, _ukprn), 
                new Claim(ProviderClaims.UserId, _userId)
            }, "mock"));
            var httpContext = new DefaultHttpContext() { User = user };
            _testController = new TestController()
            {
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
                {
                    HttpContext = httpContext
                }
            };
        }

        [Test]
        public void Ukprn_ShouldGetValueFromContext()
        {
            _testController.GetUkprn().Should().Be(int.Parse(_ukprn));
        }

        [Test]
        public void UserId_ShouldGetValueFromContext()
        {
            _testController.GetUserId().Should().Be(_userId);
        }
    }

    public class TestController : ControllerBase
    {
        public int GetUkprn() => Ukprn;
        public string GetUserId() => UserId;
    }
}
