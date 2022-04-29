using System.Security.Claims;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers
{
    [TestFixture]
    public class HomeControllerTests
    {
        [Test, MoqAutoData]
        public void Index_RedirectsToReviewYourDetails(int ukprn, [Greedy] HomeController sut)
        {
            //Arrange
            var claim = new Claim(ProviderClaims.ProviderUkprn, ukprn.ToString());
            var claimsPrinciple = new ClaimsPrincipal(new[] { new ClaimsIdentity(new[] { claim }) });
            sut.ControllerContext = new ControllerContext() { HttpContext = new DefaultHttpContext() { User = claimsPrinciple } };

            //Act
            var actual = sut.Index() as RedirectToRouteResult;

            //Assert
            Assert.IsNotNull(actual);
            actual.RouteName.Should().Be(RouteNames.ReviewYourDetails);
            actual.RouteValues["ukprn"].Should().Be(ukprn.ToString());
        }
    }
}
