using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ConfirmRegulatedStandardControllerTests
{
    [TestFixture]
    public class ConfirmRegulatedStandardControllerPostTests
    {
        private const string Ukprn = "10012002";
        private static string UserId = Guid.NewGuid().ToString();
        private static string DetailsUrl = Guid.NewGuid().ToString();
        private Mock<ILogger<EditCourseContactDetailsController>> _loggerMock;
        private Mock<IMediator> _mediatorMock;
        private Mock<IUrlHelper> _urlHelperMock;
        private ConfirmRegulatedStandardController _sut;

        [SetUp]
        public void Before_Each_Test()
        {
            _loggerMock = new Mock<ILogger<EditCourseContactDetailsController>>();
            _mediatorMock = new Mock<IMediator>();
            _urlHelperMock = new Mock<IUrlHelper>();
            _urlHelperMock
               .Setup(m => m.RouteUrl(It.Is<UrlRouteContext>(c => c.RouteName.Equals(RouteNames.ViewStandardDetails))))
               .Returns(DetailsUrl);

            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[] { new Claim(ProviderClaims.ProviderUkprn, Ukprn), new Claim(ProviderClaims.UserId, UserId)}, 
                "mock"));
            var httpContext = new DefaultHttpContext() { User = user };
            _sut = new ConfirmRegulatedStandardController(_mediatorMock.Object, _loggerMock.Object)
            {
                Url = _urlHelperMock.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };
        }

        [Test, AutoData]
        public async Task Post_InValidModel_ReturnsView(ConfirmRegulatedStandardViewModel model)
        {
            _sut.ModelState.AddModelError("key", "error");

            var result = await _sut.SubmitConfirmRegulatedStandard(model);

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.Model.Should().Be(model);
            model.BackLink.Should().Be(DetailsUrl);
            model.CancelLink.Should().Be(DetailsUrl);
        }
    }
}
